using AivyData.API;
using AivyData.Entities;
using AivyDofus.Protocol.Parser;
using AivyDofus.Server.API;
using AivyDofus.Server.Callbacks;
using AivyDomain.API;
using AivyDomain.API.Server;
using AivyDomain.Mappers.Server;
using AivyDomain.Repository.Server;
using AivyDomain.UseCases.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AivyDofus.Server
{
    public class DofusServer
    {
        public static readonly OpenServerDatabaseApi _server_api = new OpenServerDatabaseApi("./server_database.db");
        readonly ServerEntityMapper _server_mapper;
        readonly ServerRepository _server_repository;

        readonly ServerCreatorRequest _server_creator;
        readonly ServerActivatorRequest _server_activator;

        ServerEntity _server;

        readonly string _app_path;
        string _exe_path => $"{_app_path}/Dofus.exe";
        string _invoker_path => $"{_app_path}/DofusInvoker.swf";

        public DofusServer(string appPath, int port)
        {
            _app_path = appPath ?? throw new ArgumentNullException(nameof(appPath));

            _server_mapper = new ServerEntityMapper();
            _server_repository = new ServerRepository(_server_api, _server_mapper);

            _server_creator = new ServerCreatorRequest(_server_repository);
            _server_activator = new ServerActivatorRequest(_server_repository);

            _server = _server_creator.Handle(port);

            if (!StaticValues.DOFUS_PROTOCOL_INITIED)
                new BotofuParser(_invoker_path).Parse();
        }

        public void Active(bool active)
        {
            _server = _server_activator.Handle(_server, active, new DofusServerAcceptCallback(_server));
        }
    }
}
