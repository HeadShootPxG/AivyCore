using AivyData.Entities;
using AivyData.Enums;
using AivyDofus.Handler;
using AivyDofus.IO;
using AivyDofus.Protocol.Buffer;
using AivyDofus.Protocol.Elements;
using AivyDofus.Server.Handlers;
using AivyDomain.Callback.Client;
using AivyDomain.UseCases.Client;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AivyDofus.Server.Callbacks
{
    public class DofusServerClientReceiveCallback : AbstractClientReceiveCallback
    {
        static readonly Logger logger = LogManager.GetCurrentClassLogger();

        protected MessageDataBufferReader _data_buffer_reader;
        protected MessageBufferWriter _buffer_writer;

        protected MessageHandler<ServerHandlerAttribute> _handler;
        protected BigEndianReader _reader;

        private readonly NetworkContentElement _protocol_required_message = new NetworkContentElement()
        {
            fields =
            {
                { "version", "1.0.1-ad7c4fd" }
            }
        };

        private readonly NetworkElement _protocol_required = BotofuProtocolManager.Protocol[ProtocolKeyEnum.Messages, x => x.name == "ProtocolRequired"];

        private readonly NetworkContentElement _hello_game_message = new NetworkContentElement();
        private readonly NetworkElement _hello_game = BotofuProtocolManager.Protocol[ProtocolKeyEnum.Messages, x => x.name == "HelloGameMessage"];

        public DofusServerClientReceiveCallback(ClientEntity client, ClientCreatorRequest creator, ClientLinkerRequest linker, ClientConnectorRequest connector, ClientDisconnectorRequest disconnector, ClientSenderRequest sender)
            : base(client, null, creator, linker, connector, disconnector, sender, ProxyTagEnum.Client)
        {
            _buffer_writer = new MessageBufferWriter(false);
            _handler = new MessageHandler<ServerHandlerAttribute>();

            _rcv_action += OnReceive;
            _reader = new BigEndianReader();

            if (_client.IsRunning)
            {
                // send protocolRequired
                using (BigEndianWriter _writer = _buffer_writer.Build((ushort)_protocol_required.protocolID, null, new MessageDataBufferWriter(_protocol_required).Parse(_protocol_required_message)))
                {
                    _client_sender.Handle(_client, _writer.Data);
                }
                // send helloGameMessage
                using (BigEndianWriter _writer = _buffer_writer.Build((ushort)_hello_game.protocolID, null, new MessageDataBufferWriter(_hello_game).Parse(_hello_game_message)))
                {
                    _client_sender.Handle(_client, _writer.Data);
                }
            }
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
                _reader.Add(stream.ToArray(), 0, (int)stream.Length);
            }

            byte[] full_data = _reader.Data;
            while (_position < full_data.Length && full_data.Length - _position >= 2)
            {
                long start_pos = _position;
                _current_header = (ushort)((full_data[_position] * 256) + full_data[_position + 1]);
                _position += sizeof(ushort);

                _instance_id = (uint)((full_data[_position] * 256 * 256 * 256) + (full_data[_position + 1] * 256 * 256) + (full_data[_position + 2] * 256) + full_data[_position + 3]);
                _position += sizeof(uint);


                if (full_data.Length - _position < _static_header)
                    break;

                switch (_static_header)
                {
                    case 0: _length = 0; break;
                    case 1: _length = full_data[_position]; break;
                    case 2: _length = (ushort)((full_data[_position] * 256) + full_data[_position + 1]); break;
                    case 3: _length = (full_data[_position] * 256 * 256) + (full_data[_position + 1] * 256) + full_data[_position + 2]; break;
                }

                _position += _static_header;

                long _current_data_len = full_data.Length - _position;
                if (_current_data_len >= _length)
                {
                    NetworkElement _element = BotofuProtocolManager.Protocol[ProtocolKeyEnum.Messages, x => x.protocolID == _message_id];
                    _data = new byte[_current_data_len];

                    byte[] packet_data = new byte[(int)(_position - start_pos) + _length.Value];
                    Array.Copy(full_data, start_pos, packet_data, 0, packet_data.Length);
                    Array.Copy(full_data, _position, _data, 0, _length.Value);

                    if (_element != null)
                    {
                        logger.Info($"[{_tag}] {_element.BasicString}");
                        _data_buffer_reader = new MessageDataBufferReader(_element);
                        using (BigEndianReader big_data_reader = new BigEndianReader(_data))
                        {
                            if (_handler.Handle(this, _element, _data_buffer_reader.Parse(big_data_reader)))
                            {
                                // to do
                            }
                        }
                    }

                    _position += _length.Value;

                    if (_current_data_len == _length)
                    {
                        _clear();

                        _reader.Dispose();
                        _reader = new BigEndianReader();
                        _position = 0;
                        break;
                    }
                    _clear();
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
