using NBitcoin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crypto
{
    public class UserInfo
    {
        public int password { get; set; }
        public string login { get; set; }
        public string address { get; set; }
        public BitcoinExtKey key { get; set; }
    }
}