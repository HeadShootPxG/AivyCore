using AivyDofus.Protocol.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AivyDofus.Handler
{
    [AttributeUsage(AttributeTargets.Class)]
    public abstract class HandlerAttribute : Attribute
    {
        public int ProtocolId { get; set; } = -1;
        public string ProtocolName { get; set; } = "";

        public HandlerAttribute()
        {
            
        }

        public NetworkElement BaseMessage
        {
            get
            {
                if (ProtocolId <= 0 && ProtocolName == "")
                    throw new ArgumentNullException("protocolId or protocolName is not assigned");

                if (ProtocolId > 0)
                    return BotofuProtocolManager.Protocol[ProtocolKeyEnum.Messages, x => x.protocolID == ProtocolId];

                return BotofuProtocolManager.Protocol[ProtocolKeyEnum.Messages, x => x.name == ProtocolName];
            }
        }
    }
}
