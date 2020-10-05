<h2> AivyCore </h2>

AivyCore is a Network base with a Client, Server, Proxy.

The main purpose of this program is not to give you something you'll reuse without even knowing what it is about. Before you even look at the code, if you are a beginer I advise you to learn programming.

<h2> AivyDofus </h2>

AivyDofus is an implementation of AivyCore for Dofus ( www.dofus.com )

Example :

```csharp 
    class Program
    {
        static readonly ConsoleTarget log_console = new ConsoleTarget("log_console");
        static readonly LoggingConfiguration configuration = new LoggingConfiguration();
        static readonly Logger logger = LogManager.GetCurrentClassLogger();

        static OpenProxyApi _proxy_api;
        static ProxyEntityMapper _proxy_mapper;
        static ProxyRepository _proxy_repository;

        static ProxyCreatorRequest _proxy_creator;
        static ProxyActivatorRequest _proxy_activator;

        static void Main(string[] args)
        {
            configuration.AddRule(LogLevel.Info, LogLevel.Fatal, log_console);
            LogManager.Configuration = configuration;

            _proxy_api = new OpenProxyApi("./proxy_information_api.json");
            _proxy_mapper = new ProxyEntityMapper();
            _proxy_repository = new ProxyRepository(_proxy_api, _proxy_mapper);

            _proxy_creator = new ProxyCreatorRequest(_proxy_repository);
            _proxy_activator = new ProxyActivatorRequest(_proxy_repository);

            ProxyEntity proxy = _proxy_creator.Handle(@"YOUR EXECUTABLE FILE", 666);
            proxy = _proxy_activator.Handle(proxy, true);

            Console.ReadLine();
        }
    }
```

Proxy : https://github.com/Mrpotatosse/AivyCore/blob/master/AivyDofus/Proxy/DofusProxy.cs

Server : https://github.com/Mrpotatosse/AivyCore/blob/master/AivyDofus/Server/DofusServer.cs

Has you could see , the proxy can handle Dofus 2.0 and Dofus Retro ( but for Dofus Retro there isn't packet reading , but I just had a little script to let you set your logging information with code, everything is here : https://github.com/Mrpotatosse/AivyCore/blob/master/AivyDofus/Proxy/Callbacks/DofusRetroProxyClientReceiveCallback.cs )

This is an example of starting DofusServer or DofusProxy/DofusRetroProxy :

```csharp
    // example for 10 servers
    DofusServer server = new DofusServer("APP FOLDER PATH");
    int server_to_start = 10;
    for(int i = 1;i <= server_to_start;i++)
    {
        int server_port = 666;
        ServerEntity server_instance = server.Active(true, server_port + i); 
    }

    // example for 10 proxys (for DofusRetro it's the same , you just have to replace DofusProxy by DofusRetroProxy and DofusRetroProxy ctor got 
    // your Dofus.exe folder path , make sur the executable name is Dofus.exe (ONLY FOR RETRO) )
    DofusProxy proxy = new DofusProxy("EMPLACEMENT DU DOSSIER APP");
    int proxy_to_start = 10;
    for(int i = 1;i <= proxy_to_start;i++)
    {
        int proxy_port = 666;
        ProxyEntity proxy_instance = proxy.Active(true, proxy_port + i); 
        Thread.Sleep(2000); // Dofus client may take some time before starting , to avoid some client bug , make sur to sleep between each instance creator
    }
```

<h2> AivyDofus - Dofus 2.0 - Handler </h2>

DOFUS PROTOCOL IN JSON FORM IS IN YOUR EXECUTABLE FOLDER under the name ./protocol.json ( you have to start the program once to make it automatically created )

Dofus 2.0 Handlers are ``class`` 

Proxy Handlers : https://github.com/Mrpotatosse/AivyCore/tree/master/AivyDofus/Proxy/Handlers/Customs

Server Handlers : https://github.com/Mrpotatosse/AivyCore/tree/master/AivyDofus/Server/Handlers/Customs

Handler example commented : 

```csharp
    // you have to specify the attribute to make the program handle you message (set it to commentary if you don't want to handle it)
    // ProxyHandler for proxys and ServerHandler for servers
    [ProxyHandler(ProtocolName = "ServerSelectionMessage")]
    // Your Handler class have to inherit AbstractMessageHandler https://github.com/Mrpotatosse/AivyCore/blob/master/AivyDofus/Handler/AbstractMessageHandler.cs
    public class ServerSelectionMessageHandler : AbstractMessageHandler
    {
        // optional for logs
        static readonly Logger logger = LogManager.GetCurrentClassLogger();
        
        // obligatory , this property is only used by Proxy
        // set TRUE if you want to forward directly the data received 
        // set FALSE if you don't want to forward the data (you will have to send it manualy)
        public override bool IsForwardingData => false;

        // constructor arguments :
        //  - AbstractClientReceiveCallback => _callback : got
        //             ._tag -> enum for checking message provenance
        //             ._client -> client handling message
        //             ._remote -> client link (null for server) (for proxy , if _tag = Client then _remote = Dofus Server , else _remote = Dofus Client)
        //             ._client_repository -> client storage
        //             ._client_creator, ._client_linker, ._client_connector, ._client_disconnector -> class for client actions
        //  - NetworkElement => _element : message base (contain all data about the class reading and writing method)
        //  - NetworkContentElement => _content : message content
        // you can't change constructor arguments else you'll have a Runtime Error
        public ServerSelectionMessageHandler(AbstractClientReceiveCallback callback,
                                             NetworkElement element,
                                             NetworkContentElement content)
            : base(callback, element, content)
        {

        }
        
        // OBLIGATORY , handling function
        public override void Handle()
        {
            // to create a message/type you'll have to do it with a NetworkContentElement
            NetworkContentElement custom_message = new NetworkContentElement()
            {
                field = 
                { "nomDeLaPropriété", null }, // property value
                { "protocol_id" , 0 } // on some type , this is obligatory , check it on protocol.json , 'prefixed_by_type_id' value
                // { ... }   
            };
        }
        
        // optional
        public override void EndHandle()
        {
        
        }
        // optional
        public override void Error(Exception e)
        {
            logger.Error(e);
        }
    }
```

<h2> Dépendances </h2>

- NLog

- NewtonSoft Json

- EasyHook ( SocketHook by Nameless https://cadernis.fr/index.php?threads/sockethook-injector-alternative-%C3%A0-no-ankama-dll.2221/page-2#post-24796 i use an old version )

- Botofu parser ( https://gitlab.com/botofu/protocol_parser )
  
- LiteDB (https://www.litedb.org/) 
