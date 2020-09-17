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
    [ProxyHandler(ProtocolId = 40)]
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
            if (_content["serverId"] == 671)
            {
                NetworkElement element = BotofuProtocolManager.Protocol[ProtocolKeyEnum.Messages, x => x.protocolID == 42];

                logger.Info("request connection in custom server");

                Send(false, _callback._client, element, new NetworkContentElement() 
                {
                    fields = 
                    {
                        { "serverId", 671 },
                        { "address", "127.0.0.1" },
                        { "ports", new short[] { 777 } },
                        { "canCreateNewCharacter", true },
                        { "ticket", Encode(Random()) }
                    }
                });

                _callback._client_disconnector.Handle(_callback._client);
            }
            else
            {
                logger.Info($"iid {_callback._instance_id}");
                Send(true, _callback._remote, _element, _content, _callback._instance_id);
            }
        }

        public override void Error(Exception e)
        {
            logger.Error(e);
        }
    }
}
