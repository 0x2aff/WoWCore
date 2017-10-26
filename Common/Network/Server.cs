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
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using WoWCore.Common.Logging;

namespace WoWCore.Common.Network
{
    public abstract class Server : IDisposable
    {
        private string _listenerIp;
        private TcpListener _listener;

        private CancellationTokenSource _tokenSource;
        private CancellationToken _token;

        private Func<string, bool> _clientConnected;
        private Func<string, bool> _clientDisconnected;
        private Func<string, byte[], bool> _messageReceived;

        private int _activeClients;
        private ConcurrentDictionary<string, Client> _clients;

        /// <summary>
        /// Initialize the TCP server.
        /// </summary>
        /// <param name="listenerIp">The IP address on which the server should listen, nullable</param>
        /// <param name="listenerPort">The TCP port on which the server should listen.</param>
        /// <param name="clientConnected">Function to be called when a client connects.</param>
        /// <param name="clientDisconnected">Function to be called when a client disconnects.</param>
        /// <param name="messageReceived">Function to be called when a message is received.</param>
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

            _tokenSource = new CancellationTokenSource();
            _token = _tokenSource.Token;

            _activeClients = 0;
            _clients = new ConcurrentDictionary<string, Client>();

            Task.Run(AcceptConnections, _token);
        }

        public void Dispose()
        {
            _tokenSource.Cancel();
        }

        private async Task AcceptConnections()
        {
            _listener.Start();

            while (true)
            {
                _token.ThrowIfCancellationRequested();
                var client = await _listener.AcceptTcpClientAsync();
                client.LingerState.Enabled = false;

                await Task.Run(() =>
                {
                    _activeClients++;

                    var currentClient = new Client(client);
                    if (!AddClient(currentClient))
                    {
                        client.Close();
                        return;
                    }

                    var dataReceiverToken = default(CancellationToken);

                    if (_clientConnected != null)
                        Task.Run(() => _clientConnected(currentClient.Ip + ":" + currentClient.Port), new CancellationToken());

                    Task.Run(async () => await DataReceiver(currentClient, dataReceiverToken), dataReceiverToken);
                }, _token);
            }
        }

        private async Task DataReceiver(Client client, CancellationToken? cancellationToken = null)
        {
            try
            {
                while (true)
                {
                    cancellationToken?.ThrowIfCancellationRequested();

                    try
                    {
                        if (!IsConnected(client)) break;

                        var data = await MessageReadAsync(client);
                        if (data == null)
                        {
                            await Task.Delay(30, new CancellationToken());
                            continue;
                        }

                        if (_messageReceived != null)
                            await Task.Run(() => _messageReceived(client.Ip + ":" + client.Port, data), new CancellationToken());
                    }
                    catch (Exception)
                    {
                        break;
                    }
                }
            }
            finally
            {
                _activeClients--;
                if (!RemoveClient(client))
                    LogManager.Instance.Log(LogManager.LogType.Error,
                        "Can't remove client (" + client.Ip + ":" + client.Port + ").");

                if (_clientDisconnected != null)
                    await Task.Run(() => _clientDisconnected(client.Ip + ":" + client.Port), new CancellationToken());
            }
        }

        private async Task<byte[]> MessageReadAsync(Client client)
        {
            throw new NotImplementedException();
        }

        private async Task<bool> MessageWriteAsync(Client client, byte[] data)
        {
            throw new NotImplementedException();
        }

        private static bool IsConnected(Client client)
        {
            if (!client.TcpClient.Connected) return false;

            if (!client.TcpClient.Client.Poll(0, SelectMode.SelectWrite) ||
                client.TcpClient.Client.Poll(0, SelectMode.SelectError)) return false;

            var buffer = new byte[1];
            return client.TcpClient.Client.Receive(buffer, SocketFlags.Peek) != 0;
        }

        private bool AddClient(Client client)
        {
            return _clients.TryAdd(client.Ip + ":" + client.Port, client);
        }

        private bool RemoveClient(Client client)
        {
            return _clients.TryRemove(client.Ip + ":" + client.Port, out client);
        }
    }
}
