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
using System.Linq;
using System.Text;
using WoWCore.AuthServer.Opcodes;

namespace WoWCore.AuthServer.Packets
{
    public class AuthLogonChallengeClient
    {
        public AuthCmd Command;
        public AuthResult Error;
        public ushort Size;
        public string GameName;
        public byte Version1;
        public byte Version2;
        public byte Version3;
        public ushort Build;
        public string Platform;
        public string OperatingSystem;
        public string Country;
        public uint TimezoneBias;
        public string Ip;
        public byte IdentifierLength;
        public string Identifier;

        public AuthLogonChallengeClient(byte[] data)
        {
            Command = (AuthCmd) data[0];
            Error = (AuthResult) data[1];
            Size = BitConverter.ToUInt16(data, 2);
            GameName = new string(Encoding.ASCII.GetString(data, 4, 3).Reverse().ToArray());
            Version1 = data[8];
            Version2 = data[9];
            Version3 = data[10];
            Build = BitConverter.ToUInt16(data, 11);
            Platform = new string(Encoding.ASCII.GetString(data, 13, 3).Reverse().ToArray());
            OperatingSystem = new string(Encoding.ASCII.GetString(data, 17, 3).Reverse().ToArray());
            Country = new string(Encoding.ASCII.GetString(data, 21, 4).Reverse().ToArray());
            TimezoneBias = BitConverter.ToUInt32(data, 25);
            Ip = data[29] + "." + data[30] + "." + data[31] + "." + data[32];
            IdentifierLength = data[33];
            Identifier = Encoding.ASCII.GetString(data, 34, IdentifierLength);
        }
    }
}
