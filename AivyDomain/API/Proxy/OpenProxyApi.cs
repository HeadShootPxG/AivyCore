using AivyData.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace AivyDomain.API.Proxy
{
    public class OpenProxyApi : IApi<ProxyEntity>
    { 
        private readonly string _location;

        public OpenProxyApi(string location)
        {
            _location = location ?? throw new ArgumentNullException(nameof(location));
        }

        public ProxyEntity GetData(Func<ProxyEntity, bool> predicat)
        {
            throw new NotImplementedException();
        }
    }
}
