using AivyData.API;
using AivyData.Entities;
using AivyDomain.Repository;
using EasyHook;
using SocketHook;
using System;
using System.Collections.Generic;
using System.Text;

namespace AivyDomain.UseCases.Proxy
{
    public class HookCreatorRequest : IRequestHandler<string, ProxyEntity, HookEntity>
    {
        private readonly IRepository<ProxyEntity, ProxyData> _repository;
        private readonly HookInjectorRequest _hook_injector;

        public HookCreatorRequest(IRepository<ProxyEntity, ProxyData> repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _hook_injector = new HookInjectorRequest(_repository);
        }

        public HookEntity Handle(string exePath, ProxyEntity request)
        {
            return _repository.ActionResult(x => x.Port == request.Port, x =>
            {
                if (exePath is null || exePath is "")
                    throw new ArgumentNullException(nameof(exePath));

                if (request is null)
                    throw new ArgumentNullException(nameof(request));

                x.Hooker = new HookEntity()
                {
                    ExePath = exePath
                };
                x.Hooker.Hook = HookManager.CreateElement(x.HookInterface);
                x.Hooker = _hook_injector.Handle(x);

                return x;
            }).Hooker;
        }
    }
}
