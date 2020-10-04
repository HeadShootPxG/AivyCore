using AivyData.API;
using AivyData.Entities;
using AivyDomain.API;
using AivyDomain.Mappers;
using System;
using System.Collections.Generic;
using System.Text;

namespace AivyDomain.Repository.Client
{
    public class ClientRepository : IRepository<ClientEntity, ClientData>
    {
        private readonly IApi<ClientData> _api;
        private readonly IMapper<Func<ClientEntity, bool>, ClientEntity> _mapper;

        public ClientRepository(IApi<ClientData> api,
                                IMapper<Func<ClientEntity, bool>, ClientEntity> mapper)
        {
            _api = api ?? throw new ArgumentNullException(nameof(api));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public ClientData ActionApi(Func<ClientData, bool> predicat, Func<ClientData, ClientData> action)
        {
            if (predicat is null) throw new ArgumentNullException(nameof(predicat));
            if (action is null) throw new ArgumentNullException(nameof(action));
            ClientData result = FromApi(predicat);
            if (result is null) throw new ArgumentNullException(nameof(result));
            return _api.UpdateData(action(result));
        }

        public ClientEntity ActionResult(Func<ClientEntity, bool> predicat, Func<ClientEntity,ClientEntity> action)
        {
            if (action is null) throw new ArgumentNullException(nameof(action));
            ClientEntity result = GetResult(predicat);
            if (result is null) return null;// throw new ArgumentNullException(nameof(result));
            return action(result);
        }

        public ClientData FromApi(Func<ClientData, bool> predicat)
        {
            if (predicat is null) throw new ArgumentNullException(nameof(predicat));
            return _api.GetData(predicat);
        }

        public ClientEntity GetResult(Func<ClientEntity, bool> predicat)
        {
            if (predicat is null) throw new ArgumentNullException(nameof(predicat));
            return _mapper.MapFrom(predicat);
        }

        public bool Remove(Func<ClientEntity, bool> predicat)
        {
            return _mapper.Remove(predicat);
        }
    }
}
