using AivyData.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace AivyDomain.API.Client
{
    public class OpenClientApi : IApi<ClientEntity>
    {
        private readonly string _location;

        public OpenClientApi(string location)
        {
            _location = location ?? throw new ArgumentNullException(nameof(location));
        }

        public ClientEntity GetData(Func<ClientEntity, bool> predicat)
        {
            throw new NotImplementedException();
        }
    }
}
