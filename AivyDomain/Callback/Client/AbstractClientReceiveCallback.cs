using AivyData.Entities;
using AivyData.Enums;
using AivyDomain.UseCases.Client;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace AivyDomain.Callback.Client
{
    public class AbstractClientReceiveCallback : ClientReceiveCallback
    {
        static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public readonly ClientEntity _remote;
        public readonly ClientCreatorRequest _client_creator;
        public readonly ClientLinkerRequest _client_linker;
        public readonly ClientConnectorRequest _client_connector;
        public readonly ClientDisconnectorRequest _client_disconnector;
        public readonly ClientSenderRequest _client_sender;
        public readonly ProxyTagEnum _tag;

        public virtual uint _instance_id => 0;

        public AbstractClientReceiveCallback(ClientEntity client, 
                                             ClientEntity remote, 
                                             ClientCreatorRequest creator,
                                             ClientLinkerRequest linker,
                                             ClientConnectorRequest connector,
                                             ClientDisconnectorRequest disconnector,
                                             ClientSenderRequest sender, ProxyTagEnum tag = ProxyTagEnum.UNKNOW) 
            : base(client)
        {
            _remote = remote;
            _tag = tag;

            _client_creator = creator ?? throw new ArgumentNullException(nameof(creator));
            _client_linker = linker ?? throw new ArgumentNullException(nameof(linker));
            _client_connector = connector ?? throw new ArgumentNullException(nameof(connector));
            _client_disconnector = disconnector ?? throw new ArgumentNullException(nameof(disconnector));
            _client_sender = sender ?? throw new ArgumentNullException(nameof(sender));
        }

        public override void Callback(IAsyncResult result)
        {
            _client.Socket = (Socket)result.AsyncState;

            int _rcv_len = _client.Socket.EndReceive(result, out SocketError errorCode);

            if (_rcv_len > 0)
            {
                _client.ReceiveBuffer = new MemoryStream();
                _client.ReceiveBuffer.Write(_buffer, 0, _rcv_len);
            }

            if (errorCode == SocketError.Success && _client.IsRunning && _rcv_len > 0)
            {
                MemoryStream new_stream = _rcv_action?.Invoke(_client.ReceiveBuffer) ?? _client.ReceiveBuffer;

                if (_remote.IsRunning && new_stream != null && new_stream.Length > 0)
                    _client_sender.Handle(_remote, new_stream.ToArray());

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
                    _client_disconnector.Handle(_remote);
                }

            }
            else
            {
                if (_remote.IsRunning)
                {
                    if (_rcv_len > 0)
                        _client_sender.Handle(_remote, _client.ReceiveBuffer.ToArray());
                    else
                        _client_disconnector.Handle(_remote);
                }
            }

            _client.ReceiveBuffer?.Dispose();
            _client.ReceiveBuffer = null;
        }
    }
}
