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
    // remove commentary if you want to handle it
    //[ProxyHandler(ProtocolId = 6253)]
    public class RawDataMessageHandler : AbstractMessageHandler
    {
        static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public override bool IsForwardingData => true;

        public RawDataMessageHandler(AbstractClientReceiveCallback callback, 
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
