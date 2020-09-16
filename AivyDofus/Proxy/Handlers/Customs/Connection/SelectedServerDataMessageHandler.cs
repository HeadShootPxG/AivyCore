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
    [ProxyHandler(ProtocolId = 42)]
    public class SelectedServerDataMessageHandler : AbstractMessageHandler
    {
        static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public override bool IsForwardingData => true;

        public SelectedServerDataMessageHandler(ProxyClientReceiveCallback callback,
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
