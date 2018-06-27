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
    /// <inheritdoc />
    /// <summary>
    ///     Defines the authentication server.
    /// </summary>
    public class AuthServer : Singleton<AuthServer>
    {
        private readonly Server _server;

        /// <summary>
        ///     Instantiates the <see cref="AuthServer" /> class.
        /// </summary>
        private AuthServer()
        {
            var listenerIp = ConfigManager.Instance.GetSettings<AuthConfig>().Connection.ServerIp;
            var listenerPort = ConfigManager.Instance.GetSettings<AuthConfig>().Connection.ServerPort;

            _server = new Server(listenerIp, listenerPort, ClientConnected, ClientDisconnected, MessageReceived);

            LogManager.Instance.Log(LogManager.LogType.Success,
                $"Listening on {listenerIp}:{listenerPort}.");
        }

        public bool Running { get; private set; } = true;

        /// <summary>
        ///     Starts the authentication server runtime.
        /// </summary>
        /// <returns></returns>
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
                    default:
                        LogManager.Instance.Log(LogManager.LogType.Warning, "User input not valid. Ignoring command ...");
                        break;
                }
            }

            _server.Dispose();

            return Task.CompletedTask;
        }

        private static bool ClientConnected(string ip, int port)
        {
            LogManager.Instance.Log(LogManager.LogType.Info, $"Client ({ip}:{port}) connected.");
            return true;
        }

        private static bool ClientDisconnected(string ip, int port)
        {
            LogManager.Instance.Log(LogManager.LogType.Info, $"Client ({ip}:{port}) disconnected.");
            return true;
        }

        private static bool MessageReceived(string ip, int port, byte[] data)
        {
            return true;
        }
    }
}