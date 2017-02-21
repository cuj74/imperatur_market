using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Imperatur_Market_Core.shared;
using Imperatur_Market_Core.user;
using System.Globalization;
using Imperatur_Market_Core.account;

namespace Imperatur_Market_Core.system
{
    public class SystemHandler : ISystemHandler
    {
        public bool CreateSystem(string SystemLocation)
        {
            try
            {
                Directory.CreateDirectory(SystemLocation);
            }
            catch (Exception ex)
            {
                ImperaturGlobal.GetLog().Error(string.Format("Could not create directory {0}", SystemLocation), ex);
                return false;
            }

            return true;
        }

        public bool ReadSystem(string SystemLocation)
        {
            throw new NotImplementedException();
        }

        public bool VerifySystem(string SystemLocation)
        {
            return Directory.Exists(SystemLocation);
        }

        private void CreateSystemAccounts()
        {


            int AdminUserId = ImperaturGlobal.UserHandler.AddUser(
                 new User
                 {
                     FirstName = "Admin",
                     CultureInfo = CultureInfo.CurrentCulture.Name,
                     UserType = UserType.Admin
                 }
             );
            ImperaturGlobal.AccountHandler.AddAccount(
                new Account
                {
                    AccountType = AccountType.House,
                    Owner = ImperaturGlobal.UserHandler.GetUser(AdminUserId)
                }
            );
            ImperaturGlobal.AccountHandler.AddAccount(
                new Account
                {
                    AccountType = AccountType.Bank,
                    Owner = ImperaturGlobal.UserHandler.GetUser(AdminUserId)
                }
            );


        }


    }
}
