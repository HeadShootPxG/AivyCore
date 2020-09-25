﻿using AivyData.API;
using AivyData.API.Proxy;
using AivyDofus.Handler;
using AivyDofus.Protocol.Elements;
using AivyDomain.Callback.Client;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AivyDofus.Proxy.Handlers.Customs.Connection
{
    [ProxyHandler(ProtocolName = "ServerSelectionMessage")]
    public class ServerSelectionMessageHandler : AbstractMessageHandler
    {
        static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public override bool IsForwardingData => false;

        public ServerSelectionMessageHandler(AbstractClientReceiveCallback callback,
                                            NetworkElement element,
                                            NetworkContentElement content)
            : base(callback, element, content)
        {

        }

        private string _rnd_ticket(int length = 32, bool upper = true)
        {
            string str = string.Empty;
            Random rdn = new Random();
            for (int i = 1; i <= length; i++)
            {
                int num = rdn.Next(0, 26);
                str += (char)('a' + num);
            }
            return upper ? str.ToUpper() : str;
        }

        public override void Handle()
        {
            if (DofusProxy._proxy_api.GetData(null).custom_servers.FirstOrDefault(x => x.ServerId == (int)_content["serverId"]) is ProxyCustomServerData _data)
            {
                NetworkElement element = BotofuProtocolManager.Protocol[ProtocolKeyEnum.Messages, x => x.name == "SelectedServerDataMessage"];

                logger.Info("request connection in custom server");

                Send(false, _callback._client, element, new NetworkContentElement() 
                {
                    fields = 
                    {
                        { "serverId", _data.ServerId },
                        { "address", _data.IpAddress },
                        { "ports", _data.Ports },
                        { "canCreateNewCharacter", true },
                        { "ticket", _rnd_ticket() }
                    }
                });

                _callback._client_disconnector.Handle(_callback._client);
            }
            else
            {
                Send(true, _callback._remote, _element, _content, _callback.InstanceId);
            }
        }

        public override void Error(Exception e)
        {
            logger.Error(e);
        }
    }
}
