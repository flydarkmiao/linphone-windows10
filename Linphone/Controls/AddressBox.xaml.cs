﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Linphone.Controls
{
    /// <summary>
    /// Custom control representing a textbox that contains a SIP address
    /// </summary>
    public partial class AddressBox : UserControl
    {
        /// <summary>
        /// String content of the textbox.
        /// </summary>
        public String Text
        {
            get { return address.Text; }
            set { address.Text = value; }
        }

        /// <summary>
        /// Public constructor.
        /// </summary>
        public AddressBox()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        private void backspace_Hold_1(object sender, System.Windows.Input.GestureEventArgs e)
        {
            address.Text = "";
        }

        private void backspace_Click_1(object sender, RoutedEventArgs e)
        {
            if (address.Text.Length > 0)
                address.Text = address.Text.Substring(0, address.Text.Length - 1);
        }

        private void Image_ManipulationStarted_1(object sender, ManipulationStartedEventArgs e)
        {
            Image image = (sender as Image);
            image.Source = new BitmapImage(new Uri("/Assets/backspace_over.png", UriKind.RelativeOrAbsolute));
        }

        private void Image_ManipulationCompleted_1(object sender, ManipulationCompletedEventArgs e)
        {
            Image image = (sender as Image);
            image.Source = new BitmapImage(new Uri("/Assets/backspace_default.png", UriKind.RelativeOrAbsolute));
        }

        // This is needed for the textbox hack, see comment in .xaml file
        private void address_TextChanged_1(object sender, TextChangedEventArgs e)
        {
            behindAddress.Text = address.Text;
        }
    }
}
