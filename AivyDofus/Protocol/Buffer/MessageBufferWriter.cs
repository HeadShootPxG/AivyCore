using AivyDofus.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AivyDofus.Protocol.Buffer
{
    public class MessageBufferWriter
    {
        #region Const Values
        public readonly bool ClientSide;
        #endregion

        #region Modifiable Values
        public ushort? MessageId { get; private set; }
        public uint? InstanceId { get; private set; }
        public int? Length { get; private set; }
        #endregion

        #region From Modifiable Values 
        public int? Header
        {
            get
            {
                if (MessageId.HasValue)
                    return (MessageId << 2) | Length;
                return null;
            }
        }

        /*public byte[] CmpLength
        {
            get
            {
                using(BigEndianWriter writer = new BigEndianWriter())
                {
                    if (Length > 65535)
                    {
                        writer.WriteByte((byte)((Length >> 16) & 255));
                        writer.WriteShort((short)(Length & 65535));
                    }
                    else if (Length > 255)
                    {
                        writer.WriteShort((short)Length);
                    }
                    else if (Length > 0)
                    {
                        writer.WriteByte((byte)Length);
                    }
                    return writer.Data;
                }
            }
        }*/

        public int? LengthBytesCount
        {
            get
            {
                if (Header.HasValue)
                    return Header & 0x3;
                return null;
            }
        }

        public bool IsValid
        {
            get
            {
                return Header.HasValue && Length.HasValue &&
                       Length == Data.Length;
            }
        }

        public byte[] Data
        {
            get { return _data; }
            private set { _data = value; }
        }

        public int TruePacketCountLength => sizeof(short) + (ClientSide ? sizeof(uint) : 0) + LengthBytesCount.Value + Length.Value;
        public int TruePacketCurrentLen => sizeof(short) + (ClientSide ? sizeof(uint) : 0) + LengthBytesCount.Value + Data.Length;
        #endregion

        private byte[] _data;

        public MessageBufferWriter(bool clientSide)
        {
            ClientSide = clientSide;
        }

        public BigEndianWriter Build(ushort messageId, uint? instanceId, byte[] data)
        {
            BigEndianWriter writer = new BigEndianWriter();
            MessageId = messageId;
            InstanceId = instanceId;
            Data = data;
            Length = data.Length;

            writer.WriteShort(Header.Value);

            if (ClientSide)
                writer.WriteUnsignedInt(InstanceId.Value);

            switch (LengthBytesCount)
            {
                case 1:
                    writer.WriteByte((byte)Length);
                    break;
                case 2:
                    writer.WriteShort((short)Length);
                    break;
                case 3:
                    writer.WriteByte((byte)((Length >> 16) & 255));
                    writer.WriteShort((short)(Length & 65535));
                    break;
            }

            writer.WriteBytes(Data);

            return writer;
        }
    }
}
