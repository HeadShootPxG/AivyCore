using AivyDofus.Handler;
using AivyDofus.Protocol.Elements;
using AivyDomain.Callback.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AivyDofus.Server.Handlers.Customs.Connection
{
    [ServerHandler(ProtocolId = 150)]
    public class CharactersListRequestMessageHandler : AbstractMessageHandler
    {
        public override bool IsForwardingData => true;

        public CharactersListRequestMessageHandler(AbstractClientReceiveCallback callback,
                                                   NetworkElement element,
                                                   NetworkContentElement content)
            : base(callback, element, content)
        {

        }

        public override void Handle()
        {
            NetworkElement characters_list_message = BotofuProtocolManager.Protocol[ProtocolKeyEnum.Messages, x => x.protocolID == 151];
            NetworkContentElement characters_list_content = new NetworkContentElement()
            {
                fields =
                {
                    { "hasStartupActions", false },
                    { "characters", new NetworkContentElement[0] }
                }
            };

            Send(false, _callback._client, characters_list_message, characters_list_content);
        }
    }
}
