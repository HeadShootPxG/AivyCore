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
        public readonly ClientSenderRequest _client_sender;
        public readonly ClientDisconnectorRequest _client_disconnector;
        public readonly ProxyTagEnum _tag;

        public virtual uint _instance_id => 0;

        public AbstractClientReceiveCallback(ClientEntity client, ClientEntity remote, ClientSenderRequest sender, ClientDisconnectorRequest disconnector, ProxyTagEnum tag = ProxyTagEnum.UNKNOW) 
            : base(client)
        {
            _client_sender = sender ?? throw new ArgumentNullException(nameof(sender));
            _remote = remote;
            _tag = tag;
            _client_disconnector = disconnector ?? throw new ArgumentNullException(nameof(disconnector));
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

                    _rcv_action?.Invoke(_client.ReceiveBuffer);

                    if (_remote.IsRunning) 
                        _client_sender.Handle(_remote, _client.ReceiveBuffer.ToArray());

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
    }
}
