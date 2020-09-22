using System;
using System.Collections.Generic;
using System.Text;

namespace AivyDomain.Repository
{
    public interface IRepository<Entity, Data>
    {
        Entity GetResult(Func<Entity, bool> predicat);
        Entity ActionResult(Func<Entity, bool> predicat, Func<Entity, Entity> action);

        Data FromApi(Func<Data, bool> predicat);
        Data ActionApi(Func<Data, bool> predicat, Func<Data, Data> action);

        bool Remove(Func<Entity, bool> predicat);
    }
}
