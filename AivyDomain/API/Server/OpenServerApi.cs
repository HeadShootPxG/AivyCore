using AivyData.API;
using AivyData.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace AivyDomain.API.Server
{
    public class OpenServerApi : IApi<ServerData>
    {
        private readonly string _location;

        public OpenServerApi(string location)
        {
            _location = location ?? throw new ArgumentNullException(nameof(location));
        }

        public ServerData GetData(Func<ServerData, bool> predicat)
        {
            throw new NotImplementedException();
        }

        public ServerData UpdateData(ServerData data)
        {
            throw new NotImplementedException();
        }
    }
}
