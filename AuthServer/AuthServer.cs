/* MIT License
 * 
 * Copyright (c) 2019 Stanislaw Schlosser <https://github.com/0x2aff>
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

using System;
using System.Threading.Tasks;
using WoWCore.AuthServer.Config;
using WoWCore.Common;
using WoWCore.Common.Config;
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
