using System;
using System.Collections.Generic;
using System.Text;

namespace AivyData.API.Server.Look
{
    public class SubEntityLookData
    {
        public byte BindingPointCategory { get; set; }
        public byte BindingPointIndex { get; set; }
        public EntityLookData SubEntityLook { get; set; }
    }
}
