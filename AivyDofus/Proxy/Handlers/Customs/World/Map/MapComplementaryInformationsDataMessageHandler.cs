using AivyData.Entities;
using AivyData.Enums;
using AivyDofus.Handler;
using AivyDofus.Protocol.Elements;
using AivyDomain.UseCases.Client;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AivyDofus.Proxy.Handlers.Customs.World.Map
{
    //[ProxyHandler(ProtocolId = 226)]
    /*public class MapComplementaryInformationsDataMessageHandler : AbstractMessageHandler
    {
        static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public override bool IsForwardingData => true;

        public MapComplementaryInformationsDataMessageHandler(ClientSenderRequest sender,
                                      NetworkElement message,
                                      NetworkContentElement content,
                                      ClientEntity client,
                                      ProxyTagEnum tag = ProxyTagEnum.UNKNOW,
                                      ClientEntity remote = null)
            : base(sender, message, content, client, tag, remote)
        {

        }

        public override void Handle()
        {
            logger.Info($"map id : {_content["mapId"]}");
        }
    }*/
}
