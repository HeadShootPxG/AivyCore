using AivyData.Entities;
using AivyData.Enums;
using AivyDofus.IO;
using AivyDofus.Protocol.Buffer;
using AivyDomain.Callback.Client;
using AivyDomain.UseCases.Client;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AivyDofus.Proxy.Callbacks
{
    public class DofusProxyClientReceiveCallback : ProxyClientReceiveCallback
    {
        static readonly Logger logger = LogManager.GetCurrentClassLogger();

        protected MessageBufferReader _buffer_reader;
        protected BigEndianReader _reader;

        public DofusProxyClientReceiveCallback(ClientEntity client, ClientEntity remote, ClientSenderRequest sender, ClientDisconnectorRequest disconnector, ProxyTagEnum tag = ProxyTagEnum.UNKNOW)
            : base(client, remote, sender, disconnector, tag)
        {
            _reader = new BigEndianReader();
            _rcv_action = OnReceive;
            if (tag is ProxyTagEnum.UNKNOW) throw new ArgumentNullException(nameof(tag));
            _buffer_reader = new MessageBufferReader(tag == ProxyTagEnum.Client);
        }

        private void OnReceive(MemoryStream stream)
        {
            using (BigEndianWriter writer = new BigEndianWriter())
            {
                if(_reader.BytesAvailable > 0)
                    writer.WriteBytes(_reader.Data);
                if(stream.Length > 0)
                    writer.WriteBytes(stream.ToArray());
                _reader = new BigEndianReader(writer.Data);
            }

            if (_buffer_reader.Build(_reader))
            {
                logger.Info($"data : {_buffer_reader.MessageId} - (n°{_buffer_reader.InstanceId}) - {_buffer_reader.Length} - {_buffer_reader.Data.Length}");

                _buffer_reader = null;
                _buffer_reader = new MessageBufferReader(_tag == ProxyTagEnum.Client);

                stream.Dispose();
                int remnant_len = (int)_reader.BytesAvailable;
                stream = new MemoryStream(remnant_len);
                stream.Write(_reader.ReadBytes(remnant_len), 0, remnant_len);

                _reader.Dispose();
                _reader = null;
                _reader = new BigEndianReader();

                OnReceive(stream);
            }
        }
    }
}
