using AivyData.Entities;
using AivyData.Enums;
using AivyDomain.Repository.Client;
using AivyDomain.UseCases.Client;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AivyDofus.Proxy.Callbacks
{
    public class DofusRetroProxyClientReceiveCallback : DofusProxyClientReceiveCallback
    {
        static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public DofusRetroProxyClientReceiveCallback(ClientEntity client, ClientEntity remote, ClientRepository repository, ClientCreatorRequest creator, ClientLinkerRequest linker, ClientConnectorRequest connector, ClientDisconnectorRequest disconnector, ClientSenderRequest sender, ProxyEntity proxy, ProxyTagEnum tag = ProxyTagEnum.UNKNOW)
            : base(client, remote, repository, creator, linker, connector, disconnector, sender, proxy, tag)
        {

        }

        /// <summary>
        /// to do , packet reading , this is just a test
        /// </summary>
        /// <param name="stream"></param>
        protected override void OnReceive(MemoryStream stream)
        {
            byte[] arr = stream.ToArray();
            string msg = Encoding.ASCII.GetString(arr);

            logger.Info($"[{_tag}] : {msg}");

            // ONLY FOR TEST ( set account_name and clear_password with your own dofus account information ) set to commentary if you want to disable it
            if (msg.StartsWith("HC"))
            {
                string account_name = "nomdecompte";
                string clear_password = "motdepasse";

                string password_key = msg.Substring(2, msg.Length - 2);

                string version = "1.33.1\n";
                string account_info = $"{account_name}\n{password_encrypt(clear_password, password_key)}\n";
                string af = " Af\n";

                _client_sender.Handle(_client, Encoding.ASCII.GetBytes(version));
                _client_sender.Handle(_client, Encoding.ASCII.GetBytes(account_info));
                _client_sender.Handle(_client, Encoding.ASCII.GetBytes(af));
                return;
            }

            _client_sender.Handle(_remote, Encoding.ASCII.GetBytes(msg));
        }

        private readonly char[] character_array = new char[]{'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p',
    'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', 'A', 'B', 'C', 'D', 'E', 'F',
    'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V',
    'W', 'X', 'Y', 'Z', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '-', '_'};
        private string password_encrypt(string password, string key)
        {
            string result = "#1";

            for(int i = 0; i < password.Length; i++)
            {
                char ch_p = password[i];
                char ch_k = key[i];

                int n1 = ch_p / 16;
                int n2 = ch_p % 16;

                int index = (n1 + ch_k) % character_array.Length;
                int n3 = (n2 + ch_k) % character_array.Length;

                result = result + character_array[index] + character_array[n3];
            }

            return result;
        }
    }
}
