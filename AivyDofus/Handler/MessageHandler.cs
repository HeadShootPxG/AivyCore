using AivyData.Entities;
using AivyDofus.Extension;
using AivyDofus.Protocol.Elements;
using AivyDofus.Proxy.Handlers;
using AivyDomain.Callback.Client;
using AivyDomain.UseCases.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace AivyDofus.Handler
{
    public class MessageHandler<Attribute> where Attribute : HandlerAttribute 
    {
        protected static readonly object _lock = new object();
        protected readonly IEnumerable<Type> _handlers_type;

        public MessageHandler()
        {
            lock (_lock)
            {
                lock (_lock)
                {
                    if (_handlers_type is null)
                    {
                        try
                        {
                            List<Type> handlers_type = new List<Type>();

                            handlers_type = Assembly.GetEntryAssembly()
                                                     .GetTypes()
                                                     .Where(x => x.GetCustomAttribute<Attribute>() != null
                                                              && x.IsSubclassOf(typeof(AbstractMessageHandler))
                                                              && !x.IsAbstract).ToList();
                            handlers_type.AddRange(Assembly.GetCallingAssembly()
                                                     .GetTypes()
                                                     .Where(x => x.GetCustomAttribute<Attribute>() != null
                                                              && x.IsSubclassOf(typeof(AbstractMessageHandler))
                                                              && !x.IsAbstract));
                            handlers_type.AddRange(Assembly.GetExecutingAssembly()
                                                     .GetTypes()
                                                     .Where(x => x.GetCustomAttribute<Attribute>() != null
                                                              && x.IsSubclassOf(typeof(AbstractMessageHandler))
                                                              && !x.IsAbstract));

                            _handlers_type = handlers_type.Distinct().Where(x => x != null);
                        }
                        catch (ReflectionTypeLoadException error)
                        {
                            _handlers_type = error.Types.Where(x => x != null);
                        }                        
                    }
                }
            }
        }

        public async Task<bool> Handle(AbstractClientReceiveCallback callback, NetworkElement element, NetworkContentElement content)
        {
            IEnumerable<Type> _handlers = _handlers_type.Where(x => x.GetCustomAttribute<Attribute>().BaseMessage.protocolID == element.protocolID);
            bool _all_true = true;
            foreach (Type _handler in _handlers)
                _all_true = _all_true && await _handle(_handler, callback, element, content);
            return _all_true;
        }

        private async Task<bool> _handle(Type handler_type, AbstractClientReceiveCallback callback, NetworkElement element, NetworkContentElement content)
        {
            if (!GetHandler(element.protocolID)) return true;
            if (handler_type is null) return true;

            AbstractMessageHandler handler = Activator.CreateInstance(handler_type, new object[] { callback, element, content }) as AbstractMessageHandler;

            await AsyncExtension.ExecuteAsync(handler.Handle, handler.EndHandle, handler.Error); 

            return handler.IsForwardingData;
        }

        public bool GetHandler(int protocolId)
        {
            return _handlers_type.FirstOrDefault(x => x.GetCustomAttribute<Attribute>().BaseMessage.protocolID == protocolId) != null;
        }

        public bool GetHandler(string protocolName)
        {
            return _handlers_type.FirstOrDefault(x => x.GetCustomAttribute<Attribute>().BaseMessage.name == protocolName) != null;
        }
    }
}
