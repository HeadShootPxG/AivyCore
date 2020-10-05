using AivyData.API.Server.Look;
using System;
using System.Collections.Generic;
using System.Text;

namespace AivyData.API.Server.Actor
{
    public abstract class ActorData : IdentifiableData
    {
        public bool Sex { get; set; }
        public EntityLookData Look { get; set; }
        public int ServerId { get; set; }
        public DateTime CreationDateTime { get; set; }

    }
}
