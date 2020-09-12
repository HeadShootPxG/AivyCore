using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AivyDofus.Protocol.Elements
{
    [Flags]
    public enum ProtocolKeyEnum
    {
        None = 0b00,
        Messages = 0b01,
        Types = 0b10,
        MessagesAndTypes = Messages | Types
    }
}
