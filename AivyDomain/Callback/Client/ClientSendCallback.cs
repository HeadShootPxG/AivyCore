using AivyData.Entities;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace AivyDomain.Callback.Client
{
    public class ClientSendCallback : ClientCallback
    {
        public ClientSendCallback(ClientEntity client)
            : base(client)
        {
            
        }

        public override void Callback(IAsyncResult result)
        {
            _client.Socket = (Socket)result.AsyncState;
        }
    }
}
