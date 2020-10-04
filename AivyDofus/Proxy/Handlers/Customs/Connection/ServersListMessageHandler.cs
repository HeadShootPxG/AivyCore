using AivyData.API;
using AivyData.API.Proxy;
using AivyData.Entities;
using AivyData.Enums;
using AivyDofus.Handler;
using AivyDofus.Protocol.Buffer;
using AivyDofus.Protocol.Elements;
using AivyDofus.Proxy.Callbacks;
using AivyDomain.Callback.Client;
using AivyDomain.UseCases.Client;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AivyDofus.Proxy.Handlers.Customs.Connection
{
    [ProxyHandler(ProtocolName = "ServersListMessage")]
    public class ServersListMessageHandler : AbstractMessageHandler
    {
        static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public override bool IsForwardingData => false;

        public ServersListMessageHandler(AbstractClientReceiveCallback callback, 
                                         NetworkElement element,
                                         NetworkContentElement content)
            : base(callback, element, content)
        {

        }

        public override void Handle()
        {
            IEnumerable<dynamic> _servers = _content["servers"];

            foreach (ProxyCustomServerData custom in DofusProxy._proxy_api.GetData(null).custom_servers)
            {
                int status = 0;

                if (custom.Ports.Length > 0)
                {
                    // to do , try connect client to check server
                    status = 3;
                }

                _servers = _servers.Append(new NetworkContentElement()
                {
                    fields =
                    {
                        { "isMonoAccount", custom.IsMonoAccount },
                        { "isSelectable", true },
                        { "id", custom.ServerId },
                        { "type", custom.Type },
                        { "status", status },
                        { "completion", 0 },
                        { "charactersCount", 1 },
                        { "charactersSlots", 1 },
                        { "date", 1234828800000 }
                    }
                });

            }

            _content["servers"] = _servers.ToArray();

            Send(false, _callback._remote, _element, _content);
        }
    }
}
