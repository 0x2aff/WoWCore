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

using WoWCore.AuthServer.Opcodes;
using WoWCore.Common.Helper.Attributes;

namespace WoWCore.AuthServer.Packets
{
    public class AuthLogonChallengeClient
    {
        public AuthCmd Command;
        public AuthResult Error;
        public ushort Size;

        [PacketString(StringType.CString), PacketArrayReverse]
        public string GameName;

        public byte Version1;
        public byte Version2;
        public byte Version3;
        public ushort Build;

        [PacketString(StringType.CString), PacketArrayReverse]
        public string Platform;

        [PacketString(StringType.CString), PacketArrayReverse]
        public string OperatingSystem;

        [PacketString(StringType.FixedLength), PacketArrayLength(4), PacketArrayReverse]
        public string Country;

        public uint TimezoneBias;
        public uint Ip;

        [PacketString(StringType.PrefixedLength)]
        public string Identifier;
    }
}
