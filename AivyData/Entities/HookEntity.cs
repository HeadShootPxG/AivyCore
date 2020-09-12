using SocketHook;
using System;
using System.Collections.Generic;
using System.Text;

namespace AivyData.Entities
{
    public class HookEntity
    {
        public string ExePath { get; set; }
        public HookElement Hook { get; set; }
    }
}
