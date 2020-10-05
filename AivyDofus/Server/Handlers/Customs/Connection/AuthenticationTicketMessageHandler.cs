using AivyData.API.Server.Account;
using AivyData.Entities;
using AivyDofus.Handler;
using AivyDofus.Protocol.Elements;
using AivyDofus.Server.Callbacks;
using AivyDomain.Callback.Client;
using AivyDomain.UseCases.Client;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace AivyDofus.Server.Handlers.Customs.Connection
{
    [ServerHandler(ProtocolName = "AuthenticationTicketMessage")]
    public class AuthenticationTicketMessageHandler : AbstractMessageHandler
    {
        static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public override bool IsForwardingData => true;

        public AuthenticationTicketMessageHandler(AbstractClientReceiveCallback callback,
                                                  NetworkElement element,
                                                  NetworkContentElement content)
            : base(callback, element, content)
        {

        }

        public override void Handle()
        {
            DofusServerWorldClientReceiveCallback _world_callback = _casted_callback<DofusServerWorldClientReceiveCallback>();

            string token = _content["ticket"];            

            if (DofusServer._server_api.GetData<ServerAccountInformationData>(x => x.Token == token).FirstOrDefault() is ServerAccountInformationData _account)
            {
                ClientEntity connected = _world_callback._client_repository.GetResult(x => x.CurrentToken == token && x.IsRunning && x != _world_callback._client);
                if(connected != null)
                {
                    _world_callback._client_disconnector.Handle(connected);
                }
                _world_callback._client.CurrentToken = token;

                NetworkElement authentication_accepted_message = BotofuProtocolManager.Protocol[ProtocolKeyEnum.Messages, x => x.name == "AuthenticationTicketAcceptedMessage"];
                NetworkElement account_capabilities_message = BotofuProtocolManager.Protocol[ProtocolKeyEnum.Messages, x => x.name == "AccountCapabilitiesMessage"];

                NetworkContentElement account_capabilities_content = new NetworkContentElement()
                {
                    fields =
                    {
                        { "tutorialAvailable", true },
                        { "canCreateNewCharacter", true },
                        { "accountId", _account.Id },
                        { "breedsVisible", 262143 },
                        { "breedsAvailable", 262143 },
                        { "status", 0 }
                    }
                };

                Send(false, _callback._client, authentication_accepted_message, new NetworkContentElement());
                Send(false, _callback._client, account_capabilities_message, account_capabilities_content);
            }
            else
            {
                // create account
                byte[] ticket_bytes = Convert.FromBase64String(token);
                // to do
                byte[] id = new byte[]
                {
                    ticket_bytes[1],
                    ticket_bytes[3],
                    ticket_bytes[7],
                    ticket_bytes[15],
                };

                ServerAccountInformationData new_account = new ServerAccountInformationData()
                {
                    Id = Math.Abs(BitConverter.ToInt32(id, 0)),
                    Token = token,

                    CreationDateTime = DateTime.Now,
                    LastConnectionDateTime = DateTime.Now,
                    TotalTimeSpentInGame = TimeSpan.FromMilliseconds(0),
                    FriendsToken = new List<string>(),
                    IgnoredsToken = new List<string>()
                };

                DofusServer._server_api.UpdateData(new_account);
                Handle();
            }
        }

        public override void Error(Exception e)
        {
            logger.Error(e);
        }
    }
}
