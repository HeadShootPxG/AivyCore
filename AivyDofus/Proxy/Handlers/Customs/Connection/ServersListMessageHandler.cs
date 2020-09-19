using AivyData.API;
using AivyData.API.Proxy;
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
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AivyDofus.Proxy.Handlers.Customs.Connection
{
    [ProxyHandler(ProtocolId = 30)]
    public class ServersListMessageHandler : AbstractMessageHandler
    {
        static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public override bool IsForwardingData => false;

        public ServersListMessageHandler(AbstractClientReceiveCallback callback, 
                                         NetworkElement element,
                                         NetworkContentElement content)
            : base(callback, element, content)
        {

        }

        public override void Handle()
        {
            IEnumerable<dynamic> _servers = _content["servers"];

            List<ClientEntity> _testers = new List<ClientEntity>(); 
            foreach (ProxyCustomServerData custom in DofusProxy._proxy_api.GetData(null).custom_servers)
            {
                int status = 0;

                if (custom.Ports.Length > 0)
                {
                    // to do
                    ClientEntity _tester = _callback._client_creator.Handle(new IPEndPoint(IPAddress.Parse(custom.IpAddress), custom.Ports[0]));
                    _tester = _callback._client_linker.Handle(_tester, new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp));
                    _tester = _callback._client_connector.Handle(_tester, new ClientConnectCallback(_tester));

                    try
                    {
                        int c = 0;
                        while(!_tester.IsRunning)
                        {
                            if(c++ > 2000)
                            {
                                throw new Exception();
                            }
                        }

                        status = 3;
                    }
                    catch
                    {
                        status = 1;
                    }

                    _testers.Add(_tester);
                }

                _servers = _servers.Append(new NetworkContentElement()
                {
                    fields =
                    {
                        { "isMonoAccount", custom.IsMonoAccount },
                        { "isSelectable", true },
                        { "id", custom.ServerId },
                        { "type", custom.Type },
                        { "status", status },// to do
                        { "completion", 0 },
                        { "charactersCount", 1 },
                        { "charactersSlots", 1 },
                        { "date", 1234828800000 }
                    }
                });

            }

            _content["servers"] = _servers.ToArray();

            Send(false, _callback._remote, _element, _content);

            foreach(ClientEntity _tester in _testers)
            {
                _callback._client_disconnector.Handle(_tester);
            }
        }
    }
}
