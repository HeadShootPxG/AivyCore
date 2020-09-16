using AivyData.Entities;
using AivyData.Enums;
using AivyDofus.Handler;
using AivyDofus.Protocol.Buffer;
using AivyDofus.Protocol.Elements;
using AivyDofus.Proxy.Callbacks;
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
    [ProxyHandler(ProtocolId = 30)]
    public class ServersListMessageHandler : AbstractMessageHandler
    {
        static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public override bool IsForwardingData => false;

        public ServersListMessageHandler(ProxyClientReceiveCallback callback, 
                                         NetworkElement element,
                                         NetworkContentElement content)
            : base(callback, element, content)
        {

        }

        public override void Handle()
        {
            logger.Info($"{_content}");
            Send(_callback._remote, _element, _content);
        }
    }
}
