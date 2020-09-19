using System;
using System.Collections.Generic;
using System.Text;

namespace AivyData.API
{
    public class ServerData
    {
        public short Port { get; set; }
        public string Ip { get; set; }
       
        public int ServerId { get; set; }
        public bool IsMonoAccount { get; set; }
        public byte Completion { get; set; }
        public byte Status { get; set; }
        public int MaxCharacterCount { get; set; }
        public byte Type { get; set; }

        public ServerData()
        {

        }

        public override string ToString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this, Newtonsoft.Json.Formatting.Indented);
        }
    }
}
