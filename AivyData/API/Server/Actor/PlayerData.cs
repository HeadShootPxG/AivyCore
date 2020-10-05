using System;
using System.Collections.Generic;
using System.Text;

namespace AivyData.API.Server.Actor
{
    public class PlayerData : ActorData
    {
        public string AccountToken { get; set; }
        public byte Breed { get; set; }
        public string Name { get; set; }
    }
}
