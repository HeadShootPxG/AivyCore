using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace AivyData.Entities
{
    public class ServerEntity
    {
        public Socket Socket { get; set; }
        public bool IsRunning { get; set; } 
        public int Port { get; set; }
    }
}
