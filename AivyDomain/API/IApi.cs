using System;
using System.Collections.Generic;
using System.Text;

namespace AivyDomain.API
{
    public interface IApi<TData>
    {
        TData GetData(Func<TData, bool> predicat);
        TData UpdateData(TData data);
    }
}
