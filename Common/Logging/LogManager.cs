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
using System.IO;
using System.Reflection;

namespace WoWCore.Common.Logging
{
    public sealed class LogManager : Singleton<LogManager>
    {
        private readonly StreamWriter _streamWriter;

        private LogManager()
        {
            try
            {
                var fullPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "/data/logs/";

                if (!Directory.Exists(fullPath))
                {
                    Directory.CreateDirectory(fullPath);
                }

                var logFile = File.Open(fullPath + "[" + DateTime.Now.ToString("dd.MM.yy") + "] " + Assembly.GetEntryAssembly().GetName().Name + ".log", FileMode.Append, FileAccess.Write);
                _streamWriter = new StreamWriter(logFile) { AutoFlush = true };
            }
            catch (Exception e)
            {
                Console.WriteLine("[" + typeof(LogManager) + "::" + MethodBase.GetCurrentMethod().Name +
                                  "]: Can't create/open log file.");
                throw new Exception(e.Message);
            }
        }

        /// <summary>
        /// LogType to decide the output text.
        /// </summary>
        public enum LogType
        {
            Info,
            Success,
            Warning,
            Error
        }

        /// <summary>
        /// Creates a new log messages.
        /// </summary>
        /// <param name="type"><see cref="LogType"/></param>
        /// <param name="message">The log message</param>
        public void Log(LogType type, string message)
        {
            var combinedMessage = "[" + DateTime.Now + "][" + type + "] " + message;
            switch (type)
            {
                case LogType.Info:
                    break;
                case LogType.Success:
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;
                case LogType.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case LogType.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }

            Console.WriteLine(combinedMessage);
            Console.ForegroundColor = ConsoleColor.Gray;

            _streamWriter.WriteLine(combinedMessage);
        }
    }
}
