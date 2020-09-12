using SocketHook;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace AivyData.Entities
{
    public class HookInterfaceEntity : HookInterface
    {
        public event Action<IPEndPoint, int, int> OnIpRedirected;

        public override void IpRedirected(IPEndPoint ip, int processId, int redirectionPort)
        {
            base.IpRedirected(ip, processId, redirectionPort);
            OnIpRedirected?.Invoke(ip, processId, redirectionPort);
        }
    }
}
