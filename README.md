# AivyCore
Modulable Proxy

Un exemple de proxy dans https://github.com/Mrpotatosse/AivyCore/blob/master/AivyCore/Program.cs

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

            ProxyEntity proxy = _proxy_creator.Handle(@"D:\DofusApp\Dofus.exe", 666);
            proxy = _proxy_activator.Handle(proxy, true);

            Console.ReadLine();
        }
    }
}
```
