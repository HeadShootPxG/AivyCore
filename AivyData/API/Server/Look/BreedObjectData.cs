using System;
using System.Collections.Generic;
using System.Text;

namespace AivyData.API.Server.Look
{
    public class BreedObjectData : IdentifiableData
    {
        public int shortNameId { get; set; }
        public long longNameId { get; set; }
        public uint descriptionId { get; set; }
        public uint gameplayDescriptionId { get; set; }
        public uint gameplayClassDescriptionId { get; set; }
        public string maleLook { get; set; }
        public string femaleLook { get; set; }
        public short creatureBonesId { get; set; }
        public short maleArtwork { get; set; }
        public short femaleArtwork { get; set; }

        public uint[,] statsPointsForStrength { get; set; }
        public uint[,] statsPointsForIntelligence { get; set; }
        public uint[,] statsPointsForChance { get; set; }
        public uint[,] statsPointsForAgility { get; set; }
        public uint[,] statsPointsForVitality { get; set; }
        public uint[,] statsPointsForWisdom { get; set; }

        public int[] breedSpellsId { get; set; }
        public BreedRoleObjectData[] breedRoles { get; set; }
        public int[] maleColors { get; set; }
        public int[] femaleColors { get; set; }
        public int spawnMap { get; set; }
        public byte complexity { get; set; }
        public short sortIndex { get; set; }
    }
}
