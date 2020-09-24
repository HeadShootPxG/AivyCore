using AivyDofus.IO;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AivyDofus.Protocol.Buffer
{
    public class MessageBufferReader
    {
        #region Const Values
        public readonly bool ClientSide;
        #endregion

        #region Modifiable Values
        public int? Header { get; private set; }
        public uint? InstanceId { get; private set; }
        public int? Length { get; private set; }
        public byte[] FullPacket { get; private set; }   

        public long RemnantLength { get; private set; }
        public byte[] RemnantData { get; private set; }
        #endregion

        #region From Modifiable Values 
        public ushort? MessageId
        {
            get
            {
                if (Header.HasValue)
                    return (ushort)(Header >> 2);
                return null;
            }
        }

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
            get { return _data?.Data ?? null; }
        }

        public byte[] TrueFullPacket
        {
            get
            {
                using (BigEndianWriter writer = new BigEndianWriter())
                {
                    writer.WriteUnsignedShort((ushort)Header);
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
                    return writer.Data;
                }
            }
        }

        public int NonDataLength => sizeof(short) + (ClientSide ? sizeof(uint) : 0) + LengthBytesCount ?? 0;
        public int TruePacketCountLength => NonDataLength + Length.Value;
        public int TruePacketCurrentLen => NonDataLength + Data.Length;
        #endregion        

        private BigEndianReader _data { get; set; }

        public MessageBufferReader(bool clientSide)
        {
            ClientSide = clientSide;

            Header = null;
            InstanceId = null;
            Length = null;
            FullPacket = null;
            _data = new BigEndianReader();
        }

        public bool Build(BigEndianReader reader)
        {
            FullPacket = reader.Data;

            if (IsValid)
                return true;

            if (reader.BytesAvailable >= 2 && !Header.HasValue)
                Header = reader.ReadUnsignedShort();

            if (ClientSide && !InstanceId.HasValue)
                InstanceId = reader.ReadUnsignedInt();

            if(LengthBytesCount.HasValue 
            && !Length.HasValue)
            {
                switch (LengthBytesCount)
                {
                    case 0: Length = 0; break;
                    case 1: Length = reader.ReadUnsignedByte(); break;
                    case 2: Length = reader.ReadUnsignedShort(); break;
                    case 3: Length = ((reader.ReadByte() & 255) << 16) + ((reader.ReadByte() & 255) << 8) + (reader.ReadByte() & 255); break;
                    default: throw new ArgumentOutOfRangeException(nameof(Length)); 
                }
            }

            //LogManager.GetCurrentClassLogger().Info($"is_valid : '{IsValid}' - len : '{Length}({LengthBytesCount})' - eq : '{Data.Length} == {Length} ? {Data.Length == Length}'");
            if(Data is null && Length.HasValue)
            {
                if (Length == 0)
                    _data = new BigEndianReader();
                if (reader.BytesAvailable >= Length)
                    _data = new BigEndianReader(reader.ReadBytes(Length.Value));
                else if (Length > reader.BytesAvailable)
                    _data = new BigEndianReader(reader.ReadBytes((int)reader.BytesAvailable));
            }

            if(Data != null && Data.Length < Length)
            {
                int bytesToRead = 0;
                if (Data.Length + reader.BytesAvailable < Length)
                    bytesToRead = (int)reader.BytesAvailable;
                else if (Data.Length + reader.BytesAvailable >= Length)
                {
                    bytesToRead = Length.Value - Data.Length;
                    RemnantLength = (Data.Length + reader.BytesAvailable) - Length.Value;
                }

                if (bytesToRead > 0)
                {
                    _data.Add(reader.ReadBytes(bytesToRead), 0, bytesToRead);
                    if(RemnantLength > 0)
                    {
                        RemnantData = reader.ReadBytes((int)RemnantLength);
                    }
                }
            }

            if(Data is null)
            {
                _data = new BigEndianReader(FullPacket);
            }

            /*(Data is null && Length.HasValue)
            {
                if (Length == 0)
                    Data = new byte[0];
                if (reader.BytesAvailable >= Length)                
                    Data = reader.ReadBytes(Length.Value);                
                else if (Length > reader.BytesAvailable)                
                    Data = reader.ReadBytes((int)reader.BytesAvailable);                
            }

            if (Data != null && Length.HasValue && Data.Length < Length)
            {
                int bytesToRead = 0;
                if (Data.Length + reader.BytesAvailable < Length)
                    bytesToRead = (int)reader.BytesAvailable;
                else if (Data.Length + reader.BytesAvailable >= Length)
                    bytesToRead = Length.Value - Data.Length;

                if (bytesToRead != 0)
                {
                    int oldLength = Data.Length;
                    Array.Resize(ref _data, Data.Length + bytesToRead);
                    Array.Copy(reader.ReadBytes(bytesToRead), 0, Data, oldLength, bytesToRead);
                }
            }*/

            return IsValid;
        }

        ~MessageBufferReader()
        {
            Header = null;
            InstanceId = null;
            Length = null;
            FullPacket = null;
            RemnantData = null;
            _data.Dispose();
            _data = null;
        }
    }
}
