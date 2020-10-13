using AivyDofus.Handler;
using AivyDofus.Protocol.Elements;
using AivyDofus.Proxy.Callbacks;
using AivyDomain.Callback.Client;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AivyDofus.Proxy.Handlers.Customs.World.Exchange
{
    //[ProxyHandler(ProtocolName = "ExchangeStartedWithStorageMessage")]
    public class ExchangeStartedWithStorageMessageHandler : AbstractMessageHandler
    {
        static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public ExchangeStartedWithStorageMessageHandler(AbstractClientReceiveCallback callback, NetworkElement element, NetworkContentElement content) 
            : base(callback, element, content)
        {
        }

        public override bool IsForwardingData => true;

        public static readonly NetworkElement _exchange_object_move_kama = BotofuProtocolManager.Protocol[ProtocolKeyEnum.Messages, x => x.name == "ExchangeObjectMoveKamaMessage"];
        public static NetworkContentElement _exchange_object_move_kama_message(long quantity)
        {
            return new NetworkContentElement()
            {
                fields =
                {
                    { "quantity", quantity }
                }
            };
        }

        public override void Handle()
        {

        }

        public override void Error(Exception e)
        {
            logger.Error(e);
        }
    }
}
