using AivyData.API;
using AivyData.API.Server.Actor;
using AivyData.API.Server.Look;
using AivyDofus.Extension.Server.Data;
using AivyDofus.Handler;
using AivyDofus.Protocol.Elements;
using AivyDofus.Server.Callbacks;
using AivyDofus.Server.Handlers.Customs.Connection;
using AivyDomain.Callback.Client;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
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

        private static readonly NetworkElement _character_creation_result = BotofuProtocolManager.Protocol[ProtocolKeyEnum.Messages, x => x.name == "CharacterCreationResultMessage"];
        private static NetworkContentElement _character_creation_result_content(string error_code)
        {
            return new NetworkContentElement()
            {
                fields =
                {
                    { "result", int.Parse(BotofuProtocolManager.Protocol[x => x.name == "CharacterCreationResultEnum"][error_code]) }
                }
            };
        }

        public override void Handle()
        {
            DofusServerWorldClientReceiveCallback _world_callback = _casted_callback<DofusServerWorldClientReceiveCallback>();
            if (DofusServer._server_api.GetData(x => x.Port == _world_callback._server.Port) is ServerData server_data)
            {
                if (DofusServer._server_api.GetData<PlayerData>(x => x.AccountToken == _world_callback._client.CurrentToken).Count() >= 5)
                    throw new CharacterCreationException("ERR_TOO_MANY_CHARACTERS");

                string name = _content["name"];

                if (!Regex.IsMatch(name, @"(\w){3,20}"))
                    throw new CharacterCreationException("ERR_INVALID_NAME");

                if (DofusServer._server_api.GetData<PlayerData>(x => x.Name.ToLower() == name.ToLower()).Count() > 0)
                    throw new CharacterCreationException("ERR_NAME_ALREADY_EXISTS");

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
                    rnd_id = rnd.Next(1, int.MaxValue - 1);
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
                    Look = breed_data.Build(head_data, colors, sex) 
                };

                DofusServer._server_api.UpdateData(_player);
            }
        }

        // if no error
        public override void EndHandle()
        {
            Send(false, _callback._client, _character_creation_result, _character_creation_result_content("OK"));
            DofusServerWorldClientReceiveCallback _world_callback = _casted_callback<DofusServerWorldClientReceiveCallback>();
            if (DofusServer._server_api.GetData(x => x.Port == _world_callback._server.Port) is ServerData server_data &&
                DofusServer._server_api.GetData<PlayerData>(x => x.AccountToken == _world_callback._client.CurrentToken && x.ServerId == server_data.ServerId) is IEnumerable<PlayerData> players)
            {
                NetworkContentElement characters_list = CharactersListRequestMessageHandler._characters_list_content(players, server_data);
                Send(false, _callback._client, CharactersListRequestMessageHandler._characters_list_message, characters_list);
                // connect client
            }
        }

        public override void Error(Exception e)
        {
            if(e is CharacterCreationException result)
            {
                Send(false, _callback._client, _character_creation_result, _character_creation_result_content(result.error_code));
            }
            else logger.Error(e);
        }

        private class CharacterCreationException : Exception
        {
            public string error_code { get; set; }

            public CharacterCreationException(string error_code)
            {
                this.error_code = error_code;
            }
        }
    }
}
