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
                if (_dofus_protocol_initied)
                    return true;

                DOFUS_PROTOCOL_INITIED = true;
                return false;
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
