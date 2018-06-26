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
using System.Collections.Concurrent;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using WoWCore.Common.Logging;

namespace WoWCore.Common.Network
{
    public sealed class Server : IDisposable
    {
        private readonly TcpListener _listener;

        private readonly CancellationTokenSource _tokenSource;
        private readonly CancellationToken _token;

        private readonly Func<string, bool> _clientConnected;
        private readonly Func<string, bool> _clientDisconnected;
        private readonly Func<string, byte[], bool> _messageReceived;

        public int ActiveClients { get; private set; }
        private readonly ConcurrentDictionary<string, Client> _clients;

        /// <summary>
        /// Initialize the TCP server.
        /// </summary>
        /// <param name="listenerIp">The IP address on which the server should listen, nullable</param>
        /// <param name="listenerPort">The TCP port on which the server should listen.</param>
        /// <param name="clientConnected">Function to be called when a client connects.</param>
        /// <param name="clientDisconnected">Function to be called when a client disconnects.</param>
        /// <param name="messageReceived">Function to be called when a message is received.</param>
        public Server(string listenerIp, int listenerPort, Func<string, bool> clientConnected, Func<string, bool> clientDisconnected, 
            Func<string, byte[], bool> messageReceived)
        {
            if (listenerPort < 1) throw new ArgumentOutOfRangeException(nameof(listenerPort));

            _clientConnected = clientConnected;
            _clientDisconnected = clientDisconnected;
            _messageReceived = messageReceived ?? throw new ArgumentNullException(nameof(messageReceived));

            var listenerIpAddress = string.IsNullOrEmpty(listenerIp) ? IPAddress.Any : IPAddress.Parse(listenerIp);

            _listener = new TcpListener(listenerIpAddress, listenerPort);

            _tokenSource = new CancellationTokenSource();
            _token = _tokenSource.Token;

            ActiveClients = 0;
            _clients = new ConcurrentDictionary<string, Client>();

            Task.Run(AcceptConnections, _token);
        }

        /// <summary>
        /// Send data to the specified client, asynchronously.
        /// </summary>
        /// <param name="ipPort">IP:Port of the recipient client.</param>
        /// <param name="data">Byte array containing data.</param>
        /// <returns>Task with Boolean indicating if the message was sent successfully.</returns>
        public async Task<bool> SendAsync(string ipPort, byte[] data)
        {
            if (_clients.TryGetValue(ipPort, out var client)) return await MessageWriteAsync(client, data);

            LogManager.Instance.Log(LogManager.LogType.Error, "Unable to send message to client (" + ipPort + ").");
            return false;
        }

        public void Dispose()
        {
            _tokenSource.Cancel();
        }

        private async Task AcceptConnections()
        {
            _listener.Start();

            while (!_token.IsCancellationRequested)
            {
                var client = await _listener.AcceptTcpClientAsync();
                client.LingerState.Enabled = false;

                await Task.Run(() =>
                {
                    ActiveClients++;

                    var currentClient = new Client(client);
                    if (!AddClient(currentClient))
                    {
                        client.Close();
                        return;
                    }

                    var dataReceiverToken = default(CancellationToken);

                    if (_clientConnected != null)
                        Task.Run(() => _clientConnected(currentClient.Ip + ":" + currentClient.Port), _token);

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
                            await Task.Delay(30, _token);
                            continue;
                        }

                        if (_messageReceived != null) { }
                            await Task.Run(() => _messageReceived(client.Ip + ":" + client.Port, data), _token);
                    }
                    catch (Exception)
                    {
                        break;
                    }
                }
            }
            finally
            {
                ActiveClients--;
                if (!RemoveClient(client))
                    LogManager.Instance.Log(LogManager.LogType.Error,
                        "Can't remove client (" + client.Ip + ":" + client.Port + ").");

                if (_clientDisconnected != null)
                    await Task.Run(() => _clientDisconnected(client.Ip + ":" + client.Port), _token);
            }
        }

        private async Task<byte[]> MessageReadAsync(Client client)
        {
            var clientStream = client.TcpClient.GetStream();

            if (!clientStream.CanRead || !clientStream.DataAvailable) return null;

            var readBuffer = new byte[client.TcpClient.ReceiveBufferSize];
            using (var memoryStream = new MemoryStream())
            {
                do
                {
                    var numBytesRead = await clientStream.ReadAsync(readBuffer, 0, readBuffer.Length, _token);
                    if (numBytesRead <= 0)
                        break;

                    await memoryStream.WriteAsync(readBuffer, 0, numBytesRead, _token);
                }
                while (clientStream.DataAvailable);

                return memoryStream.ToArray();
            }
        }

        private async Task<bool> MessageWriteAsync(Client client, byte[] data)
        {
            if (data == null || data.Length < 1)
                return false;

            var clientStream = client.TcpClient.GetStream();
            await clientStream.WriteAsync(data, 0, data.Length, _token);
            await clientStream.FlushAsync(_token);

            return true;
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
