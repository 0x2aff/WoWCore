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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using WoWCore.Common.Extensions;
using WoWCore.Common.Helper.Attributes;

namespace WoWCore.Common.Helper
{
    public static class PacketHelper
    {
        public static T Parse<T>(BinaryReader binaryReader) where T : class, new()
        {
            return (T) Parse(binaryReader, typeof(T));
        }

        private static object Parse(BinaryReader binaryReader, Type type)
        {
            var instance = Activator.CreateInstance(type);

            foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .OrderBy(field => field.MetadataToken))
            {
                field.SetValue(instance, ParseField(binaryReader, field.FieldType, field.GetCustomAttributes()));
            }

            return instance;
        }

        private static object ParseField(BinaryReader binaryReader, Type fieldType, IEnumerable<Attribute> attributes)
        {
            if (fieldType.IsArray)
                return ParseArray(binaryReader, fieldType, attributes);

            if (fieldType.IsPrimitive || fieldType == typeof(string))
                return ParsePrimitive(binaryReader, Type.GetTypeCode(fieldType), attributes);

            if (fieldType.IsEnum)
                return ParsePrimitive(binaryReader, Type.GetTypeCode(fieldType.GetEnumUnderlyingType()), attributes);

            if (fieldType.IsClass)
                return Parse(binaryReader, fieldType);

            throw new Exception("[" + typeof(PacketHelper) + "::" + MethodBase.GetCurrentMethod().Name +
            "]: Can't parse field.");
        }

        private static object ParseArray(BinaryReader binaryReader, Type arrayType, IEnumerable<Attribute> attributes)
        {
            var attributeArray = attributes as Attribute[] ?? attributes.ToArray();

            var arrayLengthAttribute = attributeArray.OfType<PacketArrayLengthAttribute>().FirstOrDefault();
            if (arrayLengthAttribute == null)
                throw new Exception("[" + typeof(PacketHelper) + "::" + MethodBase.GetCurrentMethod().Name +
                                    "]: Array is missing the ArrayLength attribute.");

            var elementType = arrayType.GetElementType();
            var newArray = Array.CreateInstance(elementType, arrayLengthAttribute.Length);

            for (var i = 0; i < newArray.Length; i++)
                newArray.SetValue(ParseField(binaryReader, elementType, attributeArray), i);

            return newArray;
        }

        private static object ParsePrimitive(BinaryReader binaryReader, TypeCode typeCode,
            IEnumerable<Attribute> attributes)
        {
            var attributeArray = attributes as Attribute[] ?? attributes.ToArray();
            var isBigEndian = attributeArray.OfType<PacketStringAttribute>().FirstOrDefault() != null;

            switch (typeCode)
            {
                case TypeCode.Boolean:
                    return binaryReader.ReadBoolean();
                case TypeCode.Byte:
                    return binaryReader.ReadByte();
                case TypeCode.Char:
                    return binaryReader.ReadChar();
                case TypeCode.Decimal:
                    return binaryReader.ReadDecimal();
                case TypeCode.Double:
                    return binaryReader.ReadDouble();
                case TypeCode.Int16:
                    return isBigEndian ? BitConverter.ToInt16(binaryReader.ReadBytes(2), 0) : binaryReader.ReadInt16();
                case TypeCode.Int32:
                    return isBigEndian ? BitConverter.ToInt32(binaryReader.ReadBytes(4), 0) : binaryReader.ReadInt32();
                case TypeCode.Int64:
                    return isBigEndian ? BitConverter.ToInt64(binaryReader.ReadBytes(8), 0) : binaryReader.ReadInt64();
                case TypeCode.SByte:
                    return binaryReader.ReadSByte();
                case TypeCode.Single:
                    return binaryReader.ReadSingle();
                case TypeCode.String:
                    var stringAttribute = attributeArray.OfType<PacketStringAttribute>().FirstOrDefault();
                    if (stringAttribute == null)
                        throw new Exception("[" + typeof(PacketHelper) + "::" + MethodBase.GetCurrentMethod().Name +
                                            "]: String is missing the StringType attribute.");

                    switch (stringAttribute.StringType)
                    {
                        case StringType.CString:
                            return binaryReader.ReadCSTring();
                        case StringType.PrefixedLength:
                            var length = binaryReader.ReadByte();
                            Console.WriteLine(length);
                            return new string(binaryReader.ReadChars(length));
                        case StringType.FixedLength:
                            var arrayAttributes = attributeArray.OfType<PacketArrayLengthAttribute>().FirstOrDefault();
                            if (arrayAttributes == null)
                                throw new Exception("[" + typeof(PacketHelper) + "::" + MethodBase.GetCurrentMethod().Name +
                                                    "]: String is missing the ArrayLength attribute.");

                            return new string(binaryReader.ReadChars(arrayAttributes.Length));
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case TypeCode.UInt16:
                    return isBigEndian ? BitConverter.ToUInt16(binaryReader.ReadBytes(2), 0) : binaryReader.ReadUInt16();
                case TypeCode.UInt32:
                    return isBigEndian ? BitConverter.ToUInt32(binaryReader.ReadBytes(4), 0) : binaryReader.ReadUInt32();
                case TypeCode.UInt64:
                    return isBigEndian ? BitConverter.ToUInt32(binaryReader.ReadBytes(8), 0) : binaryReader.ReadUInt64();
                default:
                    throw new ArgumentOutOfRangeException(nameof(typeCode), typeCode, null);
            }
        }
    }
}
