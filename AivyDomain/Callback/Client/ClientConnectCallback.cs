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

        public ClientConnectCallback(ClientEntity client)
            : base(client)
        {
            _receiveCallback = new ClientReceiveCallback(client);
        }

        public override void Callback(IAsyncResult result)
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
