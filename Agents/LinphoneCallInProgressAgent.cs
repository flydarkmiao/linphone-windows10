﻿/*
LinphoneCallInProgressAgent.cs
Copyright (C) 2015  Belledonne Communications, Grenoble, France
This program is free software; you can redistribute it and/or
modify it under the terms of the GNU General Public License
as published by the Free Software Foundation; either version 2
of the License, or (at your option) any later version.
This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.
You should have received a copy of the GNU General Public License
along with this program; if not, write to the Free Software
Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
*/

using Microsoft.Phone.Networking.Voip;
using Linphone.Core;
using Linphone.Core.OutOfProcess;
using System.Diagnostics;

namespace Linphone.Agents
{
    public class LinphoneCallInProgressAgent : VoipCallInProgressAgent
    {
        public LinphoneCallInProgressAgent() : base()
        {
        }

        /// <summary>
        /// Called when the first call has started.
        /// </summary>
        protected override void OnFirstCallStarting()
        {
            Debug.WriteLine("[LinphoneCallInProgressAgent] The first call has started.");
            AgentHost.OnAgentStarted();
        }

        /// <summary>
        /// Called when the last call has ended.
        /// </summary>
        protected override void OnCancel()
        {
            Debug.WriteLine("[LinphoneCallInProgressAgent] The last call has ended. Calling NotifyComplete");
            base.NotifyComplete();
        }
    }
}