using AivyData.Entities;
using AivyDofus.Proxy.Callbacks;
using AivyDomain.API.Proxy;
using AivyDomain.Mappers.Proxy;
using AivyDomain.Repository.Proxy;
using AivyDomain.UseCases.Proxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AivyDofus.Proxy
{
    public class DofusProxy 
    {
        readonly OpenProxyApi _proxy_api;
        readonly ProxyEntityMapper _proxy_mapper;
        readonly ProxyRepository _proxy_repository;

        readonly ProxyCreatorRequest _proxy_creator;
        readonly ProxyActivatorRequest _proxy_activator;

        ProxyEntity _proxy;

        public DofusProxy(string exePath, int port)
        {
            _proxy_api = new OpenProxyApi("./proxy_information_api.json");
            _proxy_mapper = new ProxyEntityMapper();
            _proxy_repository = new ProxyRepository(_proxy_api, _proxy_mapper);

            _proxy_creator = new ProxyCreatorRequest(_proxy_repository);
            _proxy_activator = new ProxyActivatorRequest(_proxy_repository);

            _proxy = _proxy_creator.Handle(exePath, port);
        }

        public void Active(bool active)
        {
            _proxy = _proxy_activator.Handle(_proxy, active, new DofusProxyAcceptCallback(_proxy));
        }
    }
}
