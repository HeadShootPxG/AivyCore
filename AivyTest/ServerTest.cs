using AivyData.Entities;
using AivyDomain.API.Server;
using AivyDomain.Callback.Server;
using AivyDomain.Mappers.Server;
using AivyDomain.Repository.Server;
using AivyDomain.UseCases.Server;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace AivyTest
{
    [TestClass]
    public class ServerTest
    {
        private OpenServerApi _server_api;
        private ServerEntityMapper _server_mapper;
        private ServerRepository _server_repository;
        // request
        private ServerCreatorRequest _server_creator;
        private ServerActivatorRequest _server_activator;

        [TestMethod]
        public void Test1()
        {
            _server_api = new OpenServerApi("./server_information.json");
            _server_mapper = new ServerEntityMapper();
            _server_repository = new ServerRepository(_server_api, _server_mapper);

            _server_creator = new ServerCreatorRequest(_server_repository);
            _server_activator = new ServerActivatorRequest(_server_repository);

            ServerEntity server = _server_creator.Handle(666);

            Assert.IsNotNull(server);
            Assert.AreEqual(666, server.Port);

            ServerEntity server2 = _server_creator.Handle(667);

            Assert.IsNotNull(server2);
            Assert.AreEqual(667, server2.Port);
            Assert.AreEqual(666, server.Port);

            Assert.AreNotSame(server, server2);

            server = _server_activator.Handle(server, true, new ServerAcceptCallback(server));

            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 666));

            Assert.AreEqual(true, socket.Connected);
            Assert.AreEqual(true, server.IsRunning);
            Assert.AreEqual(false, server2.IsRunning);

            server = _server_activator.Handle(server, false, null);

            Assert.AreEqual(false, server.IsRunning);
        }
    }
}
