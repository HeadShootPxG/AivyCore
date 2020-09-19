using AivyData.API;
using AivyData.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace AivyDomain.API.Client
{
    public class OpenClientApi : IApi<ClientData>
    {
        private readonly string _location;

        public OpenClientApi(string location)
        {
            _location = location ?? throw new ArgumentNullException(nameof(location));
        }

        public ClientData GetData(Func<ClientData, bool> predicat)
        {
            throw new NotImplementedException();
        }

        public ClientData UpdateData(ClientData data)
        {
            throw new NotImplementedException();
        }
    }
}
