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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using WoWCore.Common.Extensions;
using WoWCore.Common.Helper.Attributes;

namespace WoWCore.Common.Helper
{
    public static class PacketHelper
    {
        /// <summary>
        /// Parse the client packet.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static T Parse<T>(byte[] data) where T : class, new()
        {
            return (T) Parse(new BinaryReader(new MemoryStream(data)), typeof(T));
        }

        /// <summary>
        /// Build the client packet.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static byte[] Build<T>(T instance) where T : class, new()
        {
            using (var memoryStream = new MemoryStream())
            using (var binaryWriter = new BinaryWriter(memoryStream))
            {
                Build(binaryWriter, typeof(T), instance);
                return memoryStream.ToArray();
            }
        }

        private static void Build(BinaryWriter binaryWriter, IReflect type, object instance)
        {
            foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .OrderBy(field => field.MetadataToken))
            {
                BuildField(binaryWriter, field.FieldType, field.GetValue(instance), field.GetCustomAttributes());
            }
        }

        private static void BuildField(BinaryWriter binaryWriter, Type fieldType, object value,
            IEnumerable<Attribute> attributes)
        {
            if (value == null) return;

            if (fieldType.IsArray)
                BuildArray(binaryWriter, (Array) value, attributes);
            else if (fieldType.IsPrimitive || fieldType == typeof(string))
                BuildPrimitive(binaryWriter, Type.GetTypeCode(fieldType), value, attributes);
            else if (fieldType.IsEnum)
                BuildPrimitive(binaryWriter, Type.GetTypeCode(fieldType.GetEnumUnderlyingType()), value, attributes);
            else if (fieldType.IsClass && !fieldType.IsArray && fieldType != typeof(string))
                Build(binaryWriter, fieldType, value);
            else throw new Exception("[" + typeof(PacketHelper) + "::" + MethodBase.GetCurrentMethod().Name +
                                "]: Can't build / determine field.");
        }

        private static void BuildArray(BinaryWriter binaryWriter, IEnumerable array, IEnumerable<Attribute> attributes)
        {
            var attributeArray = attributes as Attribute[] ?? attributes.ToArray();
            var elementType = array.GetType().GetElementType();
            foreach (var element in array)
            {
                BuildField(binaryWriter, elementType, element, attributeArray);
            }
        }

        private static void BuildPrimitive(BinaryWriter binaryWriter, TypeCode typeCode, object value,
            IEnumerable<Attribute> attributes)
        {
            switch (typeCode)
            {
                case TypeCode.Boolean:
                    binaryWriter.Write((bool)value);
                    break;
                case TypeCode.Byte:
                    binaryWriter.Write((byte)value);
                    break;
                case TypeCode.Char:
                    binaryWriter.Write((char)value);
                    break;
                case TypeCode.Double:
                    binaryWriter.Write((double)value);
                    break;
                case TypeCode.Empty:
                    break;
                case TypeCode.Int16:
                    binaryWriter.Write((short)value);
                    break;
                case TypeCode.Int32:
                    binaryWriter.Write((int)value);
                    break;
                case TypeCode.Int64:
                    binaryWriter.Write((long)value);
                    break;
                case TypeCode.Object:
                    break;
                case TypeCode.SByte:
                    binaryWriter.Write((sbyte)value);
                    break;
                case TypeCode.Single:
                    binaryWriter.Write((float)value);
                    break;
                case TypeCode.String:
                    var attributeArray = attributes as Attribute[] ?? attributes.ToArray();
                    var stringAttributes = attributeArray.OfType<PacketStringAttribute>().FirstOrDefault();

                    if (stringAttributes == null)
                        throw new Exception("[" + typeof(PacketHelper) + "::" + MethodBase.GetCurrentMethod().Name +
                                            "]: String is missing the PacketString attribute.");

                    var reverseArrayAttribute = attributeArray.OfType<PacketArrayReverseAttribute>().FirstOrDefault();

                    switch (stringAttributes.StringType)
                    {
                        case StringType.CString:
                            binaryWriter.Write(reverseArrayAttribute != null
                                ? Encoding.ASCII.GetBytes(new string(((string) value).Reverse().ToArray()) + '\0')
                                : Encoding.ASCII.GetBytes((string) value + '\0'));
                            break;
                        case StringType.PrefixedLength:
                            binaryWriter.Write((byte)((string)value).Length);

                            binaryWriter.Write(reverseArrayAttribute != null
                                ? Encoding.ASCII.GetBytes(new string(((string) value).Reverse().ToArray()))
                                : Encoding.ASCII.GetBytes((string) value));
                            break;
                        case StringType.FixedLength:
                            var arrayAttributes = attributeArray.OfType<PacketArrayLengthAttribute>().FirstOrDefault();
                            if (arrayAttributes == null)
                                throw new Exception("[" + typeof(PacketHelper) + "::" + MethodBase.GetCurrentMethod().Name +
                                                    "]: String is missing the PacketArrayLength attribute.");

                            binaryWriter.Write(reverseArrayAttribute != null
                                ? Encoding.ASCII.GetBytes(new string(((string) value).Reverse().ToArray()))
                                    .Fill(arrayAttributes.Length)
                                : Encoding.ASCII.GetBytes((string) value).Fill(arrayAttributes.Length));
                            break;
                    }
                    break;
                case TypeCode.UInt16:
                    binaryWriter.Write((ushort)value);
                    break;
                case TypeCode.UInt32:
                    binaryWriter.Write((uint)value);
                    break;
                case TypeCode.UInt64:
                    binaryWriter.Write((ulong)value);
                    break;
                case TypeCode.DateTime:
                case TypeCode.DBNull:
                case TypeCode.Decimal:
                    throw new Exception("[" + typeof(PacketHelper) + "::" + MethodBase.GetCurrentMethod().Name +
                                        "]: Can't build unsupported field.");
                default:
                    throw new ArgumentOutOfRangeException(nameof(typeCode), typeCode, null);
            }
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
                                            "]: String is missing the PacketString attribute.");

                    var reverseArrayAttribute = attributeArray.OfType<PacketArrayReverseAttribute>().FirstOrDefault();

                    switch (stringAttribute.StringType)
                    {
                        case StringType.CString:
                            return reverseArrayAttribute != null
                                ? new string(binaryReader.ReadCSTring().Reverse().ToArray())
                                : binaryReader.ReadCSTring();
                        case StringType.PrefixedLength:
                            return reverseArrayAttribute != null
                                ? new string(binaryReader.ReadChars(binaryReader.ReadByte()).Reverse().ToArray())
                                : new string(binaryReader.ReadChars(binaryReader.ReadByte()));
                        case StringType.FixedLength:
                            var arrayAttributes = attributeArray.OfType<PacketArrayLengthAttribute>().FirstOrDefault();
                            if (arrayAttributes == null)
                                throw new Exception("[" + typeof(PacketHelper) + "::" + MethodBase.GetCurrentMethod().Name +
                                                    "]: String is missing the PacketArrayLength attribute.");

                            return reverseArrayAttribute != null
                                ? new string(binaryReader.ReadChars(arrayAttributes.Length).Reverse().ToArray())
                                : new string(binaryReader.ReadChars(arrayAttributes.Length));
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case TypeCode.UInt16:
                    return isBigEndian ? BitConverter.ToUInt16(binaryReader.ReadBytes(2), 0) : binaryReader.ReadUInt16();
                case TypeCode.UInt32:
                    return isBigEndian ? BitConverter.ToUInt32(binaryReader.ReadBytes(4), 0) : binaryReader.ReadUInt32();
                case TypeCode.UInt64:
                    return isBigEndian ? BitConverter.ToUInt32(binaryReader.ReadBytes(8), 0) : binaryReader.ReadUInt64();
                case TypeCode.DateTime:
                case TypeCode.DBNull:
                case TypeCode.Empty:
                case TypeCode.Object:
                    throw new Exception("[" + typeof(PacketHelper) + "::" + MethodBase.GetCurrentMethod().Name +
                                        "]: Can't parse unsupported field.");
                default:
                    throw new ArgumentOutOfRangeException(nameof(typeCode), typeCode, null);
            }
        }
    }
}
