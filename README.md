<h2> # AivyCore </h2>
Proxy Modulable

Pour le moment les APIs ne sont pas encore implémenté. 

Voici un exemple de Program permettant d'installer un proxy sur un fichier éxécutable en lançant le proxy sur le port 666
```csharp
using NLog;
using NLog.Config;
using NLog.Targets;
using System;
using AivyData.Entities;
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

            ProxyEntity proxy = _proxy_creator.Handle(@"VOTRE FICHIER EXECUTABLE", 666);
            proxy = _proxy_activator.Handle(proxy, true);

            Console.ReadLine();
        }
    }
}
```

<h2> Dépendances </h2>

- NLog

- NewtonSoft Json

- EasyHook ( SocketHook de Nameless https://cadernis.fr/index.php?threads/sockethook-injector-alternative-%C3%A0-no-ankama-dll.2221/page-2#post-24796 )

- Botofu parser

