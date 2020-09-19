using AivyData.API;
using AivyData.Entities;
using AivyDomain.Callback.Client;
using AivyDomain.Repository;
using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace AivyDomain.UseCases.Client
{
    public class ClientConnectorRequest : IRequestHandler<ClientEntity, ClientConnectCallback, ClientEntity>
    {
        private readonly IRepository<ClientEntity, ClientData> _repository;

        public ClientConnectorRequest(IRepository<ClientEntity, ClientData> repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public ClientEntity Handle(ClientEntity request, ClientConnectCallback request2)
        {
            return _repository.ActionResult(x => x.RemoteIp == request.RemoteIp, x =>
            {
                if (x.IsRunning)
                    throw new Exception("client is already connected");

                if (x.RemoteIp is null)
                    throw new ArgumentNullException(nameof(x.RemoteIp));

                x.Socket.BeginConnect(x.RemoteIp, request2.Callback, x.Socket);

                return x;
            });
        }
    }
}
