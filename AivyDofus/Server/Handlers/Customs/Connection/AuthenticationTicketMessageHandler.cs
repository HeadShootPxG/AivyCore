using AivyDofus.Handler;
using AivyDofus.Protocol.Elements;
using AivyDomain.Callback.Client;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AivyDofus.Server.Handlers.Customs.Connection
{
    [ServerHandler(ProtocolId = 110)]
    public class AuthenticationTicketMessageHandler : AbstractMessageHandler
    {
        static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public override bool IsForwardingData => true;

        public AuthenticationTicketMessageHandler(AbstractClientReceiveCallback callback,
                                                  NetworkElement element,
                                                  NetworkContentElement content)
            : base(callback, element, content)
        {

        }

        public override void Handle()
        {
            NetworkElement _authentication_accepted_message = BotofuProtocolManager.Protocol[ProtocolKeyEnum.Messages, x => x.protocolID == 111];

            Send(false, _callback._client, _authentication_accepted_message, new NetworkContentElement());
        }

        public override void Error(Exception e)
        {
            logger.Error(e);
        }
    }
}
