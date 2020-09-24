using AivyData.Entities;
using AivyData.Enums;
using AivyDofus.Handler;
using AivyDofus.IO;
using AivyDofus.Protocol.Buffer;
using AivyDofus.Protocol.Elements;
using AivyDofus.Proxy.Handlers;
using AivyDomain.Callback.Client;
using AivyDomain.UseCases.Client;
using Newtonsoft.Json.Serialization;
using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

namespace AivyDofus.Proxy.Callbacks
{
    public class DofusProxyClientReceiveCallback : AbstractClientReceiveCallback
    {
        static readonly Logger logger = LogManager.GetCurrentClassLogger();

        protected MessageBufferReader _buffer_reader;
        protected MessageDataBufferReader _data_buffer_reader;
        protected MessageBufferWriter _buffer_writer;

        protected MessageHandler<ProxyHandlerAttribute> _handler;
        protected BigEndianReader _reader;

        public readonly ProxyEntity _proxy;

        public DofusProxyClientReceiveCallback(ClientEntity client,
                                               ClientEntity remote,
                                               ClientCreatorRequest creator,
                                               ClientLinkerRequest linker,
                                               ClientConnectorRequest connector, 
                                               ClientDisconnectorRequest disconnector, 
                                               ClientSenderRequest sender,
                                               ProxyEntity proxy, ProxyTagEnum tag = ProxyTagEnum.UNKNOW)
            : base(client, remote, creator, linker, connector, disconnector, sender, tag)
        {
            if (tag is ProxyTagEnum.UNKNOW) throw new ArgumentNullException(nameof(tag));

            _proxy = proxy ?? throw new ArgumentNullException(nameof(proxy));

            _buffer_reader = new MessageBufferReader(tag == ProxyTagEnum.Client);
            _handler = new MessageHandler<ProxyHandlerAttribute>();

            _rcv_action += OnReceive;
        }

        private ushort? _current_header { get; set; } = null;
        private uint? _instance_id { get; set; } = null;
        private int? _length { get; set; } = null;
        private byte[] _data { get; set; } = null;

        private int _message_id => _current_header.HasValue ? _current_header.Value >> 2 : 0;
        private int _static_header => _current_header.HasValue ? _current_header.Value & 3 : 0;

        private void _clear()
        {
            _current_header = null;
            _instance_id = null;
            _length = null;
            _data = null;
        }

        private long _position { get; set; } = 0;
        /// <summary>
        /// thx to Hitman for this implementation ;)
        /// </summary>
        /// <param name="stream"></param>
        private void OnReceive(MemoryStream stream)
        {
            if (_reader is null) _reader = new BigEndianReader();
            if (stream.Length > 0)
            {
                _client_sender.Handle(_remote, stream.ToArray());
                _reader.Add(stream.ToArray(), 0, (int)stream.Length);
            }

            byte[] full_data = _reader.Data;
            while (_position < full_data.Length && full_data.Length >= 2)
            {
                long start_pos = _position;
                _current_header = (ushort)((full_data[_position] * 256) + full_data[_position + 1]);
                _position += sizeof(ushort);

                if(_tag == ProxyTagEnum.Client)
                {
                    _instance_id = (uint)((full_data[_position] * 256 * 256 * 256) + (full_data[_position + 1] * 256 * 256) + (full_data[_position + 2] * 256) + full_data[_position + 3]);
                    _position += sizeof(uint);
                }
                _position +=  _static_header;

                switch (_static_header)
                {
                    case 0: _length = 0; break;
                    case 1: _length = full_data[_position - 1]; break;
                    case 2: _length = (ushort)((full_data[_position - 2] * 256) + full_data[_position - 1]); break;
                    case 3: _length = (full_data[_position - 3] * 65536) + (full_data[_position - 2] * 256) + full_data[_position - 1]; break;
                }

                long _current_data_len = full_data.Length - _position;
                if(_current_data_len >= _length)
                {
                    NetworkElement _element = BotofuProtocolManager.Protocol[ProtocolKeyEnum.Messages, x => x.protocolID == _message_id];
                    _data = new byte[_current_data_len];

                    if(_tag == ProxyTagEnum.Client)
                    {
                        _proxy.LAST_CLIENT_INSTANCE_ID = _instance_id.Value;
                        _proxy.MESSAGE_RECEIVED_FROM_LAST = 0;
                    }
                    else
                    {
                        _proxy.MESSAGE_RECEIVED_FROM_LAST++;
                    }

                    Array.Copy(full_data, _position, _data, 0, _current_data_len);
                    if (_element != null)
                    {
                        logger.Info($"[{_tag}] msg: {_element.BasicString}");
                        _data_buffer_reader = new MessageDataBufferReader(_element);
                        if (_handler.GetHandler(_element.protocolID))
                        {
                            _handler.Handle(this, _element, _data_buffer_reader.Parse(new BigEndianReader(_data)));
                        }
                    }

                    _position += _length.Value;

                    _clear();

                    if(_current_data_len == _length)
                    {
                        _reader.Dispose();
                        _reader = new BigEndianReader();
                        _position = 0;
                        break;
                    }
                }
                else
                {
                    _position = start_pos; 
                    break;
                }                
            }
        }
    }
}


