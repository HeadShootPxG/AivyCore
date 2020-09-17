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
                    return (MessageId << 2) | LengthBytesCount;
                return null;
            }
        }

        public int? LengthBytesCount
        {
            get
            {
                if (Length.HasValue)
                {
                    if (Length > ushort.MaxValue)
                        return 3;
                    if (Length > byte.MaxValue)
                        return 2;
                    if (Length > 0)
                        return 1;
                    return 0;
                }
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

        public int NonDataLength => sizeof(short) + (ClientSide ? sizeof(uint) : 0) + LengthBytesCount.Value;
        public int TruePacketCountLength => NonDataLength + Length.Value;
        public int TruePacketCurrentLen => NonDataLength + Data.Length;
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

            if (ClientSide && instanceId != null)
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
