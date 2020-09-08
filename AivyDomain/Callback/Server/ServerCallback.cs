using AivyData.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace AivyDomain.Callback.Server
{
    public abstract class ServerCallback : IAsyncCallback
    {
        protected readonly ServerEntity _server;

        public ServerCallback(ServerEntity server)
        {
            _server = server ?? throw new ArgumentNullException(nameof(server));
        }

        public abstract void Callback(IAsyncResult result);
    }
}
