using AivyData.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace AivyDomain.Callback.Client
{
    public abstract class ClientCallback : IAsyncCallback
    {
        public readonly ClientEntity _client;

        public ClientCallback(ClientEntity client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public abstract void Callback(IAsyncResult result);
    }
}
