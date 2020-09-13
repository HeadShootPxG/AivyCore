using AivyData.Entities;
using AivyDofus.Protocol.Elements;
using AivyDomain.UseCases.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AivyDofus.Handler
{
    public class MessageHandler<Attribute> where Attribute : HandlerAttribute
    {
        protected static readonly object _lock = new object();
        protected static IEnumerable<Type> _handlers_type;

        public MessageHandler()
        {
            lock (_lock)
            {
                if (_handlers_type is null)
                {
                    _handlers_type = Assembly.GetEntryAssembly()
                                             .GetTypes()
                                             .Where(x => x.GetCustomAttribute<Attribute>() != null
                                                      && x.IsSubclassOf(typeof(AbstractMessageHandler))
                                                      && !x.IsAbstract);
                }
            }
        }

        public void Handle(ClientSenderRequest sender, NetworkElement message, NetworkContentElement content, ClientEntity client, ClientEntity remote = null)
        {
            Type handler_type = _handlers_type.FirstOrDefault(x => x.GetCustomAttribute<Attribute>().BaseMessage.protocolID == message.protocolID);

            if (handler_type is null) return;

            AbstractMessageHandler handler = remote is null ? 
                                             Activator.CreateInstance(handler_type, new object[] { sender, message, content, client }) as AbstractMessageHandler : 
                                             Activator.CreateInstance(handler_type, new object[] { sender, message, content, client, remote }) as AbstractMessageHandler;

            try
            {
                Task.Run(handler.Handle).ContinueWith(task =>
                {
                    handler.EndHandle();
                });
            }
            catch(Exception e)
            {
                handler.Error(e);
            }
        }
    }
}
