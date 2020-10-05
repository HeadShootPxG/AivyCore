using AivyDofus.Handler;
using AivyDofus.Protocol.Elements;
using AivyDomain.Callback.Client;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AivyDofus.Proxy.Handlers.Customs.Choice
{
    [ProxyHandler(ProtocolName = "CharactersListMessage")]
    public class CharactersListMessageHandler : AbstractMessageHandler
    {
        static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public override bool IsForwardingData => true;

        public CharactersListMessageHandler(AbstractClientReceiveCallback callback, NetworkElement element, NetworkContentElement content)
            : base(callback, element, content)
        {

        }

        public override void Handle()
        {

        }
    }
}
