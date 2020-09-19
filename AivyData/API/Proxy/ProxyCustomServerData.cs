using System;
using System.Collections.Generic;
using System.Text;

namespace AivyData.API.Proxy
{
    public class ProxyCustomServerData
    {
        public int ServerId { get; set; }
        public string IpAddress { get; set; }
        public short[] Ports { get; set; }

        public bool IsMonoAccount { get; set; }
        public byte Type { get; set; }
    }
}
