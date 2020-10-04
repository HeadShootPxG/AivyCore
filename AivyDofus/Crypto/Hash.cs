using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AivyDofus.Crypto
{
    public class Hash
    {
        public static byte[] md5(string value)
        {
            using(MD5 md5 = MD5.Create())
            {
                return md5.ComputeHash(Encoding.UTF8.GetBytes(value));
            }
        }

        public static string md5_str(string value, bool upperCase = false)
        {
            byte[] bytes = md5(value);
            return Convert.ToBase64String(bytes);
        }
    }
}
