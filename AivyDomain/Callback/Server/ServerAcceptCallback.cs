using AivyData.Entities;
using AivyDomain.API.Client;
using AivyDomain.Callback.Client;
using AivyDomain.Mappers.Client;
using AivyDomain.Repository.Client;
using AivyDomain.UseCases.Client;
using NLog;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace AivyDomain.Callback.Server
{
    public class ServerAcceptCallback : ServerCallback 
    {
        static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private readonly OpenClientApi _client_api;
        private readonly ClientEntityMapper _client_mapper;
        private readonly ClientRepository _client_repository;

        private readonly ClientCreatorRequest _client_creator;
        private readonly ClientLinkerRequest _client_linker;
        private readonly ClientReceiverRequest _client_receiver;

        public ServerAcceptCallback(ServerEntity server)
            : base(server)
        {
            _client_api = new OpenClientApi("./server_callback_api.json");
            _client_mapper = new ClientEntityMapper();
            _client_repository = new ClientRepository(_client_api, _client_mapper);

            _client_creator = new ClientCreatorRequest(_client_repository);
            _client_linker = new ClientLinkerRequest(_client_repository);
            _client_receiver = new ClientReceiverRequest(_client_repository);
        }

        public override void Callback(IAsyncResult result)
        {
            if (_server.IsRunning)
            {
                _server.Socket = (Socket)result.AsyncState;

                Socket _client_socket = _server.Socket.EndAccept(result);

                ClientEntity client = _client_creator.Handle(_client_socket.RemoteEndPoint as IPEndPoint);
                client = _client_linker.Handle(client, _client_socket);
                client = _client_receiver.Handle(client, new ClientReceiveCallback(client));

                logger.Info("client connected");

                _server.Socket.BeginAccept(Callback, _server.Socket);
            }
        }
    }
}
