using Imperatur_Market_Core.entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Imperatur_Market_Core.user;
using LiteDB;
using Imperatur_Market_Core.database;

namespace Imperatur_Market_Core.account
{
    public interface IAccount 
    {
        IUser Owner { get; set; }
        AccountType AccountType { get; set; }
    }
}
