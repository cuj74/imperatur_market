using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Imperatur_Market_Core.entity;
using Imperatur_Market_Core.user;

namespace Imperatur_Market_Core.account
{
    public enum AccountType
    {
        Customer,
        Bank,
        House
    }

    public class Account : IAccount
    {
        public IUser Owner;
        private  AccountType AccountType;

    }
}
