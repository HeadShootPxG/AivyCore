using AivyData.Entities;
using AivyData.Enums;
using AivyDofus.Handler;
using AivyDofus.Protocol.Elements;
using AivyDofus.Protocol.Parser;
using AivyDomain.Callback.Client;
using AivyDomain.UseCases.Client;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace AivyDofus.Proxy.Handlers.Customs.Connection
{
    // remove commentary if you want to handle it
    [ProxyHandler(ProtocolId = 6253)]
    public class RawDataMessageHandler : AbstractMessageHandler
    {
        static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public override bool IsForwardingData => true;

        public RawDataMessageHandler(AbstractClientReceiveCallback callback, 
                                     NetworkElement element,
                                     NetworkContentElement content)
            : base(callback, element, content)
        {

        }

        public override void Handle()
        {
            dynamic content = _content["content"];
            byte[] _data = new byte[content.Length];
            Array.Copy(content, 0, _data, 0, _data.Length);
            File.WriteAllBytes(output_path, _data);
        }

        public override void Error(Exception e)
        {
            logger.Error(e);
        }

        private static readonly string output_path = Path.Combine(BotofuParser._this_executable_name, $"./rawdatamessage_rcv_{DateTime.Now.Ticks}.swf");
    }
}
