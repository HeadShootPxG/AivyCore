using AivyData.API.Proxy;
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
        public ProxyAccountMinimumInformationData AccountData { get; set; }

        public uint LAST_CLIENT_INSTANCE_ID { get; set; }
        public uint MESSAGE_RECEIVED_FROM_LAST { get; set; }
        public uint FAKE_MESSAGE_CREATED { get; set; }        

        public uint GLOBAL_INSTANCE_ID
        {
            get
            {
                return LAST_CLIENT_INSTANCE_ID + MESSAGE_RECEIVED_FROM_LAST + FAKE_MESSAGE_CREATED;
            }
            set
            {
                uint diff = value - GLOBAL_INSTANCE_ID;
                FAKE_MESSAGE_CREATED += diff;
            }
        }
    }
}
