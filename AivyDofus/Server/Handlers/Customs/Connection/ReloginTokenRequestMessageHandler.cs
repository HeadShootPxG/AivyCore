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
    [ServerHandler(ProtocolId = 6540)]
    public class ReloginTokenRequestMessageHandler : AbstractMessageHandler
    {
        public override bool IsForwardingData => true;

        public ReloginTokenRequestMessageHandler(AbstractClientReceiveCallback callback,
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
            /*NetworkElement relogin_status_message = BotofuProtocolManager.Protocol[ProtocolKeyEnum.Messages, x => x.protocolID == 6539];
            NetworkContentElement relogin_status_content = new NetworkContentElement()
            {
                fields =
                {
                    { "validToken", true },
                    { "ticket", Encode(Random()) }
                }
            };

            Send(false, _callback._client, relogin_status_message, relogin_status_content);*/
        }
    }
}
