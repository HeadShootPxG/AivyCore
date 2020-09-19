using AivyData.API;
using AivyData.API.Proxy;
using AivyDomain.API;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AivyDofus.Proxy.API
{
    public class OpenProxyConfigurationApi : IApi<ProxyData>
    {
        static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private readonly string _location;        

        public OpenProxyConfigurationApi(string location) 
        {
            _location = location ?? throw new ArgumentNullException(nameof(location));
            if (!File.Exists(location))
            {
                // default entry
                ToFile(new ProxyData() 
                {
                    custom_servers = new ProxyCustomServerData[]
                    {
                        new ProxyCustomServerData()
                        {
                            ServerId = 671,
                            IpAddress = "127.0.0.1",
                            Ports = new short[] { 777 },
                            IsMonoAccount = true,
                            Type = 1                           
                        }
                    }
                });
            }
        }

        private ProxyData FromFile()
        {
            try
            {
                return JsonConvert.DeserializeObject<ProxyData>(File.ReadAllText(_location), new JsonSerializerSettings() { Formatting = Formatting.Indented });
            }
            catch(Exception e)
            {
                logger.Error(e);
                return null;
            }
        }

        private bool ToFile(ProxyData data)
        {
            try
            {
                File.WriteAllText(_location, JsonConvert.SerializeObject(data, Formatting.Indented));
                return File.Exists(_location);
            }
            catch(Exception e)
            {
                logger.Error(e);
                return false;
            }
        }

        public ProxyData GetData(Func<ProxyData, bool> predicat)
        {
            return FromFile();
        }

        public ProxyData UpdateData(ProxyData data)
        {
            if (ToFile(data)) return data;
            throw new ArgumentException();
        }
    }
}
