<h2> # AivyCore </h2>
Proxy Modulable

Pour le moment les APIs ne sont pas encore implémenté. 

Voici un exemple de Program permettant d'installer un proxy sur un fichier éxécutable en lançant le proxy sur le port 666

```csharp
using NLog;
using NLog.Config;
using NLog.Targets;
using System;
using AivyData.Entities;
using AivyDomain.API.Proxy;
using AivyDomain.Mappers.Proxy;
using AivyDomain.Repository.Proxy;
using AivyDomain.UseCases.Proxy;

namespace AivyCore
{
    class Program
    {
        static readonly ConsoleTarget log_console = new ConsoleTarget("log_console");
        static readonly LoggingConfiguration configuration = new LoggingConfiguration();
        static readonly Logger logger = LogManager.GetCurrentClassLogger();

        static OpenProxyApi _proxy_api;
        static ProxyEntityMapper _proxy_mapper;
        static ProxyRepository _proxy_repository;

        static ProxyCreatorRequest _proxy_creator;
        static ProxyActivatorRequest _proxy_activator;

        static void Main(string[] args)
        {
            configuration.AddRule(LogLevel.Info, LogLevel.Fatal, log_console);
            LogManager.Configuration = configuration;

            _proxy_api = new OpenProxyApi("./proxy_information_api.json");
            _proxy_mapper = new ProxyEntityMapper();
            _proxy_repository = new ProxyRepository(_proxy_api, _proxy_mapper);

            _proxy_creator = new ProxyCreatorRequest(_proxy_repository);
            _proxy_activator = new ProxyActivatorRequest(_proxy_repository);

            ProxyEntity proxy = _proxy_creator.Handle(@"VOTRE FICHIER EXECUTABLE", 666);
            proxy = _proxy_activator.Handle(proxy, true);

            Console.ReadLine();
        }
    }
}
```

(<a href="https://github.com/Mrpotatosse/AivyCore/blob/master/AivyDofus/Server/DofusServer.cs">exemple pour un serveur dofus</a>)
(<a href="https://github.com/Mrpotatosse/AivyCore/blob/master/AivyDofus/Proxy/DofusProxy.cs">exemple pour un proxy dofus</a>)

<h2> AivyDofus </h2>

La création du proxy et du serveur se passe dans Program.cs  (https://github.com/Mrpotatosse/AivyCore/blob/master/AivyDofus/Program.cs)
Pour l'instant je n'ai fait que le mono-compte , mais si vous voulez modifier c'est oklm

```csharp
// DofusProxy("DOSSIER APP DOFUS", PORT)
DofusProxy proxy = new DofusProxy("EMPLACEMENT DOSSIER APP DOFUS", 666);
proxy.Active(true);

// DofusServer("DOSSIER APP DOFUS", PORT)
DofusServer server = new DofusServer("EMPLACEMENT DOSSIER APP DOFUS", 777);
server.Active(true);
```

<h2> Handlers Proxy </h2>

```csharp
/*
* Il faut mettre l'attribut ProxyHandler pour que la class soit reconnu en tant que IHandler ( si vous ne le mettez pas , le message ne sera pas stocké 
* La class doit être une sous-class de AbstractMessageHandler , et implémentera les fonction Handle() , EndHandle() ( optionel ) , Error(Exception) ( optionel ) et son
* constructeur doit être NomDeVotreClass(Callback, NetworkElement, NetworkContentElement) : base(Callback,NetworkElement,NetworkContentElement) , le constructeur ne peut pas 
* être modifié , sinon il y a une erreur lors de la création
* la class AbstractMesssageHandler contient une fonction Send(bool,ClientEntity,NetworkElement,NetworkContentElement) , elle permet d'envoyer un message avec les arguments 
* bool: si le message provient du client ( donc , message envoyé au serveur )
* ClientEntity: le client auquel on veut envoyer le message 
* NetworkElement: le message à envoyer
* NetworkContentElement: le contenu du message à envoyer
* elle contient aussi une valeur bool IsForwardingData , laissé à true , si les données seront directement transmis sans modification
*
* Lorsque le message provient du client Dofus , client = client Dofus et remote = serveur Dofus 
* Lorsque le message provient du server Dofus , client = serveur Dofus et remote = client Dofus
* Pour faire la différence , il faudra , soit vous fié à _callback._tag , sinon , vous apprenez un peu le protocol , et vous regardez quel packet est envoyé par qui ^^ 
* Il n'y a pour l'instance aucune gestion de l'instanceId , donc les packets peuvent être seulement modifié
* 
* Pour créer un message/type il faut créer un NetworkContentElement de cette forme : 
* new NetworkContentElement()
* {
*   fields = 
*   { 
*      { "nomDeLaPropriété", valeur de la propriété },
*      { "protocol_id" , 0 } // sur certain type , il peut être obligatoire ( dans le protocol c'est si prefixed_by_type_id = true ) 
*      { ... }   
*   }
* }
*
* Toutes les propriétées sont répertorié dans le fichier ./protocol.json dans le fichier éxécutable
*/
[ProxyHandler(ProtocolId = 30)]
public class ServersListMessageHandler : AbstractMessageHandler
{
    static readonly Logger logger = LogManager.GetCurrentClassLogger();

    public override bool IsForwardingData => false;

    public ServersListMessageHandler(ProxyClientReceiveCallback callback, 
                                     NetworkElement element,
                                     NetworkContentElement content)
        : base(callback, element, content)
    {

    }

    public override void Handle()
    {
        IEnumerable<dynamic> _servers = _content["servers"];            
        _content["servers"] = _servers.Append(new NetworkContentElement()
        {
            fields =
            {
                { "isMonoAccount", true },
                { "isSelectable", true },
                { "id", 671 },
                { "type", 1 },
                { "status", 3 },
                { "completion", 0 },
                { "charactersCount", 1 },
                { "charactersSlots", 5 },
                { "date", 1234828800000 }
            }
        }).ToArray();

        Send(false, _callback._remote, _element, _content);
    }
}
```

<h2> Dépendances </h2>

- NLog

- NewtonSoft Json

- EasyHook ( SocketHook de Nameless https://cadernis.fr/index.php?threads/sockethook-injector-alternative-%C3%A0-no-ankama-dll.2221/page-2#post-24796 celui que j'utilise est une ancienne version auquel j'ai appliqué quelque modification )

- Botofu parser ( https://gitlab.com/botofu/protocol_parser ) ( j'ai directement ajouter le .exe aux ressources ducoup le protocol devrait être parser à chaque ouverture du hook  https://github.com/Mrpotatosse/AivyCore/blob/master/AivyDofus/Protocol/Parser/BotofuParser.cs )

