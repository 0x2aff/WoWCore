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
using System.Net;
using System.Net.Sockets;

namespace WoWCore.Common.Network
{
    /// <inheritdoc />
    /// <summary>
    ///     Defines the client.
    /// </summary>
    public class Client : IDisposable
    {
        /// <summary>
        ///     Instantiates the <see cref="Client" /> class.
        /// </summary>
        /// <param name="tcpClient">Client connection for the TCP network service.</param>
        public Client(TcpClient tcpClient)
        {
            if (tcpClient == null) throw new ArgumentNullException(nameof(tcpClient));

            Ip = ((IPEndPoint) tcpClient.Client.RemoteEndPoint).Address.ToString();
            Port = ((IPEndPoint) tcpClient.Client.RemoteEndPoint).Port;

            TcpClient = tcpClient;
        }

        /// <summary>
        ///     IP address of the client.
        /// </summary>
        public string Ip { get; }

        /// <summary>
        ///     Port of the client.
        /// </summary>
        public int Port { get; }

        /// <summary>
        ///     Client connection for the TCP network service.
        /// </summary>
        public TcpClient TcpClient { get; }

        /// <inheritdoc />
        /// <summary>
        ///     Releases the managed and unmanaged resources used by the <see cref="T:WoWCore.Common.Network.Client" />.
        /// </summary>
        public void Dispose()
        {
            TcpClient?.Dispose();
        }
    }
}