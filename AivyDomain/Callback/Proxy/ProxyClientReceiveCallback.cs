using AivyData.Entities;
using AivyData.Enums;
using AivyDomain.Callback.Client;
using AivyDomain.Repository.Client;
using AivyDomain.UseCases.Client;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace AivyDomain.Callback.Proxy
{
    public class ProxyClientReceiveCallback : AbstractClientReceiveCallback
    {
        static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public ProxyClientReceiveCallback(ClientEntity client, 
                                          ClientEntity remote,
                                          ClientRepository repository,
                                          ClientCreatorRequest creator,
                                          ClientLinkerRequest linker,
                                          ClientConnectorRequest connector,
                                          ClientDisconnectorRequest disconnector,
                                          ClientSenderRequest sender,
                                          ProxyTagEnum tag = ProxyTagEnum.UNKNOW)
            : base(client, remote, repository, creator, linker, connector, disconnector, sender, tag)
        {

        }

        protected override void _constructor_handled()
        {
            _rcv_action += OnReceive;
        }

        protected virtual void OnReceive(MemoryStream stream)
        {
            logger.Info($"[{_tag}] : {string.Join(" ", stream.ToArray().Select(x => x.ToString("X2"))) }");

            _client_sender.Handle(_remote, stream.ToArray());
        }
    }
}
