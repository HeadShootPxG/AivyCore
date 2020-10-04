using AivyDofus.IO;
using AivyDofus.Protocol.Elements;
using AivyDofus.Protocol.Elements.Fields;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AivyDofus.Protocol.Buffer
{
    public class MessageDataBufferWriter
    {
        static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private static bool _is_primitive(ClassField field)
        {
            return BotofuProtocolManager.Protocol[ProtocolKeyEnum.MessagesAndTypes, x => x.name == field.type] is null;
        }

        private NetworkContentElement _network_content;
        private readonly NetworkElement _network_base;

        private NetworkElement _super
        {
            get
            {
                return BotofuProtocolManager.Protocol[ProtocolKeyEnum.MessagesAndTypes, x => x.name == _network_base.super];
            }
        }

        public MessageDataBufferWriter(NetworkElement network)
        {
            _network_base = network ?? throw new ArgumentNullException(nameof(network));
        }

        public byte[] Parse(NetworkContentElement content)
        {
            _network_content = content ?? throw new ArgumentNullException(nameof(content));

            using(BigEndianWriter writer = new BigEndianWriter())
            {
                _parse_super(writer);
                _parse_bool(writer);
                _parse_var(writer);
                
                return writer.Data;
            }
        }

        private void _parse_super(BigEndianWriter writer)
        {
            if (_network_base.super_serialize)            
                writer.WriteBytes(new MessageDataBufferWriter(_super).Parse(_network_content));            
        }

        private void _parse_bool(BigEndianWriter writer)
        {
            IEnumerable<ClassField> bools = _network_base.fields.Where(x => x.use_boolean_byte_wrapper).OrderBy(x => x.boolean_byte_wrapper_position);

            if (bools.Count() == 0)
                return;

            byte[] flags = new byte[bools.LastOrDefault().position.Value + 1];

            foreach (ClassField _bool in bools)
            {
                flags[_bool.position.Value] = BooleanByteWrapper.SetFlag(flags[_bool.position.Value], 
                                                                        (byte)((_bool.boolean_byte_wrapper_position.Value - 1) % 8), 
                                                                        _network_content[_bool.name]);
            }

            writer.WriteBytes(flags);
        }

        private void _parse_var(BigEndianWriter writer)
        {
            IEnumerable<ClassField> vars = _network_base.fields.Where(x => !x.use_boolean_byte_wrapper).OrderBy(x => x.position);

            foreach (ClassField field in vars)
            {
                if (field.is_vector || field.type == "ByteArray")
                {
                    dynamic array = _network_content[field.name];

                    if (!field.constant_length.HasValue)
                    {
                        string write_length_method = field.write_length_method.Replace("write", "Write");
                        if(array is null)
                        {
                            _write_value(write_length_method, 0, writer);
                            return;
                        }
                        _write_value(write_length_method, array.Length, writer);
                    }

                    if (array is byte[] _byte_array)
                    {
                        _write_value("WriteBytes", _byte_array, writer);
                    }
                    else
                    {
                        for (int i = 0; i < array.Length; i++)
                        {
                            _parse_var_element(field, array[i], writer);
                        }
                    }
                }
                else
                {
                    _parse_var_element(field, _network_content[field.name], writer);
                }
            }
        }

        private void _parse_var_element(ClassField field, dynamic value, BigEndianWriter writer)
        {
            if (_is_primitive(field))
            {
                string write_method = field.write_method.Replace("write", "Write");
                _write_value(write_method, value, writer);
            }
            else
            {
                bool is_null = value is null;

                if (field.write_false_if_null_method != null &&
                   field.write_false_if_null_method != "" &&
                   is_null)
                {
                    string check_null_method = field.write_false_if_null_method.Replace("write", "Write");
                    _write_value(check_null_method, 0, writer);
                    return;
                }

                if (is_null) throw new ArgumentNullException(nameof(value));

                NetworkElement var_type = null;

                if (field.prefixed_by_type_id)
                {
                    string write_type_id_method = field.write_type_id_method.Replace("write", "Write");
                    _write_value(write_type_id_method, value["protocol_id"], writer);

                    var_type = BotofuProtocolManager.Protocol[ProtocolKeyEnum.Types, x => x.protocolID == value["protocol_id"]];
                }
                else
                {
                    var_type = BotofuProtocolManager.Protocol[ProtocolKeyEnum.Types, x => x.name == field.type];
                }

                MessageDataBufferWriter _writer = new MessageDataBufferWriter(var_type);
                writer.WriteBytes(_writer.Parse(value));
            }
        }

        private void _write_value(string write_method, dynamic value, BigEndianWriter writer)
        {
            try
            {
                typeof(BigEndianWriter).GetMethod(write_method).Invoke(writer, new object[] { value });
            }
            catch (AmbiguousMatchException)
            {
                typeof(BigEndianWriter).GetMethods().FirstOrDefault(x => x.Name == write_method && x.GetParameters().FirstOrDefault(p => p.ParameterType == typeof(int)) != null).Invoke(writer, new object[] { value });
            }
        }
    }
}
