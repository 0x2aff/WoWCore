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

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace WoWCore.Common.Config
{
    public sealed class ConfigManager : Singleton<ConfigManager>
    {
        /// <summary>
        /// The type of configs.
        /// </summary>
        public enum ConfigType
        {
            AuthServer,
            GameServer
        }

        private readonly Dictionary<ConfigType, IConfig> _settingsRegister = new Dictionary<ConfigType, IConfig>();

        /// <summary>
        /// Register a loaded config file to make it globally available.
        /// </summary>
        /// <typeparam name="T"><see cref="IConfig"/></typeparam>
        /// <param name="type"><see cref="ConfigType"/></param>
        /// <param name="fileName">The name/path of the config file.</param>
        public void RegisterSettings<T>(ConfigType type, string fileName) where T : IConfig
        {
            IConfig setting;

            try
            {
                var jsonConfig = File.ReadAllText(fileName);
                setting = JsonConvert.DeserializeObject<T>(jsonConfig) as IConfig;
            }
            catch (Exception)
            {
                throw new Exception();
            }

            try
            {
                _settingsRegister.Add(type, setting);

            }
            catch (ArgumentException)
            {
                Console.WriteLine("[WARNING][" + typeof(ConfigManager) + "::" + MethodBase.GetCurrentMethod().Name + "]: An element with the same key already exists in the dictionary.");
            }        
        }

        /// <summary>
        /// Get the config.
        /// </summary>
        /// <typeparam name="T"><see cref="IConfig"/></typeparam>
        /// <param name="type"><see cref="ConfigType"/></param>
        /// <returns><see cref="IConfig"/> by the registered <see cref="ConfigType"/></returns>
        public T GetSettings<T>(ConfigType type) where T : IConfig
        {
            return (T) _settingsRegister.First(x => x.Key == type).Value;
        }
    }
}
