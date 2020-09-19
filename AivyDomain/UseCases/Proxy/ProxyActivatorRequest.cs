using AivyData.API;
using AivyData.Entities;
using AivyDomain.Callback.Proxy;
using AivyDomain.Repository;
using NLog;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace AivyDomain.UseCases.Proxy
{
    public class ProxyActivatorRequest : IRequestHandler<ProxyEntity, bool, ProxyAcceptCallback, ProxyEntity>
    {
        static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private readonly IRepository<ProxyEntity, ProxyData> _repository;

        public ProxyActivatorRequest(IRepository<ProxyEntity, ProxyData> repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public ProxyEntity Handle(ProxyEntity request1, bool request2, ProxyAcceptCallback request3)
        {
            return _repository.ActionResult(x => x.Port == request1.Port && request1.ProcessId == x.ProcessId, x =>
            {
                if (x.IsRunning == request2)
                    throw new Exception($"proxy running state is already : {x.IsRunning}");

                if (request2)
                {
                    x.Socket.Bind(new IPEndPoint(IPAddress.Any, x.Port));
                    x.Socket.Listen(10);

                    x.Socket.BeginAccept(request3.Callback, x.Socket);
                }
                else
                {
                    x.Socket.Dispose();
                }

                x.IsRunning = request2;

                logger.Info($"proxy running state : {x.IsRunning}");

                return x;
            });
        }
    }
}
