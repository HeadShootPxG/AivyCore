using AivyData.API;
using System;
using System.Collections.Generic;
using System.Text;

namespace AivyDomain.API.Proxy
{
    public class OpenProxyApi : IApi<ProxyData>
    { 
        private readonly string _location;

        public OpenProxyApi(string location)
        {
            _location = location ?? throw new ArgumentNullException(nameof(location));
        }

        public ProxyData GetData(Func<ProxyData, bool> predicat)
        {
            throw new NotImplementedException();
        }

        public ProxyData UpdateData(ProxyData data)
        {
            throw new NotImplementedException();
        }
    }
}
