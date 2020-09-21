using AivyData.Entities;
using AivyData.Enums;
using AivyDofus.Protocol.Buffer;
using AivyDofus.Protocol.Elements;
using AivyDofus.Proxy;
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

        protected readonly AbstractClientReceiveCallback _callback;
        protected readonly NetworkElement _element;
        protected readonly NetworkContentElement _content;

        public abstract bool IsForwardingData { get; }

        public AbstractMessageHandler(AbstractClientReceiveCallback callback, NetworkElement element, NetworkContentElement content)
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

        public virtual void Send(bool fromClient, ClientEntity sender, NetworkElement element, NetworkContentElement content, uint? instance_id = null, bool fake_msg = false)
        {
            if (fromClient && instance_id is null) 
            {
                throw new ArgumentNullException(nameof(instance_id));
            }

            if (fake_msg)
            {
                DofusProxy.FAKE_MESSAGE_SENT++;
            }

            byte[] _data = new MessageDataBufferWriter(element).Parse(content);
            byte[] _final_data = new MessageBufferWriter(fromClient).Build((ushort)element.protocolID, instance_id, _data).Data;

            _callback._client_sender.Handle(sender, _final_data);

            logger.Info($"fake message sent : {element.BasicString} {instance_id.Value}");
        }

        #region FAST ACTIONS
        public void FromClientSendPublicMessage(byte channel, string content)
        {
            ClientEntity _client = null;
            if(_callback._tag == ProxyTagEnum.Client)            
                _client = _callback._remote;
            if (_callback._tag == ProxyTagEnum.Server)
                _client = _callback._client;

            NetworkElement element = BotofuProtocolManager.Protocol[ProtocolKeyEnum.Messages, x => x.protocolID == 861];
            NetworkContentElement element_content = new NetworkContentElement()
            {
                fields =
                {
                    { "channel", channel },
                    { "content", content }
                }
            };

            Send(true, _client, element, element_content, DofusProxy.GLOBAL_INSTANCE_ID + 1, true);
        }
        #endregion
    }
}
