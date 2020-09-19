using AivyData.Entities;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace AivyDomain.Callback.Client
{
    public class ClientConnectCallback : ClientCallback
    {
        private readonly ClientReceiveCallback _receiveCallback;

        public ClientConnectCallback(ClientEntity client, ClientReceiveCallback receiveCallback)
            : base(client)
        {
            _receiveCallback = receiveCallback ?? throw new ArgumentNullException(nameof(receiveCallback));
        }

        public ClientConnectCallback(ClientEntity client)
            : this(client, new ClientReceiveCallback(client))
        {

        }

        public override void Callback(IAsyncResult result)
        {
            if (_client.IsRunning)
            {
                _client.Socket.BeginReceive(_receiveCallback._buffer,
                                            0,
                                            _receiveCallback._buffer.Length,
                                            SocketFlags.None,
                                            _receiveCallback.Callback,
                                            _client.Socket);
            }
        }
    }
}
