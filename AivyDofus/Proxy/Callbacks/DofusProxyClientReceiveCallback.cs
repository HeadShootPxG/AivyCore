using AivyData.Entities;
using AivyData.Enums;
using AivyDomain.Callback.Client;
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
    public class DofusProxyClientReceiveCallback : ProxyClientReceiveCallback
    {
        static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public DofusProxyClientReceiveCallback(ClientEntity client, ClientEntity remote, ClientSenderRequest sender, ClientDisconnectorRequest disconnector, ProxyTagEnum tag = ProxyTagEnum.UNKNOW)
            : base(client, remote, sender, disconnector, tag)
        {
            _rcv_action = OnReceive;
        }

        private void OnReceive(MemoryStream stream)
        {
            logger.Info($"rcv_len : {stream.Length}");
        }
    }
}
