/*
 *  WoWCore - World of Warcraft 1.12 Server
 *  Copyright (C) 2017 exceptionptr
 *  
 *  This program is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *  
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *  
 *  You should have received a copy of the GNU General Public License
 *  along with this program.  If not, see <https://www.gnu.org/licenses/>.
 */

using System;
using System.IO;
using System.Threading.Tasks;
using WoWCore.AuthServer.Config;
using WoWCore.AuthServer.Packets;
using WoWCore.Common;
using WoWCore.Common.Config;
using WoWCore.Common.Helper;
using WoWCore.Common.Logging;
using WoWCore.Common.Network;

namespace WoWCore.AuthServer
{
    public class AuthServer : Singleton<AuthServer>
    {
        private readonly Server _server;

        private readonly string _listenerIp;
        private readonly int _listenerPort;

        public bool Running { get; private set; } = true;

        private AuthServer()
        {
            _listenerIp = ConfigManager.Instance.GetSettings<AuthConfig>().Connection.ServerIp;
            _listenerPort = ConfigManager.Instance.GetSettings<AuthConfig>().Connection.ServerPort;

            _server = new Server(_listenerIp, _listenerPort, null, null, MessageReceived);

            LogManager.Instance.Log(LogManager.LogType.Info, "Listening on " + _listenerIp + ":" + _listenerPort + ".");
        }

        public Task Start()
        {
            while (Running)
            {
                var userInput = Console.ReadLine();
                if (string.IsNullOrEmpty(userInput)) continue;

                switch (userInput)
                {
                    case "quit":
                        LogManager.Instance.Log(LogManager.LogType.Info, "Stopping the authentication server.");
                        Running = false;
                        break;
                }
            }

            _server.Dispose();

            return Task.CompletedTask;
        }

        private static bool MessageReceived(string ipPort, byte[] data)
        {
            return true;
        }
    }
}
