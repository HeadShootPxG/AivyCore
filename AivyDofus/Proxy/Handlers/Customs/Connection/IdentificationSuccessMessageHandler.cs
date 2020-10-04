using AivyDofus.Handler;
using AivyDofus.Protocol.Elements;
using AivyDofus.Proxy.Callbacks;
using AivyDomain.Callback.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AivyDofus.Proxy.Handlers.Customs.Connection
{
    [ProxyHandler(ProtocolName = "IdentificationSuccessMessage")]
    public class IdentificationSuccessMessageHandler : AbstractMessageHandler
    {
        public IdentificationSuccessMessageHandler(AbstractClientReceiveCallback callback, NetworkElement element, NetworkContentElement content)
            : base(callback, element, content)
        {
        }

        public override bool IsForwardingData => false;

        public override void Handle()
        {
            DofusProxyClientReceiveCallback _proxy_callback = _casted_callback<DofusProxyClientReceiveCallback>();

            double account_id = _content["accountId"];
            string nickname = _content["nickname"];

            _proxy_callback._proxy.AccountData.AccountId = account_id;
            _proxy_callback._proxy.AccountData.Nickname = nickname;

            _content["hasConsoleRight"] = true;
            _content["hasRights"] = true;

            Send(false, _callback._remote, _element, _content);
        }
    }
}
