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
using System.Reflection;

namespace WoWCore.Common.Logging
{
    public sealed class LogManager
    {
        #region Singleton

        private static volatile LogManager _instance;
        private static readonly object SyncRoot = new object();

        private LogManager() { }

        public static LogManager Instance
        {
            get
            {
                if (_instance != null) return _instance;
                lock (SyncRoot)
                    if (_instance == null)
                        _instance = new LogManager();

                return _instance;
            }
        }

        #endregion

        private FileStream _logFile;
        private StreamWriter _streamWriter;

        /// <summary>
        /// LogType to decide the output text.
        /// </summary>
        public enum LogType
        {
            Info,
            Warning,
            Error
        }

        /// <summary>
        /// Registers a new logger and creates/opens the logfile
        /// </summary>
        /// <param name="logFilePath">The path of the log file</param>
        public void RegisterLogger(string logFilePath)
        {
            if (_logFile == null)
            {
                try
                {
                    _logFile = File.Open(logFilePath, FileMode.Append, FileAccess.Write);
                    _streamWriter = new StreamWriter(_logFile) { AutoFlush = true };
                }
                catch (Exception e)
                {
                    Console.WriteLine("[" + typeof(LogManager) + "::" + MethodBase.GetCurrentMethod().Name +
                                      "]: Can't create/open log file.");
                    throw new Exception(e.Message);
                }
            }
            else
            {
                Console.WriteLine("[" + typeof(LogManager) + "::" + MethodBase.GetCurrentMethod().Name +
                                  "]: Logger already registered. Using old one.");
            }
        }

        /// <summary>
        /// Creates a new log messages.
        /// </summary>
        /// <param name="type"><see cref="LogType"/></param>
        /// <param name="message">The log message</param>
        public void Log(LogType type, string message)
        {
            if (_logFile == null)
            {
                throw new Exception("[" + typeof(LogManager) + "::" + MethodBase.GetCurrentMethod().Name +
                                    "]: Please use the Register Method of the LogManager!");
            }

            var combinedMessage = "[" + DateTime.Now + "][" + type + "] " + message;
            Console.WriteLine(combinedMessage);
            _streamWriter.WriteLine(combinedMessage);
        }
    }
}
