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
    [ProxyHandler(ProtocolName = "ProtocolRequired")]
    public class ProtocolRequiredHandler : AbstractMessageHandler
    {
        static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public ProtocolRequiredHandler(AbstractClientReceiveCallback callback, NetworkElement element, NetworkContentElement content) 
            : base(callback, element, content)
        {
        }

        public override bool IsForwardingData => true;

        public override void Handle()
        {
            logger.Info($"version : {_content["version"]}");
        }
    }
}
