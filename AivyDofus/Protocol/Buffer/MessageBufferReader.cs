using AivyDofus.IO;
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

        public bool IsGameMessage
        {
            get
            {
                try 
                {
                    return IsValid && FullPacket.Length <= TruePacketCurrentLen && NonDataLength <= NonDataLength + Data.Length;
                }
                catch 
                { 
                    return false;
                }
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

        public MessageBufferReader(bool clientSide)
        {
            ClientSide = clientSide;
        }

        public bool Build(BigEndianReader reader)
        {
            FullPacket = reader.Data;

            if (IsValid)
                return true;

            if (reader.BytesAvailable >= 2 && !Header.HasValue)            
                Header = reader.ReadShort();

            if (ClientSide)
                InstanceId = reader.ReadUnsignedInt();

            if(LengthBytesCount.HasValue &&
                reader.BytesAvailable >= LengthBytesCount && !Length.HasValue)
            {
                Length = 0;
                for (int i = LengthBytesCount.Value - 1; i >= 0; i--)
                {
                    Length |= reader.ReadByte() << (i * 8);
                }
            }

            if(Data is null && Length.HasValue)
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
            }

            return IsValid;
        }

        ~MessageBufferReader()
        {
            Header = null;
            InstanceId = null;
            Length = null;
            FullPacket = null;
            Data = null;
        }
    }
}
