using AivyData.Entities;
using AivyDomain.UseCases.Client;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace AivyDomain.Callback.Client
{
    public class ProxyClientReceiveCallback : ClientReceiveCallback
    {
        static readonly Logger logger = LogManager.GetCurrentClassLogger();

        protected readonly ClientEntity _remote;
        protected readonly ClientSenderRequest _client_sender;
        protected readonly ClientDisconnectorRequest _client_disconnector;
        protected readonly string _tag;

        public ProxyClientReceiveCallback(ClientEntity client, ClientEntity remote, ClientSenderRequest sender, ClientDisconnectorRequest disconnector, string tag = "") 
            : base(client)
        {
            _remote = remote ?? throw new ArgumentNullException(nameof(remote));
            _client_sender = sender ?? throw new ArgumentNullException(nameof(sender));
            _tag = tag ?? throw new ArgumentNullException(nameof(tag));
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

                    _client_sender.Handle(_remote, _client.ReceiveBuffer.ToArray());

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
                    catch(SocketException)
                    {
                        _client_disconnector.Handle(_remote);
                    }
                }
            }            
            else
            {
                if(_remote.IsRunning)
                    _client_disconnector.Handle(_remote);
            }
        }
    }
}
