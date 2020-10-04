using AivyDofus.IO;
using AivyDofus.Protocol.Elements;
using AivyDofus.Protocol.Elements.Fields;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AivyDofus.Protocol.Buffer
{
    public class MessageDataBufferReader
    {
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

        public MessageDataBufferReader(NetworkElement network)
        {
            _network_content = new NetworkContentElement();
            _network_base = network ?? throw new ArgumentNullException(nameof(network));
        }

        public NetworkContentElement Parse(BigEndianReader reader)
        {
            _parse_super(reader);
            _parse_bool(reader);
            _parse_var(reader);

            return _network_content;
        }

        private void _parse_super(BigEndianReader reader)
        {
            if (_network_base.super_serialize)
                _network_content += new MessageDataBufferReader(_super).Parse(reader);
        }

        private void _parse_bool(BigEndianReader reader)
        {
            IEnumerable<ClassField> bools = _network_base.fields.Where(x => x.use_boolean_byte_wrapper).OrderBy(x => x.boolean_byte_wrapper_position);

            byte flag = 0;

            foreach(ClassField field in bools)
            {
                int _wrapper_pos = field.boolean_byte_wrapper_position.Value - 1;

                if (_wrapper_pos < 0) throw new ArgumentOutOfRangeException(nameof(_wrapper_pos));

                if (_wrapper_pos % 8 == 0)
                    flag = reader.ReadByte();

                _network_content[field.name] = BooleanByteWrapper.GetFlag(flag, (byte)(_wrapper_pos % 8));
            }
        }

        private void _parse_var(BigEndianReader reader)
        {
            IEnumerable<ClassField> vars = _network_base.fields.Where(x => !x.use_boolean_byte_wrapper).OrderBy(x => x.position);

            foreach(ClassField field in vars)
            {
                if (field.is_vector || field.type == "ByteArray")
                {
                    dynamic[] array;
                    if (field.constant_length.HasValue)
                    {
                        array = new dynamic[field.constant_length.Value];
                    }
                    else
                    {
                        string read_length_method = $"Read{field.write_length_method.Replace("write", "")}";
                        dynamic _length = _read_value(read_length_method, reader);

                        array = new dynamic[_length];
                    }

                    if (field.type == "ByteArray")
                    {
                        array = _read_value("ReadBytes", reader, array.Length) as dynamic[];
                    }
                    else
                    {
                        for (int i = 0; i < array.Length; i++)
                        {
                            array[i] = _parse_var_element(field, reader);
                        }
                    }

                    _network_content[field.name] = array;
                }
                else
                {
                    _network_content[field.name] = _parse_var_element(field, reader);
                }
            }
        }

        private dynamic _parse_var_element(ClassField field, BigEndianReader reader)
        {
            if (_is_primitive(field))
            {
                string read_method = $"Read{field.write_method.Replace("write", "")}";
                return _read_value(read_method, reader);
            }
            else
            {
                if(field.write_false_if_null_method != null &&
                   field.write_false_if_null_method != "")
                {
                    string check_null_method = $"Read{field.write_false_if_null_method.Replace("write", "")}";
                    if (_read_value(check_null_method, reader) == 0) return null;
                }

                MessageDataBufferReader _type_reader;

                if (field.prefixed_by_type_id)
                {
                    string read_id_method = $"Read{field.write_type_id_method.Replace("write", "")}";
                    dynamic protocol_id = _read_value(read_id_method, reader);

                    _type_reader = new MessageDataBufferReader(BotofuProtocolManager.Protocol[ProtocolKeyEnum.Types, x => x.protocolID == protocol_id]);
                    _type_reader._network_content["protocol_id"] = protocol_id;
                }
                else
                {
                    _type_reader = new MessageDataBufferReader(BotofuProtocolManager.Protocol[ProtocolKeyEnum.Types, x => x.name == field.type]);
                }

                return _type_reader.Parse(reader);
            }
        }

        private dynamic _read_value(string read_method, BigEndianReader reader, params object[] param)
        {
            return typeof(BigEndianReader).GetMethod(read_method).Invoke(reader, param);
        }
    }
}
