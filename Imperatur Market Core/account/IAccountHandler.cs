using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imperatur_Market_Core.account
{
    public interface IAccountHandler : IDisposable
    {
        ICollection<IAccount> Accounts();
        IAccount GetAccount(IAccount Account);
        bool AddAccount(IAccount Account);
    }
}
