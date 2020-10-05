using AivyData.API;
using AivyData.API.Server.Actor;
using AivyData.API.Server.Look;
using AivyDofus.Extension.Server.Data;
using AivyDofus.Handler;
using AivyDofus.Protocol.Elements;
using AivyDofus.Server.Callbacks;
using AivyDomain.Callback.Client;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace AivyDofus.Server.Handlers.Customs.Creation
{
    [ServerHandler(ProtocolName = "CharacterCreationRequestMessage")]
    public class CharacterCreationRequestMessageHandler : AbstractMessageHandler
    {
        static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public override bool IsForwardingData => true;

        public CharacterCreationRequestMessageHandler(AbstractClientReceiveCallback callback, NetworkElement element, NetworkContentElement content) 
            : base(callback, element, content)
        {

        }

        public override void Handle()
        {
            DofusServerWorldClientReceiveCallback _world_callback = _casted_callback<DofusServerWorldClientReceiveCallback>();
            if (DofusServer._server_api.GetData(x => x.Port == _world_callback._server.Port) is ServerData server_data)
            {
                string name = _content["name"];
                int[] colors = (_content["colors"] as dynamic[]).Select(x => x is int v ? v : -1).ToArray();
                byte breed = _content["breed"];
                bool sex = _content["sex"];
                short cosmeticId = _content["cosmeticId"];

                BreedObjectData breed_data = StaticValues.BREEDS.FirstOrDefault(x => x.Id == breed);
                HeadObjectData head_data = StaticValues.HEADS.FirstOrDefault(x => x.Id == cosmeticId);

                Random rnd = new Random();
                int rnd_id = -1;

                while(rnd_id == -1)
                {
                    rnd_id = rnd.Next(1, short.MaxValue);
                    if(DofusServer._server_api.GetData<PlayerData>(x => x.Id == rnd_id).Count() > 0)
                    {
                        rnd_id = -1;
                    }
                }

                PlayerData _player = new PlayerData()
                {
                    Id = rnd_id,
                    AccountToken = _world_callback._client.CurrentToken,
                    Breed = breed,
                    Name = name,
                    Sex = sex,
                    ServerId = server_data.ServerId,
                    CreationDateTime = DateTime.Now,
                    Look = breed_data.Build(head_data, colors, sex) // to do parse
                };

                DofusServer._server_api.UpdateData(_player);
            }
        }

        public override void Error(Exception e)
        {
            logger.Error(e);
        }
    }
}
