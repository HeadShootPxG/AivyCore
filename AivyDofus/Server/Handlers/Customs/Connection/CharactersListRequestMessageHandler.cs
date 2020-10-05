using AivyData.API;
using AivyData.API.Server.Actor;
using AivyDofus.Extension.Server.Data;
using AivyDofus.Handler;
using AivyDofus.Protocol.Elements;
using AivyDofus.Server.Callbacks;
using AivyDomain.Callback.Client;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AivyDofus.Server.Handlers.Customs.Connection
{
    [ServerHandler(ProtocolName = "CharactersListRequestMessage")]
    public class CharactersListRequestMessageHandler : AbstractMessageHandler
    {
        static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public override bool IsForwardingData => true;

        public CharactersListRequestMessageHandler(AbstractClientReceiveCallback callback,
                                                   NetworkElement element,
                                                   NetworkContentElement content)
            : base(callback, element, content)
        {

        }

        public static readonly NetworkElement _characters_list_message = BotofuProtocolManager.Protocol[ProtocolKeyEnum.Messages, x => x.name == "CharactersListMessage"];
        public static NetworkContentElement _characters_list_content(IEnumerable<PlayerData> players, ServerData server_data, bool hasStartupActions = false)
        {
            return new NetworkContentElement()
            {
                fields =
                {
                    { "hasStartupActions", hasStartupActions },
                    { "characters", players.Select(x => server_data.Type == 1 ? x.BaseHardcoreInformation() : x.BaseInformation()).ToArray() }
                }
            };
        }

        public override void Handle()
        {
            DofusServerWorldClientReceiveCallback _world_callback = _casted_callback<DofusServerWorldClientReceiveCallback>();
            if (DofusServer._server_api.GetData(x => x.Port == _world_callback._server.Port) is ServerData server_data &&
                DofusServer._server_api.GetData<PlayerData>(x => x.AccountToken == _world_callback._client.CurrentToken && x.ServerId == server_data.ServerId) is IEnumerable<PlayerData> players)
            {
                NetworkContentElement characters_list_content = _characters_list_content(players, server_data);
                Send(false, _callback._client, _characters_list_message, characters_list_content);
            }
        }

        public override void Error(Exception e)
        {
            logger.Error(e);
        }
    }
}
