using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace Imperatur_Market_Core.user
{
    public class User : IUser
    {
        public int Id { get; set; }
        public string _firstName { get; set; }
        public string _lastName { get; set; }
        public string _idNumber { get; set; }
        public string _city { get; set; }
        public string _street { get; set; }
        public string _postalCode { get; set; }
        public string _cultureInfo { get; set; }
        public string _hashedPassword { get; set; }
        public byte[] _salt { get; set; }
    }
}
