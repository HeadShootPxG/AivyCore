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
    public class HookInjectorRequest : IRequestHandler<ProxyEntity, HookEntity>
    {
        private readonly IRepository<ProxyEntity, ProxyData> _repository;

        public HookInjectorRequest(IRepository<ProxyEntity, ProxyData> repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public HookEntity Handle(ProxyEntity request)
        {
            return _repository.ActionResult(x => x.Port == request.Port, x =>
            {
                HookElement _hook = x.Hooker.Hook;

                RemoteHooking.CreateAndInject(
                x.Hooker.ExePath,
                string.Empty,
                0x00000004,
                InjectionOptions.DoNotRequireStrongName,
                "./SocketHook.dll",
                "./SocketHook.dll",
                out _hook.ProcessId,
                _hook.ChannelName,
                request.Port);

                return x;
            }).Hooker;
        }
    }
}
