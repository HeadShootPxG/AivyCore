using AivyData.Entities;
using AivyData.Enums;
using AivyDofus.Handler;
using AivyDofus.Protocol.Elements;
using AivyDofus.Proxy.Callbacks;
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
    //[ProxyHandler(ProtocolId = 226)]
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
            if (_callback is DofusProxyClientReceiveCallback _dofus_callback)
            {
                Task.Delay(5000).ContinueWith(task =>
                {
                    SendMultiClientChatMessage(_callback._client, 0, "Wesh les moules =D", ++_dofus_callback._proxy.GLOBAL_INSTANCE_ID);
                });
            }
        }

        public override void Error(Exception e)
        {
            logger.Error(e);
        }
    }
}
