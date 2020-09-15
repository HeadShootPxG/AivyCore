using AivyData.Entities;
using AivyDofus.Handler;
using AivyDofus.Protocol.Elements;
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

        public RawDataMessageHandler(ClientSenderRequest sender,
                                      NetworkElement message,
                                      NetworkContentElement content,
                                      ClientEntity client,
                                      ClientEntity remote = null)
            : base(sender, message, content, client, remote)
        {

        }

        public override void Handle()
        {
            logger.Info($"rdm len : {_content["content"].Length}");
        }
    }
}
