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
        private static void Main(string[] args) => new Program().Run().GetAwaiter();

        private Task Run()
        {
            Console.Title = "WoWCore Authentication Server";

            ConfigManager.Instance.RegisterSettings<AuthConfig>("AuthServerConfig.json");

            LogManager.Instance.RegisterLogger(ConfigManager.Instance.GetSettings<AuthConfig>().Logging.LogsDir +
                                               ConfigManager.Instance.GetSettings<AuthConfig>().Logging.LogFile);

            LogManager.Instance.Log(LogManager.LogType.Info, "┌───────────────────────────────────────────────────────────────────────┐");
            LogManager.Instance.Log(LogManager.LogType.Info, "│ ##      ##  #######  ##      ##  ######   #######  ########  ######## │");
            LogManager.Instance.Log(LogManager.LogType.Info, "│ ##  ##  ## ##     ## ##  ##  ## ##    ## ##     ## ##     ## ##       │");
            LogManager.Instance.Log(LogManager.LogType.Info, "│ ##  ##  ## ##     ## ##  ##  ## ##       ##     ## ##     ## ##       │");
            LogManager.Instance.Log(LogManager.LogType.Info, "│ ##  ##  ## ##     ## ##  ##  ## ##       ##     ## ########  ######   │");
            LogManager.Instance.Log(LogManager.LogType.Info, "│ ##  ##  ## ##     ## ##  ##  ## ##       ##     ## ##   ##   ##       │");
            LogManager.Instance.Log(LogManager.LogType.Info, "│ ##  ##  ## ##     ## ##  ##  ## ##    ## ##     ## ##    ##  ##       │");
            LogManager.Instance.Log(LogManager.LogType.Info, "│  ###  ###   #######   ###  ###   ######   #######  ##     ## ######## │");
            LogManager.Instance.Log(LogManager.LogType.Info, "└───────────────────────────────────────────────────────────────────────┘");

            return Task.CompletedTask;
        }
    }
}
