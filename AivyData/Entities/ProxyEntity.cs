using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace AivyData.Entities
{
    public class ProxyEntity : ServerEntity
    {
        public int ProcessId { get; set; }
        public HookEntity Hooker { get; set; }
        public HookInterfaceEntity HookInterface { get; set; }
        public Queue<IPEndPoint> IpRedirectedStack { get; set; }
    }
}
