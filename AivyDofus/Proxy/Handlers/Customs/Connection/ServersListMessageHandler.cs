using AivyData.Entities;
using AivyData.Enums;
using AivyDofus.Handler;
using AivyDofus.Protocol.Buffer;
using AivyDofus.Protocol.Elements;
using AivyDofus.Proxy.Callbacks;
using AivyDomain.Callback.Client;
using AivyDomain.UseCases.Client;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AivyDofus.Proxy.Handlers.Customs.Connection
{
    [ProxyHandler(ProtocolId = 30)]
    public class ServersListMessageHandler : AbstractMessageHandler
    {
        static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public override bool IsForwardingData => false;

        public ServersListMessageHandler(ProxyClientReceiveCallback callback, 
                                         NetworkElement element,
                                         NetworkContentElement content)
            : base(callback, element, content)
        {

        }

        public override void Handle()
        {
            IEnumerable<dynamic> _servers = _content["servers"];            
            _content["servers"] = _servers/*.Append(new NetworkContentElement()
            {
                fields =
                {
                    { "isMonoAccount", false },
                    { "isSelectable", true },
                    { "id", 127 },
                    { "type", 1 },
                    { "status", 3 },
                    { "completion", 0 },
                    { "charactersCount", 1 },
                    { "charactersSlots", 5 },
                    { "date", 1234828800000 }
                }
            }).Append(new NetworkContentElement()
            {
                fields =
                {
                    { "isMonoAccount", false },
                    { "isSelectable", true },
                    { "id", 119 },
                    { "type", 1 },
                    { "status", 3 },
                    { "completion", 0 },
                    { "charactersCount", 1 },
                    { "charactersSlots", 5 },
                    { "date", 1234828800000 }
                }
            }).Append(new NetworkContentElement()
            {
                fields =
                {
                    { "isMonoAccount", false },
                    { "isSelectable", true },
                    { "id", 122 },
                    { "type", 1 },
                    { "status", 3 },
                    { "completion", 0 },
                    { "charactersCount", 1 },
                    { "charactersSlots", 5 },
                    { "date", 1234828800000 }
                }
            })*/.Append(new NetworkContentElement()
            {
                fields =
                {
                    { "isMonoAccount", true },
                    { "isSelectable", true },
                    { "id", 671 },
                    { "type", 1 },
                    { "status", 3 },
                    { "completion", 0 },
                    { "charactersCount", 1 },
                    { "charactersSlots", 5 },
                    { "date", 1234828800000 }
                }
            })/*.Append(new NetworkContentElement()
            {
                fields =
                {
                    { "isMonoAccount", false },
                    { "isSelectable", true },
                    { "id", 901 },
                    { "type", 1 },
                    { "status", 3 },
                    { "completion", 0 },
                    { "charactersCount", 1 },
                    { "charactersSlots", 5 },
                    { "date", 1234828800000 }
                }
            })*/.ToArray();


            Send(false, _callback._remote, _element, _content);
        }
    }
}
