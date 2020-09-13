using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AivyDofus.Handler
{
    public interface IMessageHandler
    {
        void Handle();
        void EndHandle();
        void Error(Exception e);
    }
}
