using System;
using System.Collections.Generic;
using System.Text;

namespace AivyDomain.Callback
{
    public interface IAsyncCallback
    {
        void Callback(IAsyncResult result);
    }
}
