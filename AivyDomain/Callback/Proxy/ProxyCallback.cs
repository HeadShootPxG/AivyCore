using AivyData.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace AivyDomain.Callback.Proxy
{
    public abstract class ProxyCallback : IAsyncCallback
    {
        protected readonly ProxyEntity _proxy;

        public ProxyCallback(ProxyEntity proxy)
        {
            _proxy = proxy ?? throw new ArgumentNullException(nameof(proxy));
        }

        public abstract void Callback(IAsyncResult result);
    }
}
