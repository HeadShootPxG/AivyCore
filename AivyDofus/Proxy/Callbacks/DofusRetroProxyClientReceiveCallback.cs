using AivyData.Entities;
using AivyData.Enums;
using AivyDomain.UseCases.Client;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AivyDofus.Proxy.Callbacks
{
    public class DofusRetroProxyClientReceiveCallback : DofusProxyClientReceiveCallback
    {
        static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public DofusRetroProxyClientReceiveCallback(ClientEntity client, ClientEntity remote, ClientCreatorRequest creator, ClientLinkerRequest linker, ClientConnectorRequest connector, ClientDisconnectorRequest disconnector, ClientSenderRequest sender, ProxyEntity proxy, ProxyTagEnum tag = ProxyTagEnum.UNKNOW) 
            : base(client, remote, creator, linker, connector, disconnector, sender, proxy, tag)
        {

        }

        /// <summary>
        /// to do , packet reading
        /// </summary>
        /// <param name="stream"></param>
        protected override void OnReceive(MemoryStream stream)
        {
            byte[] arr = stream.ToArray();

            logger.Info($"rcv : {Encoding.ASCII.GetString(arr)}");
            _client_sender.Handle(_remote, stream.ToArray());
        }
    }
}
