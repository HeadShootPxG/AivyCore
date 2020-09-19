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

        protected MessageBufferReader _buffer_reader;        
        protected MessageDataBufferReader _data_buffer_reader;
        protected MessageBufferWriter _buffer_writer;

        protected MessageHandler<ServerHandlerAttribute> _handler;
        protected BigEndianReader _reader;

        public override uint _instance_id => _buffer_reader.InstanceId.HasValue ? _buffer_reader.InstanceId.Value : base._instance_id;

        private readonly NetworkContentElement _protocol_required_message = new NetworkContentElement()
        {
            fields =
            {
                { "currentVersion", BotofuProtocolManager.Protocol.version.protocol.version.current },
                { "requiredVersion", BotofuProtocolManager.Protocol.version.protocol.version.minimum }
            }
        };
        private readonly NetworkElement _protocol_required = BotofuProtocolManager.Protocol[ProtocolKeyEnum.Messages, x => x.protocolID == 1];

        private readonly NetworkContentElement _hello_game_message = new NetworkContentElement();
        private readonly NetworkElement _hello_game = BotofuProtocolManager.Protocol[ProtocolKeyEnum.Messages, x => x.protocolID == 101];

        public DofusServerClientReceiveCallback(ClientEntity client, ClientCreatorRequest creator, ClientLinkerRequest linker, ClientConnectorRequest connector, ClientDisconnectorRequest disconnector, ClientSenderRequest sender)
            : base(client, null, creator, linker, connector, disconnector, sender, ProxyTagEnum.Client)
        {
            _buffer_reader = new MessageBufferReader(true);
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

        public override void Callback(IAsyncResult result)
        {
            _client.Socket = (Socket)result.AsyncState;
            _client.LastTimeMessage = DateTime.Now;
            int _rcv_len = _client.Socket.EndReceive(result, out SocketError errorCode);

            if (_rcv_len > 0 && errorCode == SocketError.Success)
            {
                _client.ReceiveBuffer = new MemoryStream();
                _client.ReceiveBuffer.Write(_buffer, 0, _rcv_len);

                MemoryStream _new_stream = _rcv_action?.Invoke(_client.ReceiveBuffer);

                if (_new_stream != null)
                {
                    _reader.Dispose();
                    _reader = new BigEndianReader();
                    _buffer_reader = new MessageBufferReader(_buffer_reader.ClientSide);
                }

                _client.ReceiveBuffer.Dispose();

                _buffer = new byte[_client.ReceiveBufferLength];

                try
                {
                    _client.Socket.BeginReceive(_buffer,
                                                0,
                                                _buffer.Length,
                                                SocketFlags.None,
                                                Callback,
                                                _client.Socket);
                }
                catch (SocketException e)
                {
                    logger.Error(e);
                }
            }
            else
            {
                _client_disconnector.Handle(_client);
            }
        }

        private MemoryStream OnReceive(MemoryStream stream)
        {
            _reader.Add(stream.ToArray(), 0, (int)stream.Length);

            if (_buffer_reader.Build(_reader))
            {
                byte[] full_data = _reader.Data;
                if (BotofuProtocolManager.Protocol[ProtocolKeyEnum.Messages, x => x.protocolID == _buffer_reader.MessageId] is NetworkElement element
                    && _buffer_reader.TruePacketCurrentLen == _buffer_reader.TruePacketCountLength)
                {
                    byte[] base_data = new byte[_buffer_reader.TruePacketCountLength];
                    // remove/set commentary to unsee/see message
                    logger.Info($"{_tag} {element.BasicString} - (l:{base_data.Length})");
                    byte[] remnant = new byte[full_data.Length - _buffer_reader.TruePacketCountLength];

                    Array.Copy(full_data, 0, base_data, 0, base_data.Length);
                    Array.Copy(full_data, base_data.Length, remnant, 0, remnant.Length);

                    if (_handler.GetHandler(element.protocolID))
                    {
                        _data_buffer_reader = new MessageDataBufferReader(element);

                        if (!_handler.Handle(this, element, _data_buffer_reader.Parse(new BigEndianReader(_buffer_reader.Data))))
                        {
                            if (remnant.Length > 0)
                            {
                                _reader.Dispose();
                                _reader = new BigEndianReader();
                                _buffer_reader = new MessageBufferReader(_buffer_reader.ClientSide);

                                return OnReceive(new MemoryStream(remnant));
                            }

                            return new MemoryStream();
                        }
                    }

                    if (remnant.Length > 0)
                    {                        
                        _reader.Dispose();
                        _reader = new BigEndianReader();
                        _buffer_reader = new MessageBufferReader(_buffer_reader.ClientSide);

                        return OnReceive(new MemoryStream(remnant));
                    }
                }

                return new MemoryStream(full_data);
            }

            return null;
        }
    }    
}
