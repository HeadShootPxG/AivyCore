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
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace AivyDofus.Proxy.Handlers.Customs.World.Map
{
    [ProxyHandler(ProtocolId = 226)]
    public class MapComplementaryInformationsDataMessageHandler : AbstractMessageHandler
    {
        static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public override bool IsForwardingData => true;

        public MapComplementaryInformationsDataMessageHandler(AbstractClientReceiveCallback callback, 
                                                              NetworkElement element,
                                                              NetworkContentElement content)
            : base(callback, element, content)
        {

        }

        public override void Handle()
        {
            /* // test
            IEnumerable<dynamic> _actors = _content["actors"];

            _actors = new dynamic[] { _actors.FirstOrDefault(x => x["protocol_id"] == 36 && x["name"] == "zobi-poilue") };

            _content["actors"] = _actors.ToArray();
            Send(false, _callback._remote, _element, _content);
            */
        }

        public override void Error(Exception e)
        {
            logger.Error(e);
        }
    }
}
