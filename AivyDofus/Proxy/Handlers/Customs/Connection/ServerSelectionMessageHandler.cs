using AivyData.API;
using AivyData.API.Proxy;
using AivyDofus.Handler;
using AivyDofus.Protocol.Elements;
using AivyDomain.Callback.Client;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AivyDofus.Proxy.Handlers.Customs.Connection
{
    [ProxyHandler(ProtocolName = "ServerSelectionMessage")]
    public class ServerSelectionMessageHandler : AbstractMessageHandler
    {
        static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public override bool IsForwardingData => false;

        public ServerSelectionMessageHandler(AbstractClientReceiveCallback callback,
                                            NetworkElement element,
                                            NetworkContentElement content)
            : base(callback, element, content)
        {

        }

        public static string Random()
        {
            return Guid.NewGuid().ToString("N");
        }

        public static byte[] Encode(string ticket)
        {
            return Encoding.ASCII.GetBytes(ticket);
        }

        public override void Handle()
        {
            if (DofusProxy._proxy_api.GetData(null).custom_servers.FirstOrDefault(x => x.ServerId == (int)_content["serverId"]) is ProxyCustomServerData _data)
            {
                logger.Info($"data:{_data.ServerId} - content:{_content["serverId"]}");
                NetworkElement element = BotofuProtocolManager.Protocol[ProtocolKeyEnum.Messages, x => x.protocolID == 42];

                logger.Info("request connection in custom server");

                Send(false, _callback._client, element, new NetworkContentElement() 
                {
                    fields = 
                    {
                        { "serverId", _data.ServerId },
                        { "address", _data.IpAddress },
                        { "ports", _data.Ports },
                        { "canCreateNewCharacter", true },
                        { "ticket", Encode(Random()) }
                    }
                });

                _callback._client_disconnector.Handle(_callback._client);
            }
            else
            {
                Send(true, _callback._remote, _element, _content, _callback._instance_id);
            }
        }

        public override void Error(Exception e)
        {
            logger.Error(e);
        }
    }
}
