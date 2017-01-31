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
        private string _firstName;
        private string _lastName;
        private string _idNumber;
        private string _city;
        private string _street;
        private string _postalCode;
        private CultureInfo _cultureInfo;
        private string _hashedPassword;
        private byte[] _salt;
    }
}
