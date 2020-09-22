using AivyData.Entities;
using AivyDomain.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace AivyDomain.Mappers.Server
{
    public class ServerEntityMapper : IMapper<Func<ServerEntity, bool>, ServerEntity>
    {
        private readonly List<ServerEntity> _servers;

        public ServerEntityMapper()
        {
            _servers = new List<ServerEntity>();
        }

        public ServerEntity MapFrom(Func<ServerEntity, bool> input)
        {
            if (input is null) throw new ArgumentNullException(nameof(input));

            ServerEntity entity = _servers.FirstOrDefault(input);
            
            if(entity is null)
            {
                entity = new ServerEntity() 
                {
                    Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp),
                    IsRunning = false
                };
                _servers.Add(entity);
            }

            return entity;
        }

        public bool Remove(Func<ServerEntity, bool> predicat)
        {
            return _servers.Remove(_servers.FirstOrDefault(predicat));
        }
    }
}
