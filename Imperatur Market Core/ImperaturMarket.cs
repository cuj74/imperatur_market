using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Imperatur_Market_Core.account;
using Imperatur_Market_Core.entity;
using Imperatur_Market_Core.shared;
using Imperatur_Market_Core.user;
using Imperatur_Market_Core.trade;
using Imperatur_Market_Core.securities;
using Imperatur_Market_Core.system;
using Imperatur_Market_Core.database;
using Imperatur_Market_Core.monetary;
using Ninject;


namespace Imperatur_Market_Core
{
    public class ImperaturMarket : IImperaturMarket
    {
        private IAccountHandler _accountHandler;
        private IUserHandler _userHandler;
        private ITradeHandler _tradeHandler;
        private ISecurityHandler _securityHandler;
        private ISystemHandler _systemHandler;
        private IDatabaseHandler _databaseHandler;
        private ILogicalTransactionHandler _logicalTransactionHandler;

        public IAccountHandler AccountHandler
        {
            get
            {
                return _accountHandler;
            }

        }

        public IUserHandler UserHandler
        {
            get
            {
                return _userHandler;
            }

        }

        public ITradeHandler TradeHandler
        {
            get
            {
                return _tradeHandler;
            }

        }

        public ISecurityHandler SecurityHandler
        {
            get
            {
                return _securityHandler;
            }

        }

        public ISystemHandler SystemHandler
        {
            get
            {
                return _systemHandler;
            }

        }

        public IDatabaseHandler DatabaseHandler
        {
            get
            {
                return _databaseHandler;
            }


        }

        public ILogicalTransactionHandler LogicalTransactionHandler
        {
            get
            {
                return _logicalTransactionHandler;
            }
        }

        public ImperaturMarket(string SystemLocation)
        {
            _databaseHandler = ImperaturGlobal.Kernel.Get<IDatabaseHandler>();
            _accountHandler = ImperaturGlobal.Kernel.Get<IAccountHandler>();
            _userHandler = ImperaturGlobal.Kernel.Get<IUserHandler>();
            _tradeHandler = ImperaturGlobal.Kernel.Get<ITradeHandler>();
            _securityHandler = ImperaturGlobal.Kernel.Get<ISecurityHandler>();
            _systemHandler = ImperaturGlobal.Kernel.Get<ISystemHandler>();
            _logicalTransactionHandler = ImperaturGlobal.Kernel.Get<ILogicalTransactionHandler>();

            ImperaturGlobal.InitHandlers(DatabaseHandler, AccountHandler, UserHandler, SystemLocation);


            if (!_systemHandler.VerifySystem(SystemLocation))
            {
                _systemHandler.CreateSystem(SystemLocation);
            }
            //var t = _accountHandler.Accounts();
            //int gg = 0;

            /*
            
            var UserToAdd = new User
            {
                _firstName = "Urban",
                _city = "Slätte",
                _lastName = "Kolkesson",
                _postalCode = "1234",
                _cultureInfo = "sv-se",
                _hashedPassword = "sdf2qfwefwse3fwsf",
                _idNumber = "7451651",
                _salt = new byte[] {22},
                _street = "poasdas vägen"
            };
            int id = _userHandler.AddUser(UserToAdd);
            var AccountToAdd = new Account
            {
                AccountType = AccountType.Customer,
                Owner = _userHandler.GetUser(id)
            };

            _accountHandler.AddAccount(AccountToAdd);

            var HouseUserToAdd = new User
            {
                _firstName = "New Shiny Bolaget AB",
                _city = "Stockholm",
                _lastName = "",
                _postalCode = "11040",
                _cultureInfo = "sv-se",
                _hashedPassword = "sdf2qfwefwse3fwsf",
                _idNumber = "7451651",
                _salt = new byte[] { 22 },
                _street = "fina gatan 22B"
            };

            int Houseid = _userHandler.AddUser(HouseUserToAdd);

            var HouseAccountToAdd = new Account
            {
                AccountType = AccountType.House,
                Owner =_userHandler.GetUser(Houseid)
            };
            int HouseAccountId = _accountHandler.AddAccount(HouseAccountToAdd);

            var sdf = _userHandler.GetUser(Houseid);

            AccountToAdd = new Account
            {
                AccountType = AccountType.Bank,
                Owner = _userHandler.GetUser(Houseid)
            };
            _accountHandler.AddAccount(AccountToAdd);
            */

        }


    
    }
}
