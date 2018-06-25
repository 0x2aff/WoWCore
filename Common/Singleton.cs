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
