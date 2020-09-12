using AivyData.Entities;
using AivyDomain.API;
using AivyDomain.Mappers;
using System;
using System.Collections.Generic;
using System.Text;

namespace AivyDomain.Repository.Proxy
{
    public class ProxyRepository : IRepository<ProxyEntity>
    {
        private readonly IApi<ProxyEntity> _api;
        private readonly IMapper<Func<ProxyEntity, bool>, ProxyEntity> _mapper;

        public ProxyRepository(IApi<ProxyEntity> api,
                               IMapper<Func<ProxyEntity, bool>, ProxyEntity> mapper)
        {

            _api = api ?? throw new ArgumentNullException(nameof(api));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public ProxyEntity ActionResult(Func<ProxyEntity, bool> predicat, Func<ProxyEntity, ProxyEntity> action)
        {
            if (action is null) throw new ArgumentNullException(nameof(action));
            ProxyEntity result = GetResult(predicat);
            if (result is null) throw new ArgumentNullException(nameof(result));
            return action(result);
        }        

        public ProxyEntity FromApi(Func<ProxyEntity, bool> predicat)
        {
            if (predicat is null) throw new ArgumentNullException(nameof(predicat));
            return _api.GetData(predicat);
        }

        public ProxyEntity GetResult(Func<ProxyEntity, bool> predicat)
        {
            if (predicat is null) throw new ArgumentNullException(nameof(predicat));
            return _mapper.MapFrom(predicat);
        }
    }
}
