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
    public abstract class Server
    {
        private string _listenerIp;
        private TcpListener _listener;

        private Func<string, bool> _clientConnected;
        private Func<string, bool> _clientDisconnected;
        private Func<string, byte[], bool> _messageReceived;

        /// <summary>
        /// Initialize the TCP server.
        /// </summary>
        /// <param name="listenerIp"></param>
        /// <param name="listenerPort"></param>
        /// <param name="clientConnected"></param>
        /// <param name="clientDisconnected"></param>
        /// <param name="messageReceived"></param>
        protected Server(string listenerIp, int listenerPort, Func<string, bool> clientConnected, Func<string, bool> clientDisconnected, 
            Func<string, byte[], bool> messageReceived)
        {
            IPAddress listenerIpAddress;

            if (listenerPort < 1) throw new ArgumentOutOfRangeException(nameof(listenerPort));

            _clientConnected = clientConnected;
            _clientDisconnected = clientDisconnected;
            _messageReceived = messageReceived ?? throw new ArgumentNullException(nameof(messageReceived));

            if (string.IsNullOrEmpty(listenerIp))
            {
                listenerIpAddress = IPAddress.Any;
                _listenerIp = listenerIpAddress.ToString();
            }
            else
            {
                listenerIpAddress = IPAddress.Parse(listenerIp);
                _listenerIp = listenerIp;
            }

            _listener = new TcpListener(listenerIpAddress, listenerPort);
        }

        // TODO: Task AcceptConnections(), Task DataReceiver(), MessageReadAsync(), MessageWriteAsync etc.
    }
}
