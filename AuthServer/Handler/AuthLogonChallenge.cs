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
using System.Security.Cryptography.X509Certificates;
using System.Text;
using WoWCore.Common.Network;

namespace WoWCore.AuthServer.Handler
{
    public sealed class AuthLogonChallenge : PacketReader
    {
        public byte Command { get; }
        public byte Error { get; }
        public ushort Size { get; }
        public string GameName { get; }
        public string Version { get; }
        public ushort Build { get; }
        public string Platform { get; }
        public string OperatingSystem { get; }
        public string Country { get; }
        public uint TimeZoneBias { get; }
        public string IpAddress { get; }
        public byte SRPILength { get; }
        public string SRPI { get; }

        public AuthLogonChallenge(byte[] data) : base(data)
        {
            Command = data[0];
            Error = data[1];
            Size = (ushort) BitConverter.ToInt16(data, 2);
            GameName = Encoding.ASCII.GetString(data, 4, 3);
            Version = data[8] + "." + data[9] + "." + data[10];
            Build = (ushort)BitConverter.ToInt16(data, 11);
            Platform = new string(Encoding.ASCII.GetString(data, 13, 3).Reverse().ToArray());
            OperatingSystem = new string(Encoding.ASCII.GetString(data, 17, 3).Reverse().ToArray());
            Country = new string(Encoding.ASCII.GetString(data, 21, 4).Reverse().ToArray());
            TimeZoneBias = BitConverter.ToUInt32(data, 25);
            IpAddress = data[29] + "." + data[30] + "." + data[31] + "." + data[32];
            SRPILength = data[33];
            SRPI = Encoding.ASCII.GetString(data, 34, SRPILength);
        }
    }
}
