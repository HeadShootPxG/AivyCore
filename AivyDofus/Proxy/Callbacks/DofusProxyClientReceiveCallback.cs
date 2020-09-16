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
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace AivyDofus.Proxy.Callbacks
{
    public class DofusProxyClientReceiveCallback : ProxyClientReceiveCallback
    {
        static readonly Logger logger = LogManager.GetCurrentClassLogger();

        protected MessageBufferReader _buffer_reader;
        protected MessageDataBufferReader _data_buffer_reader;
        protected MessageBufferWriter _buffer_writer;

        protected MessageHandler<ProxyHandlerAttribute> _handler;
        protected BigEndianReader _reader;

        public override uint _instance_id => _buffer_reader.InstanceId.HasValue ? _buffer_reader.InstanceId.Value : base._instance_id; 

        public DofusProxyClientReceiveCallback(ClientEntity client, ClientEntity remote, ClientSenderRequest sender, ClientDisconnectorRequest disconnector, ProxyTagEnum tag = ProxyTagEnum.UNKNOW)
            : base(client, remote, sender, disconnector, tag)
        {
            if (tag is ProxyTagEnum.UNKNOW) throw new ArgumentNullException(nameof(tag));

            _buffer_reader = new MessageBufferReader(tag == ProxyTagEnum.Client);
            _handler = new MessageHandler<ProxyHandlerAttribute>();

            _rcv_action += OnReceive;
            _reader = new BigEndianReader();
        }

        public override void Callback(IAsyncResult result)
        {
            _client.Socket = (Socket)result.AsyncState;

            if (_client.IsRunning && _remote.IsRunning)
            {
                int _rcv_len = _client.Socket.EndReceive(result, out SocketError errorCode);

                if (_rcv_len > 0 && errorCode == SocketError.Success && _client.IsRunning && _remote.IsRunning)
                {
                    _client.ReceiveBuffer = new MemoryStream();
                    _client.ReceiveBuffer.Write(_buffer, 0, _rcv_len);

                    MemoryStream _new_stream = _rcv_action?.Invoke(_client.ReceiveBuffer);

                    if (_remote.IsRunning && _new_stream != null)
                    {
                        if(_new_stream.Length > 0)
                            _client_sender.Handle(_remote, _new_stream.ToArray());
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
                    catch (SocketException)
                    {
                        _client_disconnector.Handle(_remote);
                    }

                }
            }
            else
            {
                if (_remote.IsRunning)
                    _client_disconnector.Handle(_remote);
            }
        }

        private MemoryStream OnReceive(MemoryStream stream)
        {
            _reader.Add(stream.ToArray(), 0, (int)stream.Length);

            if(_buffer_reader.Build(_reader))
            {
                byte[] full_data = _reader.Data;
                if (BotofuProtocolManager.Protocol[ProtocolKeyEnum.Messages, x => x.protocolID == _buffer_reader.MessageId] is NetworkElement element
                    && _buffer_reader.TruePacketCurrentLen == _buffer_reader.TruePacketCountLength)
                {
                    // remove commentary to see message
                    byte[] base_data = new byte[_buffer_reader.TruePacketCountLength];
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

                    if(remnant.Length > 0)
                    {
                        _client_sender.Handle(_remote, base_data);
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


