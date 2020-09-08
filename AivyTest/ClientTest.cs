using AivyData.Entities;
using AivyDomain.API.Client;
using AivyDomain.Mappers.Client;
using AivyDomain.Repository.Client;
using AivyDomain.UseCases.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AivyTest
{
    [TestClass]
    public class ClientTest
    {
        private OpenClientApi _client_api;
        private ClientEntityMapper _client_mapper;
        private ClientRepository _client_repository;
        // request
        private ClientCreatorRequest _client_creator;

        [TestMethod]
        public void Test1()
        {
            _client_api = new OpenClientApi("./server_information.json");
            _client_mapper = new ClientEntityMapper();
            _client_repository = new ClientRepository(_client_api, _client_mapper);

            _client_creator = new ClientCreatorRequest(_client_repository);

            ClientEntity client = _client_creator.Handle(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 666));

            Assert.IsNotNull(client);

            ClientEntity client2 = _client_creator.Handle(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 666));

            Assert.IsNotNull(client2);

            Assert.AreNotSame(client, client2);
        }
    }
}
