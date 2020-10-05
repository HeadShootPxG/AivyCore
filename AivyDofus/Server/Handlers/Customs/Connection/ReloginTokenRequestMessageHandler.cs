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
    [ServerHandler(ProtocolName = "ReloginTokenRequestMessage")]
    public class ReloginTokenRequestMessageHandler : AbstractMessageHandler
    {
        public override bool IsForwardingData => true;

        public ReloginTokenRequestMessageHandler(AbstractClientReceiveCallback callback,
                                                  NetworkElement element,
                                                  NetworkContentElement content)
            : base(callback, element, content)
        {

        }

        public override void Handle()
        {

        }
    }
}
