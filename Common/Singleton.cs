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
    /// Generic Singleton implementing the pattern in a thread safe and lazy way.
    /// </summary>
    /// <remarks>
    /// This class uses reflection to solve a limitation in the generics pattern to allocate the T type.
    /// If the T type has a public constructor it will throw an exception.
    /// </remarks>
    /// <typeparam name="T">The type that should be a unique instance.</typeparam>
    public abstract class Singleton<T> where T : class
    {
        public static T Instance => SingletonFactory.Instance;

        internal static class SingletonFactory
        {
            internal static T Instance;

            static SingletonFactory()
            {
                CreateInstance(typeof(T));
            }

            public static T CreateInstance(Type type)
            {
                var ctorsPublic = type.GetConstructors(BindingFlags.Instance | BindingFlags.Public);

                if(ctorsPublic.Length > 0)
                    throw new Exception(type.FullName + " has one or more public constructors so the " +
                                        "property cannot be enforced.");

                var ctorNonPublic = type.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null,
                    new Type[0], new ParameterModifier[0]);

                if (ctorNonPublic == null)
                    throw new Exception(type.FullName + " doesn't have a private/protected constructor so " +
                                        "the property cannot be enforced");

                try
                {
                    return Instance = (T) ctorNonPublic.Invoke(new object[0]);
                }
                catch (Exception e)
                {
                    throw new Exception("The Singleton couldn't be constructed, check if" + type.FullName + 
                        " has a default constructor", e);
                }
            }
        }
    }
}
