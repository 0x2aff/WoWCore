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
using WoWCore.Common.Config;
using WoWCore.Common.Logging;

namespace WoWCore.AuthServer
{
    /// <summary>
    /// Entrypoint class responsible for the authentication server.
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// Entrypoint of the authentication server.
        /// </summary>
        /// <param name="args"></param>
        private static void Main(string[] args) => Initialize();

        private static void Initialize()
        {
            Console.Title = "WoWCore Authentication Server";

            ConfigManager.Instance.RegisterSettings<AuthConfig>("AuthServerConfig.json");

            LogManager.Instance.Log(LogManager.LogType.Info, "┌─────────────────────────────────────────────────────────────────┐");
            LogManager.Instance.Log(LogManager.LogType.Info, "│ WoWCore - World of Warcraft 1.12.x Server                       │");
            LogManager.Instance.Log(LogManager.LogType.Info, "│ Copyright (C) 2017 exceptionptr                                 │");
            LogManager.Instance.Log(LogManager.LogType.Info, "├─────────────────────────────────────────────────────────────────┤");
            LogManager.Instance.Log(LogManager.LogType.Info, "│ This program is distributed in the hope that it will be useful, │");
            LogManager.Instance.Log(LogManager.LogType.Info, "│ but WITHOUT ANY WARRANTY; without even the implied warranty of  │");
            LogManager.Instance.Log(LogManager.LogType.Info, "│ MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the   │");
            LogManager.Instance.Log(LogManager.LogType.Info, "│ GNU General Public License for more details.                    │");
            LogManager.Instance.Log(LogManager.LogType.Info, "└─────────────────────────────────────────────────────────────────┘");

            AuthServer.Instance.Start();
        }
    }
}
