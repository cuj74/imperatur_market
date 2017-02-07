﻿using System;
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

        public ImperaturMarket(string SystemLocation)
        {
            _databaseHandler = ImperaturGlobal.Kernel.Get<IDatabaseHandler>();
            _accountHandler = ImperaturGlobal.Kernel.Get<IAccountHandler>();
            _userHandler = ImperaturGlobal.Kernel.Get<IUserHandler>();
            _tradeHandler = ImperaturGlobal.Kernel.Get<ITradeHandler>();
            _securityHandler = ImperaturGlobal.Kernel.Get<ISecurityHandler>();
            _systemHandler = ImperaturGlobal.Kernel.Get<ISystemHandler>();
            
            ImperaturGlobal.InitHandlers(DatabaseHandler, SystemLocation);


            if (!_systemHandler.VerifySystem(SystemLocation))
            {
                _systemHandler.CreateSystem(SystemLocation);
            }
            //var t = _accountHandler.Accounts();
            //int gg = 0;

            
            
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
            

        }
    
    }
}
