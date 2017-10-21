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
using System.Reflection;

namespace WoWCore.Common
{
    /// <summary>
    /// Static factory class for implementing the singleton pattern on Types
    /// which contain a private, parameter-less constructor.
    /// </summary>
    /// <typeparam name="T">The underlying singleton type.</typeparam>
    public abstract class Singleton<T> where T : Singleton<T>
    {
        // ReSharper disable once InconsistentNaming
        private static readonly Lazy<T> _instance;

        public static T Instance => _instance.Value;

        static Singleton()
        {
            _instance = new Lazy<T>(() =>
            {
                try
                {
                    // Binding the flags include private constructor.
                    var ctor = typeof(T).GetConstructor(
                        BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Type.EmptyTypes,
                        null);

                    return (T) ctor.Invoke(null);
                }
                catch (Exception e)
                {
                    throw new SingletonConstructorException(e);
                }
            });
        }
    }
}
