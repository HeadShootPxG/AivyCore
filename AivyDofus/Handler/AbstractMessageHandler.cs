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
        static readonly Logger logger = LogManager.GetCurrentClassLogger();

        protected readonly ProxyClientReceiveCallback _callback;
        protected readonly NetworkElement _element;
        protected readonly NetworkContentElement _content;

        public abstract bool IsForwardingData { get; }

        public AbstractMessageHandler(ProxyClientReceiveCallback callback, NetworkElement element, NetworkContentElement content)
        {
            _callback = callback ?? throw new ArgumentNullException(nameof(callback));
            _element = element ?? throw new ArgumentNullException(nameof(element));
            _content = content ?? throw new ArgumentNullException(nameof(content));
        }

        public virtual void EndHandle()
        {

        }

        public virtual void Error(Exception e)
        {

        }

        public abstract void Handle();

        public virtual void Send(bool fromClient, ClientEntity sender, NetworkElement element, NetworkContentElement content, uint? instance_id = null)
        {
            if (fromClient && instance_id is null) throw new ArgumentNullException(nameof(instance_id));

            byte[] _data = new MessageDataBufferWriter(element).Parse(content);
            byte[] _final_data = new MessageBufferWriter(fromClient).Build((ushort)element.protocolID, instance_id, _data).Data;

            _callback._client_sender.Handle(sender, _final_data);

            logger.Info($"data sent : {_final_data.Length}");
        }
    }
}
