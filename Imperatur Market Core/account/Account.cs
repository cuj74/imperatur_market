using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Imperatur_Market_Core.entity;
using Imperatur_Market_Core.user;
using LiteDB;
using Imperatur_Market_Core.database;

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
        public int Id { get; set; }
        [BsonRef("User")]
        public User Owner { get; set; }
        public AccountType AccountType { get; set; }
    }
}
