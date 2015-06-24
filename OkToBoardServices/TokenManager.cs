using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace OkToBoardServices
{
    public class TokenManager
    {
        public static bool ValidateToken(string token)
        {
            var key = ConfigurationManager.AppSettings["TokenKey"];
            var bytes = new UnicodeEncoding().GetBytes(key);
            var hashed = new SHA512Managed().ComputeHash(bytes);
            var serverToken = hashed.Aggregate<byte, string>("", (s, b) => s += string.Format("{0:x2}", b));            
            return token == serverToken;
        }
    }
}