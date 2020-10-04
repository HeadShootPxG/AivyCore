using System;
using System.Collections.Generic;
using System.Text;

namespace AivyData.API.Proxy
{
    public class ProxyAccountMinimumInformationData
    {
        public double AccountId { get; set; }
        public string Nickname { get; set; }
        public bool ConnectedToCustomServer { get; set; }

        public override string ToString()
        {
            return $"{AccountId}~{Nickname.ToLower()}";
        }
    }
}
