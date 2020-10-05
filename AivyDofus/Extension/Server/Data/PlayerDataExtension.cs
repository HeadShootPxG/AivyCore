using AivyData.API.Server.Actor;
using AivyDofus.Protocol.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AivyDofus.Extension.Server.Data
{
    public static class PlayerDataExtension
    {
        public static NetworkContentElement BaseInformation(this PlayerData player)
        {
            return new NetworkContentElement()
            {
                fields =
                {
                    { "protocol_id", 6238 },
                    { "id", player.Id },
                    { "name", player.Name },
                    { "level", 1 /*short*/ },
                    { "breed", player.Breed },
                    { "entityLook", player.Look.Look() },
                    { "sex", player.Sex }
                }
            };
        }

        public static NetworkContentElement BaseHardcoreInformation(this PlayerData player)
        {
            NetworkContentElement base_information = player.BaseInformation();

            base_information["protocol_id"] = 3059;
            base_information["deathState"] = 0;// byte -> PlayerLifeStatusEnum
            base_information["deathCount"] = 0;// short
            base_information["deathMaxLevel"] = 1;// short

            return base_information;
        }
    }
}
