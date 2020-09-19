using AivyData.API;
using AivyData.Entities;
using AivyDomain.Repository;
using NLog;
using System;
using System.Collections.Generic;
using System.Text;

namespace AivyDomain.UseCases.Client
{
    public class ClientDisconnectorRequest : IRequestHandler<ClientEntity, ClientEntity>
    {
        static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private readonly IRepository<ClientEntity, ClientData> _repository;

        public ClientDisconnectorRequest(IRepository<ClientEntity, ClientData> repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public ClientEntity Handle(ClientEntity request)
        {
            return _repository.ActionResult(x => x.RemoteIp == request.RemoteIp, x =>
            {
                if (request is null)
                    throw new ArgumentNullException(nameof(request));

                logger.Info($"client leaved (was connected ? {x.IsRunning})");

                if (!x.IsRunning)
                    return x;

                x.Socket.Disconnect(true);
                return x;
            });
        }
    }
}
