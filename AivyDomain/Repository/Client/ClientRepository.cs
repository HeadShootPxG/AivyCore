using AivyData.Entities;
using AivyDomain.API;
using AivyDomain.Mappers;
using System;
using System.Collections.Generic;
using System.Text;

namespace AivyDomain.Repository.Client
{
    public class ClientRepository : IRepository<ClientEntity>
    {
        private readonly IApi<ClientEntity> _api;
        private readonly IMapper<Func<ClientEntity, bool>, ClientEntity> _mapper;

        public ClientRepository(IApi<ClientEntity> api,
                                IMapper<Func<ClientEntity, bool>, ClientEntity> mapper)
        {
            _api = api ?? throw new ArgumentNullException(nameof(api));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public ClientEntity ActionResult(Func<ClientEntity, bool> predicat, Func<ClientEntity,ClientEntity> action)
        {
            if (action is null) throw new ArgumentNullException(nameof(action));
            ClientEntity result = GetResult(predicat);
            if (result is null) throw new ArgumentNullException(nameof(result));
            return action(result);
        }

        public ClientEntity FromApi(Func<ClientEntity, bool> predicat)
        {
            if (predicat is null) throw new ArgumentNullException(nameof(predicat));
            return _api.GetData(predicat);
        }

        public ClientEntity GetResult(Func<ClientEntity, bool> predicat)
        {
            if (predicat is null) throw new ArgumentNullException(nameof(predicat));
            return _mapper.MapFrom(predicat);
        }
    }
}
