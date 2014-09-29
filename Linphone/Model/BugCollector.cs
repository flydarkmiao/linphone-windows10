﻿using Microsoft.Phone.Tasks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.Storage;

namespace Linphone.Model
{
    /// <summary>
    /// Collects and reports uncatched exceptions
    /// </summary>
    public class BugCollector
    {
        const string exceptionsFileName = "exceptions.log";
        const string logFileName = "Linphone.log";

        internal static void LogException(Exception e, string extra)
        {
            try
            {
                using (var store = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    using (TextWriter output = new StreamWriter(store.OpenFile(exceptionsFileName, FileMode.Append)))
                    {
                        output.WriteLine("-------------------------");
                        output.WriteLine("Type: {0}", extra);
                        output.WriteLine("Message: {0}", e.Message);
                        foreach (KeyValuePair<string, string> kvp in e.Data)
                        {
                            output.WriteLine("Data: Key= {0}, Value= {1}", kvp.Key, kvp.Value);
                        }
                        output.WriteLine("Stacktrace: {0}", e.StackTrace);
                        output.Flush();
                        output.Close();
                    }
                }
            }
            catch (Exception) { }
        }

        internal static void LogMessage(string message)
        {
            try
            {
                using (var store = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    using (TextWriter output = new StreamWriter(store.OpenFile(exceptionsFileName, FileMode.Append)))
                    {
                        output.WriteLine("Custom message: {0}", message);
                        output.Flush();
                        output.Close();
                    }
                }
            }
            catch (Exception) { }
        }

        internal static bool HasExceptionToReport()
        {
            try
            {
                using (var store = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    return store.FileExists(exceptionsFileName);
                }
            }
            catch (Exception) { }

            return false;
        }

        internal static async Task<bool> HasLinphoneLogFile()
        {
            ApplicationSettingsManager appSettings = new ApplicationSettingsManager();
            appSettings.Load();

            try
            {
                StorageFile file = await ApplicationData.Current.LocalFolder.GetFileAsync(appSettings.LogOption);
                return file != null;
            }
            catch (FileNotFoundException)
            {
                return false;
            }
        }

        internal static async void ReportExceptions()
        {
            try
            {
                string body = "";
                body += "Version of the app : " + XDocument.Load("WMAppManifest.xml").Root.Element("App").Attribute("Version").Value;
                body += "--------------------";

                using (var store = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if (store.FileExists(exceptionsFileName))
                    {
                        using (TextReader input = new StreamReader(store.OpenFile(exceptionsFileName, FileMode.Open)))
                        {
                            body += input.ReadToEnd();
                            input.Close();
                        }
                    }
                    if (store.FileExists(logFileName))
                    {
                        body += "\r\n"; 
                        // Limit the amount of linphone logs
                        string logs = await ReadLogs();
                        body += logs.Length > 32000 ? logs.Substring(logs.Length - 32000) : logs;
                    }
                }
                EmailComposeTask email = new EmailComposeTask();
                email.To = "linphone-wphone@belledonne-communications.com";
                email.Subject = "Exception report";
                email.Body = body;
                email.Show();
            }
            catch (Exception) { }
        }

        internal static async Task<string> ReadLogs()
        {
            ApplicationSettingsManager appSettings = new ApplicationSettingsManager();
            appSettings.Load();

            byte[] data;
            StorageFolder folder = ApplicationData.Current.LocalFolder;
            StorageFile file = await folder.GetFileAsync(appSettings.LogOption);

            using (Stream s = await file.OpenStreamForReadAsync())
            {
                data = new byte[s.Length];
                await s.ReadAsync(data, 0, (int)s.Length);
            }

            return Encoding.UTF8.GetString(data, 0, data.Length);
        }

        internal static void DeleteFile()
        {
            try
            {
                using (var store = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    store.DeleteFile(exceptionsFileName);
                }
            }
            catch (Exception) { }
        }

        internal static void DeleteLinphoneLogFileIfFileTooBig()
        {
            bool delete = false;
            try
            {
                using (var store = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if (store.FileExists(logFileName))
                    {
                        using (var file = store.OpenFile(logFileName, FileMode.Open))
                        {
                            Debug.WriteLine("[BugCollector] Log file size is " + file.Length);
                            if (file.Length > 200000)
                            {
                                delete = true;
                            }
                        }
                        if (delete)
                        {
                            store.DeleteFile(logFileName);
                        }
                    }
                }
            }
            catch (Exception) { }
        }

        internal static async void DeleteLinphoneLogFile()
        {
            ApplicationSettingsManager appSettings = new ApplicationSettingsManager();
            appSettings.Load();

            StorageFile file = await ApplicationData.Current.LocalFolder.GetFileAsync(appSettings.LogOption);
            if (file == null)
                return;

            try
            {
                await file.DeleteAsync();
            }
            catch (UnauthorizedAccessException)
            {
                Debug.WriteLine("Can't delete linphone logs...");
            }
        }
    }
}
