using System;
using System.Collections.Generic;
using System.Text;

namespace AivyDomain.Repository
{
    public interface IRepository<Result>
    {
        Result GetResult(Func<Result, bool> predicat);
        Result FromApi(Func<Result, bool> predicat);
        Result ActionResult(Func<Result, bool> predicat, Func<Result,Result> action);
    }
}
