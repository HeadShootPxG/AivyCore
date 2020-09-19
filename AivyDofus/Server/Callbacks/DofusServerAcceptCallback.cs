using AivyData.Entities;
using AivyDofus.Protocol.Elements;
using AivyDomain.Callback.Server;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace AivyDofus.Server.Callbacks
{
    public class DofusServerAcceptCallback : ServerAcceptCallback
    {
        static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public DofusServerAcceptCallback(ServerEntity server) 
            : base(server)
        {

        }

        public override void Callback(IAsyncResult result)
        {
            if (_server.IsRunning)
            {
                _server.Socket = (Socket)result.AsyncState;

                Socket _client_socket = _server.Socket.EndAccept(result);

                ClientEntity client = _client_creator.Handle(_client_socket.RemoteEndPoint as IPEndPoint);
                client = _client_linker.Handle(client, _client_socket);
                client = _client_receiver.Handle(client, new DofusServerClientReceiveCallback(client, _client_creator, _client_linker, _client_connector, _client_disconnector, _client_sender));

                logger.Info("client connected");

                _server.Socket.BeginAccept(Callback, _server.Socket);
            }
        }
    }
}
