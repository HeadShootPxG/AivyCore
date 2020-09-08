using AivyData.Entities;
using AivyDomain.Callback.Server;
using AivyDomain.Repository;
using NLog;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace AivyDomain.UseCases.Server
{
    public class ServerActivatorRequest : IRequestHandler<ServerEntity, bool, ServerEntity>
    {
        private readonly IRepository<ServerEntity> _repository;

        static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public ServerActivatorRequest(IRepository<ServerEntity> repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public ServerEntity Handle(ServerEntity request1, bool request2)
        {
            return _repository.ActionResult(x => x.Port == request1.Port, x => 
            {
                if (x.IsRunning == request2)
                    throw new Exception($"server running state is already : {x.IsRunning}");

                if (request2)
                {
                    x.Socket.Bind(new IPEndPoint(IPAddress.Any, x.Port));
                    x.Socket.Listen(10);

                    x.Socket.BeginAccept(new ServerAcceptCallback(x).Callback, x.Socket);
                }
                else
                {
                    x.Socket.Dispose();
                }

                x.IsRunning = request2;

                logger.Info($"server running state : {x.IsRunning}");

                return x;
            });
        }
    }
}
