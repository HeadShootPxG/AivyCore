using AivyData.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace AivyDomain.API.Server
{
    public class OpenServerApi : IApi<ServerEntity>
    {
        private readonly string _location;

        public OpenServerApi(string location)
        {
            _location = location ?? throw new ArgumentNullException(nameof(location));
        }

        public ServerEntity GetData(Func<ServerEntity, bool> predicat)
        {
            throw new NotImplementedException();
        }
    }
}
