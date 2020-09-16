using AivyData.Entities;
using AivyData.Enums;
using AivyDofus.Handler;
using AivyDofus.Protocol.Elements;
using AivyDomain.Callback.Client;
using AivyDomain.UseCases.Client;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AivyDofus.Proxy.Handlers.Customs.Connection
{
    [ProxyHandler(ProtocolId = 4)]
    public class IdentificationMessageHandler : AbstractMessageHandler
    {
        static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public override bool IsForwardingData => true;

        public IdentificationMessageHandler(ProxyClientReceiveCallback callback,
                                            NetworkElement element,
                                            NetworkContentElement content)
            : base(callback, element, content)
        {

        }

        public override void Handle()
        {
            dynamic[] credentials = _content["credentials"];
            logger.Info($"credentials : {credentials.Length}");
        }
    }
}
