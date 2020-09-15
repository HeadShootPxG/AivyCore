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
    [ProxyHandler(ProtocolId = 4)]
    public class IdentificationMessageHandler : AbstractMessageHandler
    {
        static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public override bool IsForwardingData => true;

        public IdentificationMessageHandler(ClientSenderRequest sender,
                                      NetworkElement message,
                                      NetworkContentElement content,
                                      ClientEntity client,
                                      ClientEntity remote = null)
            : base(sender, message, content, client, remote)
        {

        }

        public override void Handle()
        {
            dynamic[] credentials = _content["credentials"];
            logger.Info($"credentials : {credentials.Length}");
        }
    }
}
