using NLog;
using NLog.Config;
using NLog.Targets;
using System;
using AivyData.Entities;
using AivyDomain.API.Proxy;
using AivyDomain.Mappers.Proxy;
using AivyDomain.Repository.Proxy;
using AivyDomain.UseCases.Proxy;
using System.Diagnostics;

namespace AivyCore
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Zobi it is empty ='(");

            ProcessStartInfo info = new ProcessStartInfo(@"D:\MEGA_DL\Botofu\botofu_protocol_parser.exe", @"D:\DofusApp\DofusInvoker.swf D:\MEGA_DL\Botofu\protocol.json");
            Process p = Process.Start(info);
            Console.ReadLine();
        }
    }
}
