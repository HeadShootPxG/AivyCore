using System;
using System.Collections.Generic;
using System.Text;

namespace AivyDomain.Mappers
{
    public interface IMapper<in Input, out Output>
    {
        Output MapFrom(Input input);
        bool Remove(Func<Output, bool> predicat);
    }
}
