using AivyData.Entities;
using AivyData.Enums;
using AivyDofus.Protocol.Elements;
using AivyDofus.Protocol.Parser;
using AivyDofus.Proxy.API;
using AivyDofus.Proxy.Callbacks;
using AivyDomain.Callback.Client;
using AivyDomain.Callback.Proxy;
using AivyDomain.Mappers.Proxy;
using AivyDomain.Repository.Proxy;
using AivyDomain.UseCases.Proxy;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AivyDofus.Proxy
{
    public class DofusMultiProxy
    {
        static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public static readonly OpenProxyConfigurationApi _proxy_api = new OpenProxyConfigurationApi("./proxy_information_api.json");

        public readonly ProxyRepository _proxy_repository;

        protected readonly ProxyEntityMapper _proxy_mapper = new ProxyEntityMapper();

        protected readonly ProxyCreatorRequest _proxy_creator;
        protected readonly ProxyActivatorRequest _proxy_activator;

        public DofusMultiProxy()
        {
            if(_proxy_repository is null)
            {
                _proxy_repository = new ProxyRepository(_proxy_api, _proxy_mapper);
                _proxy_creator = new ProxyCreatorRequest(_proxy_repository);
                _proxy_activator = new ProxyActivatorRequest(_proxy_repository);
            }
        }        

        public ProxyEntity Active<Callback>(bool active, int port, string folder_path) where Callback : ProxyAcceptCallback
        {
            if (active)
            {
                ProxyEntity result = _proxy_creator.Handle(folder_path, port);
                result = _proxy_activator.Handle(result, active, Activator.CreateInstance(typeof(Callback), new object[] { result }) as Callback);
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

        private bool protocol2_parsed { get; set; } = false;
        public ProxyEntity Active(ProxyCallbackTypeEnum type, bool active, int port, string folder_path, string exe_name)
        {
            if (type == ProxyCallbackTypeEnum.Dofus2 && !protocol2_parsed)
            {
                protocol2_parsed = true;
                string _invoker_path = $"{folder_path}/DofusInvoker.swf";
                new BotofuParser(_invoker_path).Parse();
            }

            string exe_path = Path.Combine(folder_path, $"{exe_name}.exe");
            switch (type)
            {
                case ProxyCallbackTypeEnum.Dofus2:return Active<DofusProxyAcceptCallback>(active, port, exe_path);
                case ProxyCallbackTypeEnum.DofusRetro: return Active<DofusRetroProxyAcceptCallback>(active, port, exe_path);
                default: logger.Info("default proxy callback was called"); return Active<ProxyAcceptCallback>(active, port, exe_path);
            }
        }
    }
}
