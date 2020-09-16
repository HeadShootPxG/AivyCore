using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AivyDofus
{
    public class StaticValues
    {
        static readonly object _locker = new object();

        static bool _dofus_protocol_initied = false;
        public static bool DOFUS_PROTOCOL_INITIED
        {
            get
            {
                bool _result = _dofus_protocol_initied;
                DOFUS_PROTOCOL_INITIED = !_result;
                return _result;
            }
            set
            {
                lock (_locker)
                {
                    _dofus_protocol_initied = value;
                }

            }
        }
    }
}
