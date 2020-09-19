using AivyData.API;
using AivyData.Entities;
using AivyDomain.Callback;
using AivyDomain.Callback.Client;
using AivyDomain.Repository;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace AivyDomain.UseCases.Client
{
    public class ClientReceiverRequest : IRequestHandler<ClientEntity, ClientReceiveCallback,ClientEntity> 
    {
        private readonly IRepository<ClientEntity, ClientData> _repository;

        public ClientReceiverRequest(IRepository<ClientEntity, ClientData> repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public ClientEntity Handle(ClientEntity request1, ClientReceiveCallback request2)
        {
            return _repository.ActionResult(x => x.IsRunning ? x.Socket.RemoteEndPoint == request1.Socket.RemoteEndPoint : x.RemoteIp == request1.RemoteIp, x =>
            {
                if (request1 is null) throw new ArgumentNullException(nameof(request1));
                if (request2 is null) throw new ArgumentNullException(nameof(request2));

                x.Socket.BeginReceive(request2._buffer, 0, request2._buffer.Length, SocketFlags.None, request2.Callback, x.Socket);
                return x;
            });
        }
    }
}
