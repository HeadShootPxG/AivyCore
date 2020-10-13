using AivyData.Entities;
using AivyDofus.Protocol.Parser;
using AivyDofus.Proxy.API;
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
    public class DofusRetroProxy
    {
        // just for test
        public static readonly OpenProxyApi _proxy_api = new OpenProxyApi("./retro_proxy_information_api.json");

        public readonly ProxyRepository _proxy_repository;

        protected readonly ProxyEntityMapper _proxy_mapper = new ProxyEntityMapper();

        protected readonly ProxyCreatorRequest _proxy_creator;
        protected readonly ProxyActivatorRequest _proxy_activator;

        protected readonly string _app_path;
        public string _exe_path => $"{_app_path}/Dofus.exe";

        public DofusRetroProxy(string appPath)
        {
            _app_path = appPath ?? throw new ArgumentNullException(nameof(appPath));

            if (_proxy_repository is null)
            {
                _proxy_repository = new ProxyRepository(_proxy_api, _proxy_mapper);
                _proxy_creator = new ProxyCreatorRequest(_proxy_repository);
                _proxy_activator = new ProxyActivatorRequest(_proxy_repository);
            }
        }

        public virtual ProxyEntity Active(bool active, int port)
        {
            if (active)
            {
                ProxyEntity result = _proxy_creator.Handle(_exe_path, port);
                result = _proxy_activator.Handle(result, active, new DofusRetroProxyAcceptCallback(result));
                return result;
            }
            else
            {
                if (_proxy_repository.Remove(x => x.Port == port))
                {
                    return null;
                }
                throw new ArgumentNullException($"cannot disable proxy with port : {port}");
            }
        }
    }

    public class DofusProxy 
    { 
        public static readonly OpenProxyConfigurationApi _proxy_api = new OpenProxyConfigurationApi("./proxy_information_api.json");

        public readonly ProxyRepository _proxy_repository;

        protected readonly ProxyEntityMapper _proxy_mapper = new ProxyEntityMapper();

        protected readonly ProxyCreatorRequest _proxy_creator;
        protected readonly ProxyActivatorRequest _proxy_activator;

        protected readonly string _app_path;
        protected string _exe_path => $"{_app_path}/Dofus.exe";
        protected virtual string _invoker_path => $"{_app_path}/DofusInvoker.swf";

        public DofusProxy(string appPath)
        {
            _app_path = appPath ?? throw new ArgumentNullException(nameof(appPath));

            if (_proxy_repository is null)
            {
                _proxy_repository = new ProxyRepository(_proxy_api, _proxy_mapper);
                _proxy_creator = new ProxyCreatorRequest(_proxy_repository);
                _proxy_activator = new ProxyActivatorRequest(_proxy_repository);
            }
                
            new BotofuParser(_invoker_path).Parse();
        }

        public virtual ProxyEntity Active(bool active, int port)
        {
            if (active)
            {
                ProxyEntity result = _proxy_creator.Handle(_exe_path, port);
                result = _proxy_activator.Handle(result, active, new DofusProxyAcceptCallback(result));
                return result;
            }
            else
            {
                if (_proxy_repository.Remove(x => x.Port == port))
                {
                    return null;
                }
                throw new ArgumentNullException($"cannot disable proxy with port : {port}");
            }
        }
    }
}
