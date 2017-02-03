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
        [BsonRef("Users")]
        public IUser Owner { get; set; }
        public AccountType AccountType { get; set; }
        public string EntityIdentifier { get; set; }
        public string EntityType { get; set; }
        public IEntity SubEntity { get; set; }
    }
}
