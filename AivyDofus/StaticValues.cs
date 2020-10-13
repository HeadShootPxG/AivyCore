using AivyData.API.Server.Look;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AivyDofus
{
    public class StaticValues
    {
        static readonly object _locker_raw_data_id = new object();
        static int _raw_data_id = 1528;
        public static int RAW_DATA_MSG_RCV_ID
        {
            get
            {
                return _raw_data_id;
            }
            set
            {
                lock (_locker_raw_data_id)
                {
                    _raw_data_id = value;
                }
            }
        }

        static readonly object _locker_breed = new object();
        static BreedObjectData[] _breeds = null;
        public static BreedObjectData[] BREEDS
        {
            get
            {
                if(_breeds is null)
                {
                    BREEDS = JsonConvert.DeserializeObject<BreedObjectData[]>(Encoding.UTF8.GetString(Properties.Resources.BreedJson), new JsonSerializerSettings() { Formatting = Formatting.Indented });
                }
                return _breeds;
            }
            set
            {
                lock (_locker_breed)
                {
                    _breeds = value;
                }
            }
        }

        static readonly object _locker_head = new object();
        static HeadObjectData[] _heads = null;
        public static HeadObjectData[] HEADS
        {
            get
            {
                if(_heads is null)
                {
                    HEADS = JsonConvert.DeserializeObject<HeadObjectData[]>(Encoding.UTF8.GetString(Properties.Resources.HeadJson), new JsonSerializerSettings() { Formatting = Formatting.Indented });
                }
                return _heads;
            }
            set
            {
                lock (_locker_head)
                {
                    _heads = value;
                }
            }
        }
    }
}
