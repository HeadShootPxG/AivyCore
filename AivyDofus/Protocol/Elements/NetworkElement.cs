using AivyDofus.Protocol.Elements.Fields;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AivyDofus.Protocol.Elements
{
    public class NetworkElement
    {
        public ClassField[] fields { get; set; }
        public string name { get; set; }
        public int protocolID { get; set; }
        public string super { get; set; }
        public bool super_serialize { get; set; }
        public string supernamespace { get; set; }
        public bool use_hash_function { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        [JsonIgnore]
        public string BasicString
        {
            get
            {
                return $"{name} ({protocolID})";
            }
        }

        ~NetworkElement()
        {
            fields = null;
            name = null;
            protocolID = -1;
            super = null;
            super_serialize = false;
            supernamespace = null;
            use_hash_function = false;
        }
    }
}
