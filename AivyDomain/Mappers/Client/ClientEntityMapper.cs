using AivyData.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace AivyDomain.Mappers.Client
{
    public class ClientEntityMapper : IMapper<Func<ClientEntity, bool>, ClientEntity>
    {
        private readonly List<ClientEntity> _clients;

        public ClientEntityMapper()
        {
            _clients = new List<ClientEntity>();
        }

        public ClientEntity MapFrom(Func<ClientEntity, bool> input)
        {
            if (input is null) throw new ArgumentNullException(nameof(input));
            if (input(new ClientEntity()))
            {
                ClientEntity client = new ClientEntity()
                {
                    ReceiveBufferLength = 16384 // to do -> get from file ? 
                };
                _clients.Add(client);
                return client;
            }
            return _clients.FirstOrDefault(input);
        }

        public bool Remove(Func<ClientEntity, bool> predicat)
        {
            return _clients.Remove(_clients.FirstOrDefault(predicat));
        }
    }
}
