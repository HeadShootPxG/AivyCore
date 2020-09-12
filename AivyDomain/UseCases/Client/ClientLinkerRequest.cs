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
        private readonly IRepository<ClientEntity> _repository;

        public ClientLinkerRequest(IRepository<ClientEntity> repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public ClientEntity Handle(ClientEntity request1, Socket request2)
        {
            return _repository.ActionResult(x => x == request1, x => 
            {
                if (request1 is null) throw new ArgumentNullException(nameof(request1));
                if (request2 is null) throw new ArgumentNullException(nameof(request2));

                x.Socket = request2;
                return x;
            });
        }
    }
}
