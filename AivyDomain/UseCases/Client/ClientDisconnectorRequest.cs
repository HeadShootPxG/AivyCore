using AivyData.Entities;
using AivyDomain.Repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace AivyDomain.UseCases.Client
{
    public class ClientDisconnectorRequest : IRequestHandler<ClientEntity, ClientEntity>
    {
        private readonly IRepository<ClientEntity> _repository;

        public ClientDisconnectorRequest(IRepository<ClientEntity> repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public ClientEntity Handle(ClientEntity request)
        {
            return _repository.ActionResult(x => x.RemoteIp == request.RemoteIp, x =>
            {
                if (request is null)
                    throw new ArgumentNullException(nameof(request));

                if(x.IsRunning)
                    x.Socket.Disconnect(true);

                return x;
            });
        }
    }
}
