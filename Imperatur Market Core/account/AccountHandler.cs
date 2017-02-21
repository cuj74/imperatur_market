using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Imperatur_Market_Core.shared;
using LiteDB;

namespace Imperatur_Market_Core.account
{
    public class AccountHandler : IAccountHandler
    {
        public ICollection<Account> Accounts()
        {
            return  GetAccounCollection().FindAll().ToList();
        }

        public int AddAccount(Account AccountToAdd)
        {
            return GetAccounCollection().Insert(AccountToAdd);
        }

        public Account GetAccount(int Id)
        {
            return GetAccounCollection().FindById(Id);
        }

        private LiteDB.LiteCollection<Account> GetAccounCollection()
        {
            return ImperaturGlobal.DatabaseHandler.GetCollectionFromDataBase<Account>().Include(c => c.Owner);
        }

        public Account GetHouseAccount()
        {
            return GetFirstAccountOfType(AccountType.House);
        }

        public Account GetBankAccount()
        {
            return GetFirstAccountOfType(AccountType.Bank);
        }
        private Account GetFirstAccountOfType(AccountType accounttype)
        {
            return GetAccounCollection().Find(x => x.AccountType.Equals(accounttype)).First();
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~AccountHandler() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }


        #endregion
    }
}
