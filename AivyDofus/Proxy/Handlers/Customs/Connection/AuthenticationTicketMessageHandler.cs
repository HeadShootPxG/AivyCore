using AivyDofus.Crypto;
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

namespace AivyDofus.Proxy.Handlers.Customs.Connection
{
    [ProxyHandler(ProtocolName = "AuthenticationTicketMessage")]
    public class AuthenticationTicketMessageHandler : AbstractMessageHandler
    {
        static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public override bool IsForwardingData => false;

        public AuthenticationTicketMessageHandler(AbstractClientReceiveCallback callback,
                                                  NetworkElement element,
                                                  NetworkContentElement content)
            : base(callback, element, content)
        {

        }

        public override void Handle()
        {
            DofusProxyClientReceiveCallback _proxy_callback = _casted_callback<DofusProxyClientReceiveCallback>();

            if (_proxy_callback._proxy.AccountData.ConnectedToCustomServer)
            {
                _content["ticket"] = Hash.md5_str(_proxy_callback._proxy.AccountData.ToString());
            }
            Send(true, _callback._remote, _element, _content, _callback.InstanceId);
        }

        public override void Error(Exception e)
        {
            logger.Error(e);
        }
    }
}
