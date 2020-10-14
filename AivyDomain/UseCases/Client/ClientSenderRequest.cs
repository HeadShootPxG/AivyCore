using AivyData.API;
using AivyData.Entities;
using AivyDomain.Callback.Client;
using AivyDomain.Repository;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace AivyDomain.UseCases.Client
{
    public class ClientSenderRequest : IRequestHandler<ClientEntity, byte[], ClientEntity>
    {
        private readonly IRepository<ClientEntity, ClientData> _repository;

        public ClientSenderRequest(IRepository<ClientEntity, ClientData> repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public ClientEntity Handle(ClientEntity request1, byte[] request2)
        {   
            return _repository.ActionResult(x => x.IsRunning ? x.Socket.RemoteEndPoint == request1.Socket.RemoteEndPoint
                                               : x.RemoteIp == request1.RemoteIp, x =>
            {
                if (request2 is null) throw new ArgumentNullException(nameof(request2));
                if (!x.IsRunning) return x;

                x.Socket.BeginSend(request2, 0, request2.Length, SocketFlags.None, new ClientSendCallback(x).Callback, x.Socket);

                return x;
            });
        }
    }
}
