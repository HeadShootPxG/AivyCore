using AivyData.Entities;
using AivyData.Enums;
using AivyDofus.Protocol.Parser;
using AivyDofus.Proxy;
using AivyDofus.Proxy.Callbacks;
using AivyDofus.Server;
using AivyDomain.API.Proxy;
using AivyDomain.Callback.Proxy;
using AivyDomain.Mappers.Proxy;
using AivyDomain.Repository.Proxy;
using AivyDomain.UseCases.Proxy;
using NLog;
using NLog.Config;
using NLog.Targets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AivyDofus
{
    public class Program
    {
        static readonly ConsoleTarget log_console = new ConsoleTarget("log_console");
        static readonly FileTarget log_file = new FileTarget("log_file") { FileName = "./log.txt" };
        static readonly LoggingConfiguration configuration = new LoggingConfiguration();

        public static void Main(string[] args)
        {
            Console.Title = "AivyCore - 1.0.0";

            configuration.AddRule(LogLevel.Debug, LogLevel.Fatal, log_console);
            LogManager.Configuration = configuration;

            try
            {
                string type = args[0];
                if (type == "-p")
                {
                    if (int.TryParse(args[1], out int proxy_type))
                    {
                        DofusMultiProxy multi_proxy = new DofusMultiProxy();
                        if (int.TryParse(args[2], out int proxy_port))
                        {
                            ProxyEntity p_entity = multi_proxy.Active((ProxyCallbackTypeEnum)proxy_type, true, proxy_port, args[3], args[4]);
                        }
                        else
                        {
                            throw new Exception();
                        }
                    }
                    else
                    {
                        throw new Exception();
                    }
                }
                if (type == "-s")
                {
                    // to do
                }
            }
            catch(Exception e)
            {
                LogManager.GetCurrentClassLogger().Fatal($"bad args {string.Join(" ", args)}\ntry with -> -type(s or p) parsing_type(2 or 1 or 0) port(number) folder_location(string) exe_name(string)");
                LogManager.GetCurrentClassLogger().Error(e);

                Console.ReadKey();
                Environment.Exit(0);
            }

            /*DofusRetroProxy r_proxy = new DofusRetroProxy(@"D:\retro\resources\app\retroclient");
            ProxyEntity retro_entity = r_proxy.Active(true, 668);*/

            /*DofusProxy proxy = new DofusProxy(@"D:\AppDofus");
            ProxyEntity p_entity = proxy.Active(true, 666);*/

            /*Thread.Sleep(2000);
            ProxyEntity p2_entity = proxy.Active(true, 667);

            DofusServer server = new DofusServer(@"D:\AppDofus");
            ServerEntity s_entity = server.Active(true, 777);*/

            Console.ReadLine();
        }
    }
}
