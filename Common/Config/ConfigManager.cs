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
using System.IO;

namespace WoWCore.Common.Config
{
    public sealed class ConfigManager : Singleton<ConfigManager>
    {
        private IConfig _settings;

        private ConfigManager() { }

        /// <summary>
        /// Register a loaded config file to make it globally available.
        /// </summary>
        /// <typeparam name="T"><see cref="IConfig"/></typeparam>
        /// <param name="fileName">The name/path of the config file.</param>
        public void RegisterSettings<T>(string fileName) where T : IConfig
        {
            try
            {
                _settings = JsonConvert.DeserializeObject<T>(File.ReadAllText(fileName));
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }    
        }

        /// <summary>
        /// Get the config.
        /// </summary>
        /// <typeparam name="T"><see cref="IConfig"/></typeparam>
        /// <returns><see cref="IConfig"/></returns>
        public T GetSettings<T>() where T : IConfig
        {
            return (T) _settings;
        }
    }
}
