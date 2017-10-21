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
using System.Runtime.Serialization;

namespace WoWCore.Common
{
    /// <inheritdoc cref="Exception" />
    /// <summary>
    /// Represents constructor errors that occur while creating a singleton.
    /// </summary>
    public class SingletonConstructorException : Exception
    {
        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        public SingletonConstructorException() { }

        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public SingletonConstructorException(string message) : base(message) { }

        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance with a reference to the inner
        /// exception that is the cause of this exception.
        /// </summary>
        /// <param name="innerException">The exception that is the cause of the current exception
        /// or a null reference if no inner exception is specified.</param>
        public SingletonConstructorException(Exception innerException) : base(null, innerException) { }

        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance with a specified error message and a
        /// reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception
        /// or a null reference if no inner exception is specified.</param>
        public SingletonConstructorException(string message, Exception innerException) : base(message, innerException) { }

        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance with serialized data.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/> that holds the 
        /// serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="StreamingContext"/> that contains 
        /// contextual information about the source or destination.</param>
        /// <exception cref="ArgumentNullException">The info parameter is null.</exception>
        /// <exception cref="SerializationException">The class name is null or System.Exception.HResult is zero (0).</exception>
        protected SingletonConstructorException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
