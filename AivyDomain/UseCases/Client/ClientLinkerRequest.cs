using AivyData.API;
using AivyData.Entities;
using AivyDomain.Repository;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace AivyDomain.UseCases.Client
{
    public class ClientLinkerRequest : IRequestHandler<ClientEntity, Socket, ClientEntity>
    {
        private readonly IRepository<ClientEntity, ClientData> _repository;

        public ClientLinkerRequest(IRepository<ClientEntity, ClientData> repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public ClientEntity Handle(ClientEntity request1, Socket request2)
        {
            return _repository.ActionResult(x => x.IsRunning ? x.Socket?.RemoteEndPoint == request1.Socket?.RemoteEndPoint : x.RemoteIp == request1.RemoteIp, x => 
            {
                if (request1 is null) throw new ArgumentNullException(nameof(request1));
                if (request2 is null) throw new ArgumentNullException(nameof(request2));

                x.Socket = request2;
                return x;
            });
        }
    }
}
