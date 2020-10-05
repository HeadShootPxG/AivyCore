using System;
using System.Collections.Generic;
using System.Text;

namespace AivyData.API.Server.Look
{
    public class BreedRoleObjectData
    {
        public byte breedId { get; set; }
        public int roleId { get; set; }
        public uint descriptionId { get; set; }
        public short value { get; set; }
        public short order { get; set; }
    }
}
