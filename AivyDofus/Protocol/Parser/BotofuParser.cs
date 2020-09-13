using AivyDofus.Protocol.Elements;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AivyDofus.Protocol.Parser
{
    public class BotofuParser
    {
        readonly string DofusInvokerPath;

        public static readonly string _this_executable_name = AppDomain.CurrentDomain.BaseDirectory;

        public static string _parser_executable_name = Path.Combine(_this_executable_name, "botofu_protocol_parser.exe");
        public static string _output_path = Path.Combine(_this_executable_name, "protocol.json");

        public BotofuParser(string dofusInvoker)
        {
            DofusInvokerPath = dofusInvoker ?? throw new ArgumentNullException(nameof(dofusInvoker));
        }

        public void Parse()
        {
            byte[] _byte_exe = Properties.Resources.botofu_protocol_parser;
            File.WriteAllBytes(_parser_executable_name, _byte_exe);

            Process _parser_process = Process.Start(_parser_executable_name, $"{DofusInvokerPath} ./protocol.json");

            _parser_process.EnableRaisingEvents = true;
            _parser_process.Exited += _parser_process_Exited;
        }

        private void _parser_process_Exited(object sender, EventArgs e)
        {
            _writeIndented();
        }

        private void _writeIndented()
        {
            string not_indented_content = File.ReadAllText(_output_path);
            BotofuProtocol protocol = JsonConvert.DeserializeObject<BotofuProtocol>(not_indented_content);
            File.WriteAllText(_output_path, JsonConvert.SerializeObject(protocol, Formatting.Indented));
        }
    }
}
