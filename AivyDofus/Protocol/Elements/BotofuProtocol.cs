using AivyDofus.Protocol.Elements.Fields;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AivyDofus.Protocol.Elements
{
    public class BotofuProtocol
    {
        public EnumField[] enumerations { get; set; }
        public NetworkElement[] messages { get; set; }
        public NetworkElement[] types { get; set; }
        public Version version { get; set; }

        public NetworkElement this[ProtocolKeyEnum key, Func<NetworkElement, bool> predicat]
        {
            get
            {
                if ((key & ProtocolKeyEnum.Messages) == ProtocolKeyEnum.Messages && messages.FirstOrDefault(predicat) is NetworkElement message)
                    return message;

                if ((key & ProtocolKeyEnum.Types) == ProtocolKeyEnum.Types && types.FirstOrDefault(predicat) is NetworkElement type)
                    return type;

                return null;
            }
        }

        public EnumField this[Func<EnumField, bool> predicat]
        {
            get
            {
                return enumerations.FirstOrDefault(predicat);
            }
        }
    }
}
