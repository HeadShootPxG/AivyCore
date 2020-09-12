using AivyData.Entities;
using AivyDomain.API.Client;
using AivyDomain.API.Server;
using AivyDomain.Mappers.Client;
using AivyDomain.Mappers.Server;
using AivyDomain.Repository.Client;
using AivyDomain.Repository.Server;
using AivyDomain.UseCases.Client;
using AivyDomain.UseCases.Server;
using NLog;
using NLog.Config;
using NLog.Targets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Remoting;
using AivyDomain.API.Proxy;
using AivyDomain.Mappers.Proxy;
using AivyDomain.Repository.Proxy;
using AivyDomain.UseCases.Proxy;

namespace AivyCore
{
    class Program
    {
        static readonly ConsoleTarget log_console = new ConsoleTarget("log_console");
        static readonly LoggingConfiguration configuration = new LoggingConfiguration();
        static readonly Logger logger = LogManager.GetCurrentClassLogger();

        /*static OpenServerApi _server_api;
        static ServerEntityMapper _server_mapper;
        static ServerRepository _server_repository;
        // request
        static ServerCreatorRequest _server_creator;
        static ServerActivatorRequest _server_activator;


        static OpenClientApi _client_api;
        static ClientEntityMapper _client_mapper;
        static ClientRepository _client_repository;
        // request
        static ClientCreatorRequest _client_creator;
        static ClientConnectorRequest _client_connector;
        static ClientLinkerRequest _client_linker;
        static ClientSenderRequest _client_sender;*/

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

            ProxyEntity proxy = _proxy_creator.Handle(@"D:\DofusApp\Dofus.exe", 666);
            proxy = _proxy_activator.Handle(proxy, true);

            /*_server_api = new OpenServerApi("./server_information.json");
            _server_mapper = new ServerEntityMapper();
            _server_repository = new ServerRepository(_server_api, _server_mapper);

            _server_creator = new ServerCreatorRequest(_server_repository);
            _server_activator = new ServerActivatorRequest(_server_repository);

            _client_api = new OpenClientApi("./client_information.json");
            _client_mapper = new ClientEntityMapper();
            _client_repository = new ClientRepository(_client_api, _client_mapper);

            _client_creator = new ClientCreatorRequest(_client_repository);
            _client_connector = new ClientConnectorRequest(_client_repository);
            _client_linker = new ClientLinkerRequest(_client_repository);
            _client_sender = new ClientSenderRequest(_client_repository);

            ServerEntity server = _server_creator.Handle(666);
            server = _server_activator.Handle(server, true);

            ClientEntity client = _client_creator.Handle(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 666));
            client = _client_linker.Handle(client, new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp));
            client = _client_connector.Handle(client);
            while (!client.IsRunning) { }
            client = _client_sender.Handle(client, new byte[] { 1,2,3,4,5,6,7,8,9,10 });
            client = _client_sender.Handle(client, new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });
            client = _client_sender.Handle(client, new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });
            client = _client_sender.Handle(client, new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });
            client = _client_sender.Handle(client, new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });
            client = _client_sender.Handle(client, new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });
            client = _client_sender.Handle(client, new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });
            client = _client_sender.Handle(client, new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });
            client = _client_sender.Handle(client, new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });*/

            Console.ReadLine();
        }
    }
}
