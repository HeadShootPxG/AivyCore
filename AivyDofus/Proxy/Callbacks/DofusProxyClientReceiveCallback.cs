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

        public override uint _instance_id => _buffer_reader.InstanceId.HasValue ? _buffer_reader.InstanceId.Value : base._instance_id;

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
            _reader = new BigEndianReader();
        }

        private string _hex_string(byte[] bytes, bool lower = true, bool space = true)
        {
            string result = BitConverter.ToString(bytes).Replace("-", space ? " " : "");
            return lower ? result.ToLower() : result.ToUpper();
        }

        private MemoryStream OnReceive(MemoryStream stream)
        {
            return stream;

            //logger.Info($"rcv : {_hex_string(stream.ToArray())}");

            /*if (_buffer_reader.Build(_reader))
            {
                logger.Info($"builded id : {_buffer_reader.MessageId}");

                _reader.Dispose();
                _reader = new BigEndianReader();
                _buffer_reader = new MessageBufferReader(_buffer_reader.ClientSide);

                return new MemoryStream(_reader.Data);
            }
            else
            {
                _buffer_reader = new MessageBufferReader(_buffer_reader.ClientSide);
                return stream;
            }*/
            /*_reader.Add(stream.ToArray(), 0, (int)stream.Length);

            if(_buffer_reader.Build(_reader))
            {
                byte[] full_data = _reader.Data;

                if (BotofuProtocolManager.Protocol[ProtocolKeyEnum.Messages, x => x.protocolID == _buffer_reader.MessageId] is NetworkElement element)
                {
                    if (_buffer_reader.ClientSide)
                    {
                        _proxy.LAST_CLIENT_INSTANCE_ID = _buffer_reader.InstanceId.Value;
                        _proxy.MESSAGE_RECEIVED_FROM_LAST = 0;
                    }
                    else
                    {
                        _proxy.MESSAGE_RECEIVED_FROM_LAST += 1;
                    }
                    
                    byte[] base_data = new byte[_buffer_reader.TruePacketCountLength];
                    // remove/set commentary to unsee/see message
                    // logger.Info($"{_tag} {element.BasicString} - (l:{base_data.Length}) (id:{_buffer_reader.InstanceId} + {_proxy.FAKE_MESSAGE_CREATED} = {_buffer_reader.InstanceId + _proxy.FAKE_MESSAGE_CREATED}|{_proxy.GLOBAL_INSTANCE_ID})");
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

                    if(remnant.Length > 0 && _remote.IsRunning)
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

            return stream;*/
        }
    }
}


