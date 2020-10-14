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

        public ProxyEntity Handle(ProxyEntity proxy, bool active, ProxyAcceptCallback callback)
        {
            return _repository.ActionResult(x => x.Port == proxy.Port && proxy.ProcessId == x.ProcessId, x =>
            {
                if (x.IsRunning == active)
                    throw new Exception($"proxy running state is already : {x.IsRunning}");

                if (active)
                {
                    if (callback is null) throw new ArgumentNullException(nameof(callback));

                    x.Socket.Bind(new IPEndPoint(IPAddress.Any, x.Port));
                    x.Socket.Listen(10);

                    x.Socket.BeginAccept(callback.Callback, x.Socket);
                }
                else
                {
                    x.Socket.Dispose();
                    _repository.Remove(v => v.Port == x.Port);
                }

                x.IsRunning = active;

                logger.Info($"proxy running state : {x.IsRunning}");

                return x;
            });
        }
    }
}
