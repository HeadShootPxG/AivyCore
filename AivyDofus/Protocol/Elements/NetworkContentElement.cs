using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AivyDofus.Protocol.Elements
{
    public class NetworkContentElement
    {
        public Dictionary<string, dynamic> fields { get; set; } = new Dictionary<string, dynamic>();

        public dynamic this[string key]
        {
            get
            {
                if (fields.ContainsKey(key))
                    return fields[key];
                return null;
            }
            set
            {
                if (fields.ContainsKey(key))
                {
                    fields[key] = value;
                }
                else
                {
                    fields.Add(key, value);
                }
            }
        }

        public override string ToString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this, Newtonsoft.Json.Formatting.Indented);
        }

        public static NetworkContentElement operator +(NetworkContentElement content1, NetworkContentElement content2)
        {
            foreach(string key in content2.fields.Keys)
            {
                content1[key] = content2[key];
            }

            return content1;
        }
    }
}
