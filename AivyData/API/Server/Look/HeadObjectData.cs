using System;
using System.Collections.Generic;
using System.Text;

namespace AivyData.API.Server.Look
{
    public class HeadObjectData : IdentifiableData
    {
        public string skins { get; set; }
        public string asset_id { get; set; }
        public byte breed { get; set; }
        public byte gender { get; set; }
        public string label { get; set; }
        public byte order { get; set; }
    }
}
