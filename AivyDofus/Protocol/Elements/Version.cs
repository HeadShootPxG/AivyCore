using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AivyDofus.Protocol.Elements
{
    public class Version
    {
        public ClientVersion client { get; set; }
        public ParserVersion parser { get; set; }
        public ProtocolVersion protocol { get; set; }
    }

    public class ClientVersion
    {
        public BuildVersion build { get; set; }
        public GameVersion version { get; set; }
    }

    public class BuildVersion
    {
        public VersionDate date { get; set; }
        public string type { get; set; }
    }

    public class GameVersion
    {
        public int major { get; set; }
        public int minor { get; set; }
        public int patch { get; set; }
        public int build { get; set; }
    }

    public class ParserVersion
    {
        public int major { get; set; }
        public int minor { get; set; }
    }

    public class VersionDate
    {
        public byte day { get; set; }
        public byte hour { get; set; }
        public byte minute { get; set; }
        public byte month { get; set; }
        public short year { get; set; }
        public string full { get; set; }
    }

    public class ProtocolVersion
    {
        public VersionDate date { get; set; }
        public ProtocolRequired version { get; set; }
    }

    public class ProtocolRequired
    {
        public int current { get; set; }
        public int minimum { get; set; }
    }
}
