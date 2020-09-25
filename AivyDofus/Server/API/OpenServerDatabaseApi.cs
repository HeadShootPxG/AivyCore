﻿using AivyData.API;
using AivyDomain.API;
using LiteDB;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace AivyDofus.Server.API
{
    public class OpenServerDatabaseApi : IApi<ServerData>
    {
        static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private readonly string _location;

        public OpenServerDatabaseApi(string location)
        {
            _location = location ?? throw new ArgumentNullException(location);

            /* default entry */
            if(GetData(x => x.ServerId == 671) is null)
            {
                ServerData data = UpdateData(new ServerData()
                {
                    Port = 777,
                    Ip = "127.0.0.1",

                    ServerId = 671,
                    MaxCharacterCount = 1,
                    IsMonoAccount = true,
                    Completion = 0,
                    Status = 3,
                    Type = 1
                });
            }
        }

        public ServerData GetData(Func<ServerData, bool> predicat)
        {
            if (predicat is null) throw new ArgumentNullException(nameof(predicat));

            using(LiteDatabase db = new LiteDatabase(_location))
            {
                ILiteCollection<ServerData> servers = db.GetCollection<ServerData>(typeof(ServerData).Name);
                return servers.Query().ToArray().FirstOrDefault(predicat);
            }
        }

        public ServerData UpdateData(ServerData data)
        {
            if (data is null) throw new ArgumentNullException(nameof(data));

            using (LiteDatabase db = new LiteDatabase(_location))
            {
                ILiteCollection<ServerData> servers = db.GetCollection<ServerData>(typeof(ServerData).Name);

                servers.Upsert(data);
                return data;
            }
        }
    }
}