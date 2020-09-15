using AivyData.Entities;
using AivyData.Enums;
using AivyDofus.Protocol.Buffer;
using AivyDofus.Protocol.Elements;
using AivyDomain.Callback.Client;
using AivyDomain.UseCases.Client;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AivyDofus.Handler
{
    public abstract class AbstractMessageHandler : IMessageHandler
    {
        protected readonly ProxyClientReceiveCallback _callback;
        protected readonly NetworkElement _element;
        protected readonly NetworkContentElement _content;

        private readonly MessageBufferWriter _writer;
        private readonly MessageDataBufferWriter _data_writer;

        public abstract bool IsForwardingData { get; }

        public AbstractMessageHandler(ProxyClientReceiveCallback callback, NetworkElement element, NetworkContentElement content)
        {
            _callback = callback ?? throw new ArgumentNullException(nameof(callback));
            _element = element ?? throw new ArgumentNullException(nameof(element));
            _content = content ?? throw new ArgumentNullException(nameof(content));

            _writer = new MessageBufferWriter(_callback._tag == ProxyTagEnum.Client);
            _data_writer = new MessageDataBufferWriter(_element);
        }

        public virtual void EndHandle()
        {

        }

        public virtual void Error(Exception e)
        {

        }

        public abstract void Handle();

        public virtual void Send(bool client, NetworkContentElement content)
        {
            ClientEntity sender = client ? _callback._client : _callback._remote;

            byte[] _data = _data_writer.Parse(content);
            uint? _instance_id = null;
            /*if((_callback._tag == ProxyTagEnum.Client && !client)
             || _callback._tag == ProxyTagEnum.Server && client)
            {
                LogManager.GetCurrentClassLogger().Info("NOP");
                // to do
                _instance_id = 0;
            }*/
            byte[] _final_data = _writer.Build((ushort)_element.protocolID, _instance_id, _data).Data;

            _callback._client_sender.Handle(sender, _final_data);
        }
    }
}
