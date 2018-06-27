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
    /// <inheritdoc />
    /// <summary>
    ///     Defines the TCP server.
    /// </summary>
    public sealed class Server : IDisposable
    {
        private readonly Func<string, int, bool> _clientConnected;
        private readonly Func<string, int, bool> _clientDisconnected;

        private readonly ConcurrentDictionary<string, Client> _clients;

        private readonly TcpListener _listener;
        private readonly Func<string, int, byte[], bool> _messageReceived;
        private readonly CancellationToken _token;

        private readonly CancellationTokenSource _tokenSource;

        /// <summary>
        ///     Instantiates the server.
        /// </summary>
        /// <param name="listenerIp">The IP address on which the server should listen, nullable.</param>
        /// <param name="listenerPort">The port on which the server should listen.</param>
        /// <param name="clientConnected">Function to be called when a client connects.</param>
        /// <param name="clientDisconnected">Function to be called when a client disconnects.</param>
        /// <param name="messageReceived">Function to be called when a message is received.</param>
        public Server(string listenerIp, int listenerPort, Func<string, int, bool> clientConnected,
            Func<string, int, bool> clientDisconnected, Func<string, int, byte[], bool> messageReceived)
        {
            string listenerIp1;
            IPAddress listenerAddress;
            _clientConnected = clientConnected;
            _clientDisconnected = clientDisconnected;
            _messageReceived = messageReceived ?? throw new ArgumentNullException(nameof(messageReceived));

            if (string.IsNullOrEmpty(listenerIp))
            {
                listenerAddress = IPAddress.Any;
                listenerIp1 = listenerAddress.ToString();
            }
            else
            {
                listenerAddress = IPAddress.Parse(listenerIp);
                listenerIp1 = listenerIp;
            }

            if (listenerPort < 1) throw new ArgumentOutOfRangeException(nameof(listenerPort));
            var listenerPort1 = listenerPort;

            LogManager.Instance.Log(LogManager.LogType.Info, $"Server starting on {listenerIp1}:{listenerPort1}.");

            _listener = new TcpListener(listenerAddress, listenerPort1);
            _tokenSource = new CancellationTokenSource();
            _token = _tokenSource.Token;

            _clients = new ConcurrentDictionary<string, Client>();

            Task.Run(AcceptConnections, _token).ConfigureAwait(false);
        }

        public void Dispose()
        {
            _tokenSource?.Dispose();
        }

        /// <summary>
        ///     Send data to the specified client, asynchronously.
        /// </summary>
        /// <param name="ip">The IP address of the client.</param>
        /// <param name="port">The port of the client.</param>
        /// <param name="data">Byte array containing data.</param>
        /// <returns>Boolean indicating if the message was sent successfully.</returns>
        public async Task<bool> SendAsync(string ip, int port, byte[] data)
        {
            if (_clients.TryGetValue(new Guid(ip + ":" + port).ToString(), out var client))
                return await SendAsync(client, data).ConfigureAwait(false);

            return false;
        }

        /// <summary>
        ///     Send data to the specified client, asynchronously.
        /// </summary>
        /// <param name="client">The <see cref="Client" />.</param>
        /// <param name="data">Byte array containing data.</param>
        /// <returns>Boolean indicating if the message was sent successfully.</returns>
        public async Task<bool> SendAsync(Client client, byte[] data)
        {
            return await MessageWriteAsync(client, data).ConfigureAwait(false);
        }

        private async Task AcceptConnections()
        {
            _listener.Start();

            while (!_token.IsCancellationRequested)
                try
                {
                    var tcpClient = await _listener.AcceptTcpClientAsync();
                    tcpClient.LingerState.Enabled = false;

                    var client = new Client(tcpClient);

                    if (!AddClient(client))
                    {
                        client.Dispose();
                        return;
                    }

                    if (_clientConnected != null)
                        await Task.Run(() => _clientConnected(client.Ip, client.Port), _token).ConfigureAwait(false);

                    await Task.Run(() => DataReceiver(client), _token).ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    LogManager.Instance.Log(LogManager.LogType.Error, e.Message);
                }
        }

        private async Task DataReceiver(Client client)
        {
            try
            {
                while (!_token.IsCancellationRequested)
                    try
                    {
                        if (!IsConnected(client)) break;

                        var data = await MessageReadAsync(client);
                        if (data == null)
                        {
                            await Task.Delay(30, _token);
                            continue;
                        }

                        if (_messageReceived != null)
                            await Task.Run(() => _messageReceived(client.Ip, client.Port, data), _token)
                                .ConfigureAwait(false);
                    }
                    catch (Exception e)
                    {
                        LogManager.Instance.Log(LogManager.LogType.Error, e.Message);
                    }
            }
            finally
            {
                if (!RemoveClient(client))
                    LogManager.Instance.Log(LogManager.LogType.Error,
                        $"Can't remove client ({client.Ip}:{client.Port}).");

                if (_clientDisconnected != null)
                    await Task.Run(() => _clientDisconnected(client.Ip, client.Port), _token).ConfigureAwait(false);
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
                } while (clientStream.DataAvailable);

                return memoryStream.ToArray();
            }
        }

        private async Task<bool> MessageWriteAsync(Client client, byte[] data)
        {
            if (data == null || data.Length < 1)
                return false;

            var clientStream = client.TcpClient.GetStream();
            await clientStream.WriteAsync(data, 0, data.Length, _token).ConfigureAwait(false);
            await clientStream.FlushAsync(_token).ConfigureAwait(false);

            return true;
        }

        private bool AddClient(Client client)
        {
            return _clients.TryAdd(new Guid(client.Ip + ":" + client.Port).ToString(), client);
        }

        private bool RemoveClient(Client client)
        {
            return _clients.TryRemove(new Guid(client.Ip + ":" + client.Port).ToString(), out client);
        }

        private static bool IsConnected(Client client)
        {
            if (!client.TcpClient.Connected) return false;

            if (!client.TcpClient.Client.Poll(0, SelectMode.SelectWrite) ||
                client.TcpClient.Client.Poll(0, SelectMode.SelectError)) return false;

            return client.TcpClient.Client.Receive(new byte[1], SocketFlags.Peek) != 0;
        }
    }
}