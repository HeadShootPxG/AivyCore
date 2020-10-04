using AivyData.Entities;
using AivyData.Enums;
using AivyDomain.Repository.Client;
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
        public readonly ClientRepository _client_repository;
        public readonly ClientCreatorRequest _client_creator;
        public readonly ClientLinkerRequest _client_linker;
        public readonly ClientConnectorRequest _client_connector;
        public readonly ClientDisconnectorRequest _client_disconnector;
        public readonly ClientSenderRequest _client_sender;
        public readonly ProxyTagEnum _tag;

        public virtual uint InstanceId => 0;

        public AbstractClientReceiveCallback(ClientEntity client, 
                                             ClientEntity remote, 
                                             ClientRepository repository,
                                             ClientCreatorRequest creator,
                                             ClientLinkerRequest linker,
                                             ClientConnectorRequest connector,
                                             ClientDisconnectorRequest disconnector,
                                             ClientSenderRequest sender, ProxyTagEnum tag = ProxyTagEnum.UNKNOW) 
            : base(client)
        {
            _remote = remote;
            _tag = tag;

            _client_repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _client_creator = creator ?? throw new ArgumentNullException(nameof(creator));
            _client_linker = linker ?? throw new ArgumentNullException(nameof(linker));
            _client_connector = connector ?? throw new ArgumentNullException(nameof(connector));
            _client_disconnector = disconnector ?? throw new ArgumentNullException(nameof(disconnector));
            _client_sender = sender ?? throw new ArgumentNullException(nameof(sender));

            _constructor_handled();
        }

        protected virtual void _constructor_handled()
        {
            // to do
        }

        public override void Callback(IAsyncResult result)
        {
            _client.Socket = (Socket)result.AsyncState;

            int _rcv_len = _client.Socket.EndReceive(result, out SocketError errorCode);

            if (_rcv_len > 0)
            {
                if (_client.ReceiveBuffer != null) _client.ReceiveBuffer.Dispose();
                _client.ReceiveBuffer = new MemoryStream(_buffer, 0, _rcv_len);
            }

            if (errorCode == SocketError.Success && _client.IsRunning && _rcv_len > 0)
            {
                if(_client.ReceiveBuffer != null)
                    _rcv_action?.Invoke(_client.ReceiveBuffer);

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
            else
            {
                if(_remote is null)
                {
                    if (_client.IsRunning)
                    {
                        _client_disconnector.Handle(_client);
                    }
                }
                else if (_remote.IsRunning)
                {
                    if (_rcv_len > 0)
                        _rcv_action?.Invoke(_client.ReceiveBuffer);
                    else
                        _client_disconnector.Handle(_remote);
                }
            }
        }
    }
}
