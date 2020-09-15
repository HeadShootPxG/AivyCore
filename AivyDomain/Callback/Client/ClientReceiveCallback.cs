using AivyData.Entities;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace AivyDomain.Callback.Client
{
    public class ClientReceiveCallback : ClientCallback
    {
        static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public byte[] _buffer { get; set; }

        protected Func<MemoryStream, MemoryStream> _rcv_action { get; set; }

        public ClientReceiveCallback(ClientEntity client)
            : base(client)
        {
            _buffer = new byte[client.ReceiveBufferLength];
        }

        public override void Callback(IAsyncResult result)
        {
            _client.Socket = (Socket)result.AsyncState;

            int _rcv_len = _client.Socket.EndReceive(result, out SocketError errorCode);

            if (_rcv_len > 0 && errorCode == SocketError.Success)
            {
                _client.ReceiveBuffer = new MemoryStream();
                _client.ReceiveBuffer.Write(_buffer, 0, _rcv_len);

                _buffer = new byte[_client.ReceiveBufferLength];

                _rcv_action?.Invoke(_client.ReceiveBuffer);

                try
                {
                    _client.Socket.BeginReceive(_buffer,
                                                0,
                                                _buffer.Length,
                                                SocketFlags.None,
                                                Callback,
                                                _client.Socket);
                }
                catch (SocketException e)
                {
                    logger.Error(e);
                }
            }
        }
    }
}
