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
    public class DofusProxy 
    {
        public static readonly OpenProxyConfigurationApi _proxy_api = new OpenProxyConfigurationApi("./proxy_information_api.json");
        public static uint GLOBAL_INSTANCE_ID => LAST_CLIENT_INSTANCE_ID + SERVER_MSG_RCV_SINCE_CLIENT + FAKE_MESSAGE_SENT;
        public static uint LAST_CLIENT_INSTANCE_ID = 0;
        public static uint SERVER_MSG_RCV_SINCE_CLIENT = 0;
        public static uint FAKE_MESSAGE_SENT = 0;
        readonly ProxyEntityMapper _proxy_mapper;
        readonly ProxyRepository _proxy_repository;

        readonly ProxyCreatorRequest _proxy_creator;
        readonly ProxyActivatorRequest _proxy_activator;

        ProxyEntity _proxy;

        readonly string _app_path;
        string _exe_path => $"{_app_path}/Dofus.exe";
        string _invoker_path => $"{_app_path}/DofusInvoker.swf";

        public DofusProxy(string appPath, int port)
        {
            _app_path = appPath ?? throw new ArgumentNullException(nameof(appPath));

            _proxy_mapper = new ProxyEntityMapper();
            _proxy_repository = new ProxyRepository(_proxy_api, _proxy_mapper);

            _proxy_creator = new ProxyCreatorRequest(_proxy_repository);
            _proxy_activator = new ProxyActivatorRequest(_proxy_repository);

            _proxy = _proxy_creator.Handle(_exe_path, port);

            if (!StaticValues.DOFUS_PROTOCOL_INITIED)
                new BotofuParser(_invoker_path).Parse();
        }

        public void Active(bool active)
        {
            _proxy = _proxy_activator.Handle(_proxy, active, new DofusProxyAcceptCallback(_proxy));
        }
    }
}
