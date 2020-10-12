using AivyData.Entities;
using AivyData.Enums;
using AivyDomain.Callback.Client;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace AivyDofus.Proxy.Callbacks
{
    public class DofusRetroProxyAcceptCallback : DofusProxyAcceptCallback
    {
        static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public DofusRetroProxyAcceptCallback(ProxyEntity proxy)
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
                DofusRetroProxyClientReceiveCallback remote_rcv_callback = new DofusRetroProxyClientReceiveCallback(remote, client, _client_repository, _client_creator, _client_linker, _client_connector, _client_disconnector, _client_sender, _proxy, ProxyTagEnum.Server);
                remote = _client_connector.Handle(remote, new ClientConnectCallback(remote, remote_rcv_callback));

                if (client.IsRunning && remote.IsRunning)
                {
                    client = _client_receiver.Handle(client, new DofusRetroProxyClientReceiveCallback(client, remote, _client_repository, _client_creator, _client_linker, _client_connector, _client_disconnector, _client_sender, _proxy, ProxyTagEnum.Client));

                    logger.Info("client connected");
                }
                else
                {
                    client = _client_disconnector.Handle(client);

                    logger.Info("remote client cannot connect to server");
                }

                _proxy.Socket.BeginAccept(Callback, _proxy.Socket);
            }
        }
    }
}
