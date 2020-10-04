using AivyData.Entities;
using AivyData.Enums;
using AivyDomain.Callback.Client;
using AivyDomain.Callback.Proxy;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AivyDofus.Proxy.Callbacks
{
    public class DofusProxyAcceptCallback : ProxyAcceptCallback
    {
        static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public DofusProxyAcceptCallback(ProxyEntity proxy) 
            : base(proxy)
        {
        }

        public override void Callback(IAsyncResult result)
        {
            if (_proxy.IsRunning)
            {
                _proxy.Socket = (Socket)result.AsyncState;

                Socket _client_socket = _proxy.Socket.EndAccept(result);

                ClientEntity client = _client_creator.Handle(_client_socket.RemoteEndPoint as IPEndPoint);
                client = _client_linker.Handle(client, _client_socket);

                ClientEntity remote = _client_creator.Handle(_proxy.IpRedirectedStack.Dequeue());
                remote = _client_linker.Handle(remote, new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp));
                DofusProxyClientReceiveCallback remote_rcv_callback = new DofusProxyClientReceiveCallback(remote, client, _client_repository, _client_creator, _client_linker, _client_connector, _client_disconnector, _client_sender, _proxy, ProxyTagEnum.Server);
                remote = _client_connector.Handle(remote, new ClientConnectCallback(remote, remote_rcv_callback));

                // wait remote client to connect
                try
                {
                    while (!remote.IsRunning) { logger.Info("waiting"); }
                }
                catch(Exception e)
                {
                    logger.Error(e);
                    return;
                }

                if (client.IsRunning)
                {
                    client = _client_receiver.Handle(client, new DofusProxyClientReceiveCallback(client, remote, _client_repository, _client_creator, _client_linker, _client_connector, _client_disconnector, _client_sender, _proxy, ProxyTagEnum.Client));

                    logger.Info("client connected");
                }

                _proxy.Socket.BeginAccept(Callback, _proxy.Socket);
            }
        }
    }
}
