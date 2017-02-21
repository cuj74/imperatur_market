using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace Imperatur_Market_Core.user
{
    public enum UserType
    {
        Admin,
        Regular
    }

    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string IdNumber { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string PostalCode { get; set; }
        public string CultureInfo { get; set; }
        public string HashedPassword { get; set; }
        public byte[] Salt { get; set; }
        public UserType UserType { get; set; }
    }
}
