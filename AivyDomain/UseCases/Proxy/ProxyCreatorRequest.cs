using AivyData.Entities;
using AivyDomain.Repository;
using System;
using System.Net;
using System.Collections.Generic;
using System.Text;
using NLog;
using AivyData.API;

namespace AivyDomain.UseCases.Proxy
{
    public class ProxyCreatorRequest : IRequestHandler<string, int, ProxyEntity>
    {
        static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private readonly IRepository<ProxyEntity, ProxyData> _repository;
        private readonly HookCreatorRequest _hook_creator;

        public ProxyCreatorRequest(IRepository<ProxyEntity, ProxyData> repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _hook_creator = new HookCreatorRequest(_repository);
        }

        /// <summary>
        /// request1 = processId , request2 = port
        /// </summary>
        /// <param name="request1"></param>
        /// <param name="request2"></param>
        /// <returns></returns>
        public ProxyEntity Handle(string exePath, int port)
        {
            return _repository.ActionResult(x => x.Port == port, x =>
            {
                x.Port = port;
                x.HookInterface.OnIpRedirected += (ip, processId, portRed) =>
                {
                    x.IpRedirectedStack.Enqueue(ip);
                };
                x.Hooker = _hook_creator.Handle(exePath, x);

                return x;
            });
        }
    }
}
