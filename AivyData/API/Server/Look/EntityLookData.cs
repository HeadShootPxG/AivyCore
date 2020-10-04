using System;
using System.Collections.Generic;
using System.Text;

namespace AivyData.API.Server.Look
{
    public class EntityLookData
    {
        public short BonesId { get; set; }
        public short[] Skins { get; set; }
        public int[] IndexedColors { get; set; }
        public short[] Scales { get; set; }
        public SubEntityLookData[] Subentities { get; set; }
    }
}
