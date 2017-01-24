using System;
using Nancy;
using Imperatur_v2;
using Imperatur_v2.account;
using System.Collections.Generic;
using System.Linq;

namespace ImperaturService
{
    public class ImperaturWebService : NancyModule
    {
        private IImperaturMarket _imperaturMarket;

        public ImperaturWebService(IImperaturMarket imperaturMarket)
        {
            _imperaturMarket = imperaturMarket;

            Get["/"] = _ => View["index"];
            Get["/Accounts"] = parameters =>
            {
                List<IAccountInterface> oIA = _imperaturMarket.GetAccountHandler().Accounts();
                var feeds = oIA.Where(x => x.GetAccountType().Equals(AccountType.Customer)).Select(f => f.AccountName).ToArray();
                var feeds2 = oIA.Where(x => x.GetAccountType().Equals(AccountType.Customer)).Select
                (f => new
                {
                    accountname = f.AccountName,
                    availablefunds = f.GetAvailableFunds().First().ToString(),
                    identifier = f.Identifier,
                    totalfunds = f.GetTotalFunds().First().ToString()
                }).ToArray();
                return Response.AsJson(feeds2);
            };
        }
    }
}
