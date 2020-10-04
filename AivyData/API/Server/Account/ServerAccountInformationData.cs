using System;
using System.Collections.Generic;
using System.Text;

namespace AivyData.API.Server.Account
{
    public class ServerAccountInformationData : IdentifiableData
    {
        public string Token { get; set; }
        public DateTime CreationDateTime { get; set; }
        public DateTime LastConnectionDateTime { get; set; }
        public List<string> FriendsToken { get; set; }
        public List<string> IgnoredsToken { get; set; }
        public TimeSpan TotalTimeSpentInGame { get; set; }
    }
}
