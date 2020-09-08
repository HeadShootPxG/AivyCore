using AivyData.Entities;
using AivyDomain.Callback.Client;
using AivyDomain.Repository;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace AivyDomain.UseCases.Client
{
    public class ClientReceiverRequest : IRequestHandler<ClientEntity, ClientEntity>
    {
        private readonly IRepository<ClientEntity> _repository;

        public ClientReceiverRequest(IRepository<ClientEntity> repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public ClientEntity Handle(ClientEntity request)
        {
            return _repository.ActionResult(x => x.Socket.RemoteEndPoint == request.Socket.RemoteEndPoint, x =>
            {
                ClientReceiveCallback callback = new ClientReceiveCallback(x);
                x.Socket.BeginReceive(callback._buffer, 0, callback._buffer.Length, SocketFlags.None, callback.Callback, x.Socket);
                return x;
            });
        }
    }
}
