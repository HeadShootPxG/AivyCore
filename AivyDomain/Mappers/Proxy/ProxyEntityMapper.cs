using AivyData.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace AivyDomain.Mappers.Proxy
{
    public class ProxyEntityMapper : IMapper<Func<ProxyEntity, bool>, ProxyEntity>
    {
        private readonly List<ProxyEntity> _proxys;

        public ProxyEntityMapper()
        {
            _proxys = new List<ProxyEntity>();
        }

        public ProxyEntity MapFrom(Func<ProxyEntity, bool> input)
        {
            if (input is null)
                throw new ArgumentNullException(nameof(input));

            ProxyEntity proxy = _proxys.FirstOrDefault(input);

            if(proxy is null)
            {
                proxy = new ProxyEntity()
                {
                    Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp),
                    IsRunning = false,
                    HookInterface = new HookInterfaceEntity(),
                    IpRedirectedStack = new Queue<IPEndPoint>()
                };
                _proxys.Add(proxy);
            }

            return proxy;
        }

        public bool Remove(Func<ProxyEntity, bool> predicat)
        {
            return _proxys.Remove(_proxys.FirstOrDefault(predicat));
        }
    }
}
