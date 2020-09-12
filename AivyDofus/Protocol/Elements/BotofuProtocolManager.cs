using AivyDofus.Protocol.Parser;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AivyDofus.Protocol.Elements
{
    public class BotofuProtocolManager
    {
        static readonly object _protocol_locker = new object();

        private static BotofuProtocol _protocol = null;
        public static BotofuProtocol Protocol
        {
            get
            {
                lock (_protocol_locker)
                {
                    if (!File.Exists(BotofuParser._output_path))
                        throw new FileNotFoundException("protocol file was not found. Make sure you parsed it.", BotofuParser._output_path);

                    if (_protocol is null)
                        _protocol = JsonConvert.DeserializeObject<BotofuProtocol>(File.ReadAllText(BotofuParser._output_path), new JsonSerializerSettings() { Formatting = Formatting.Indented });
                    return _protocol;
                }
            }
        }
    }
}
