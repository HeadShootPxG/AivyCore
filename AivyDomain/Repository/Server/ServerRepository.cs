using AivyData.API;
using AivyData.Entities;
using AivyDomain.API;
using AivyDomain.Mappers;
using System;
using System.Collections.Generic;
using System.Text;

namespace AivyDomain.Repository.Server
{
    public class ServerRepository : IRepository<ServerEntity, ServerData>
    {
        private readonly IApi<ServerData> _api;
        private readonly IMapper<Func<ServerEntity, bool>, ServerEntity> _mapper;

        public ServerRepository(IApi<ServerData> api,
                                IMapper<Func<ServerEntity,bool>, ServerEntity> mapper)
        {
            _api = api ?? throw new ArgumentNullException(nameof(api));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public ServerData ActionApi(Func<ServerData, bool> predicat, Func<ServerData, ServerData> action)
        {
            if (predicat is null) throw new ArgumentNullException(nameof(predicat));
            if (action is null) throw new ArgumentNullException(nameof(action));
            ServerData result = FromApi(predicat);
            if (result is null) throw new ArgumentNullException(nameof(result));
            return _api.UpdateData(action(result));
        }

        public ServerEntity ActionResult(Func<ServerEntity, bool> predicat, Func<ServerEntity, ServerEntity> action)
        {
            if (action is null) throw new ArgumentNullException(nameof(action));
            ServerEntity result = GetResult(predicat);
            if (result is null) return null;// throw new ArgumentNullException(nameof(result));
            return action(result);
        }

        public ServerData FromApi(Func<ServerData, bool> predicat)
        {
            if (predicat is null) throw new ArgumentNullException(nameof(predicat));
            return _api.GetData(predicat);
        }

        public ServerEntity GetResult(Func<ServerEntity, bool> predicat)
        {
            if (predicat is null) throw new ArgumentNullException(nameof(predicat));
            return _mapper.MapFrom(predicat);
        }

        public bool Remove(Func<ServerEntity, bool> predicat)
        {
            return _mapper.Remove(predicat);
        }
    }
}
