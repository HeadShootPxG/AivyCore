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
    [ServerHandler(ProtocolName = "AuthenticationTicketMessage")]
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
            NetworkElement authentication_accepted_message = BotofuProtocolManager.Protocol[ProtocolKeyEnum.Messages, x => x.name == "AuthenticationTicketAcceptedMessage"];
            NetworkElement account_capabilities_message = BotofuProtocolManager.Protocol[ProtocolKeyEnum.Messages, x => x.name == "AccountCapabilitiesMessage"];

            NetworkContentElement account_capabilities_content = new NetworkContentElement()
            {
                fields =
                {
                    { "tutorialAvailable", true },
                    { "canCreateNewCharacter", true },
                    { "accountId", 1 },
                    { "breedsVisible", 262143 },
                    { "breedsAvailable", 262143 },
                    { "status", 0 }
                }
            };

            Send(false, _callback._client, authentication_accepted_message, new NetworkContentElement());
            Send(false, _callback._client, account_capabilities_message, account_capabilities_content);
        }

        public override void Error(Exception e)
        {
            logger.Error(e);
        }
    }
}
