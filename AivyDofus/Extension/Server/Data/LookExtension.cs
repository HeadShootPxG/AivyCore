using AivyData.API.Server.Look;
using AivyDofus.Protocol.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AivyDofus.Extension.Server.Data
{
    public static class LookExtension
    {
        public static NetworkContentElement Look(this EntityLookData look)
        {
            return new NetworkContentElement()
            {
                fields =
                {
                    { "protocol_id", 2892 },
                    { "bonesId", look.BonesId },// short
                    { "skins", look.Skins },
                    { "indexedColors", look.IndexedColors },
                    { "scales", look.Scales },
                    { "subentities", look.Subentities.Select(x => x.SubLook()).ToArray() },
                }
            };
        }

        public static NetworkContentElement SubLook(this SubEntityLookData sublook)
        {
            return new NetworkContentElement()
            {
                fields =
                {
                    { "protocol_id", 5174 },
                    { "bindingPointCategory", sublook.BindingPointCategory }, // byte
                    { "bindingPointIndex", sublook.BindingPointIndex },// byte
                    { "subEntityLook", sublook.SubEntityLook.Look() }
                }
            };
        }

        public static EntityLookData Build(this BreedObjectData breed, HeadObjectData head, int[] colors, bool sex)
        {
            for (int i = 0; i < colors.Length; i++)
            {
                if (colors[i] == -1 && (sex ? i < breed.femaleColors.Length : i < breed.maleColors.Length))
                {
                    colors[i] = sex ? breed.femaleColors[i] : breed.maleColors[i];
                }
                colors[i] = (i + 1 & 255) << 24 | colors[i] & 16777215;
            }

            string breed_look = sex ? breed.femaleLook : breed.maleLook;

            string breed_look_pattern = @"(?<bonesId>\d+)\|(?<skin>\d+)\|\|(?<scale>\d+)";

            Match match = Regex.Match(breed_look, breed_look_pattern);

            short bonesId = short.Parse(match.Groups["bonesId"].Value);
            short skin = short.Parse(match.Groups["skin"].Value);
            short scale = short.Parse(match.Groups["scale"].Value);
            
            EntityLookData look = new EntityLookData()
            {
                BonesId = bonesId,
                IndexedColors = colors,
                Skins = new short[] { skin, short.Parse(head.skins) },
                Scales = new short[] { scale },
                Subentities = new SubEntityLookData[0]
            };

            return look;
        }
    }
}
