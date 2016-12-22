using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Imperatur_v2.shared;

namespace Imperatur_v2.customer
{
    [DesignAttribute(true)]
    public class Customer
    {
        public string FirstName;
        public string LastName;
        public string Street;
        public string Postalcode;
        public string City;
        public string Idnumber;

        [DesignAttribute(true)]
        public string FullName
        {
            get {
                return string.Format("{0} {1}", FirstName ?? "", LastName ?? "");
            }
        }
    }
}
