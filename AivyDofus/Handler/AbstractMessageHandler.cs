using AivyData.Entities;
using AivyDofus.Protocol.Buffer;
using AivyDofus.Protocol.Elements;
using AivyDomain.UseCases.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AivyDofus.Handler
{
    public abstract class AbstractMessageHandler : IMessageHandler
    {
        protected readonly ClientSenderRequest _sender_request;
        
        protected readonly ClientEntity _client;
        protected readonly ClientEntity _remote;

        protected readonly NetworkElement _message;
        protected readonly NetworkContentElement _content;

        public AbstractMessageHandler(ClientSenderRequest sender,
                                      NetworkElement message,
                                      NetworkContentElement content,
                                      ClientEntity client,
                                      ClientEntity remote = null)
        {
            _sender_request = sender ?? throw new ArgumentNullException(nameof(sender));

            _message = message ?? throw new ArgumentNullException(nameof(message));
            _content = content ?? throw new ArgumentNullException(nameof(content));

            _client = client ?? throw new ArgumentNullException(nameof(client));
            _remote = remote;
        }

        public virtual void EndHandle()
        {

        }

        public virtual void Error(Exception e)
        {

        }

        public abstract void Handle();
    }
}
