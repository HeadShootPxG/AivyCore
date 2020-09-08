using AivyData.Entities;
using AivyDomain.API;
using AivyDomain.Mappers;
using System;
using System.Collections.Generic;
using System.Text;

namespace AivyDomain.Repository.Server
{
    public class ServerRepository : IRepository<ServerEntity>
    {
        private readonly IApi<ServerEntity> _api;
        private readonly IMapper<Func<ServerEntity, bool>, ServerEntity> _mapper;

        public ServerRepository(IApi<ServerEntity> api,
                                IMapper<Func<ServerEntity,bool>, ServerEntity> mapper)
        {
            _api = api ?? throw new ArgumentNullException(nameof(api));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public ServerEntity ActionResult(Func<ServerEntity, bool> predicat, Func<ServerEntity, ServerEntity> action)
        {
            if (action is null) throw new ArgumentNullException(nameof(action));
            ServerEntity result = GetResult(predicat);
            if (result is null) throw new ArgumentNullException(nameof(result));
            return action(result);
        }

        public ServerEntity FromApi(Func<ServerEntity, bool> predicat)
        {
            if (predicat is null) throw new ArgumentNullException(nameof(predicat));
            return _api.GetData(predicat);
        }

        public ServerEntity GetResult(Func<ServerEntity, bool> predicat)
        {
            if (predicat is null) throw new ArgumentNullException(nameof(predicat));
            return _mapper.MapFrom(predicat);
        }
    }
}
