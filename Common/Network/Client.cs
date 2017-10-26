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
using System.Net;
using System.Net.Sockets;

namespace WoWCore.Common.Network
{
    public class Client
    {
        public string Ip { get; set; }
        public int Port { get; set; }

        public TcpClient TcpClient { get; set; }

        public Client(TcpClient tcpClient)
        {
            if (tcpClient == null) throw new ArgumentNullException(nameof(tcpClient));

            Ip = ((IPEndPoint)tcpClient.Client.RemoteEndPoint).Address.ToString();
            Port = ((IPEndPoint)tcpClient.Client.RemoteEndPoint).Port;

            TcpCLient = tcpClient;
        }
    }
}
