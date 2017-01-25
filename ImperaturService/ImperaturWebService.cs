using System;
using Nancy;
using Imperatur_v2;
using Imperatur_v2.account;
using System.Collections.Generic;
using System.Linq;
using Imperatur_v2.monetary;
using Imperatur_v2.shared;

namespace ImperaturService
{
    public class ImperaturWebService : NancyModule
    {
        private IImperaturMarket _imperaturMarket;

        public ImperaturWebService(IImperaturMarket imperaturMarket)
        {
            _imperaturMarket = imperaturMarket;
            string RestBase = "/api/";

            Get["/"] = _ => View["index"];

            Get[RestBase + "/accountsearch/{search}"] = parameters =>
            {
                List<IAccountInterface> oIA = _imperaturMarket.GetAccountHandler().SearchAccount(parameters.search, AccountType.Customer);
                var feeds2 = oIA.Select
                (f => new
                {
                    accountname = f.AccountName,
                    availablefunds = f.GetAvailableFunds().First().ToString(),
                    identifier = f.Identifier,
                    totalfunds = f.GetTotalFunds().First().ToString()
                }).ToArray();
                return Response.AsJson(feeds2);
            };

            Get[RestBase + "/account"] = parameters =>
            {
                List<IAccountInterface> oIA = _imperaturMarket.GetAccountHandler().Accounts();
                
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

            Get[RestBase + "account/{id}"] = identifier =>
            {
                IAccountInterface oA = _imperaturMarket.GetAccountHandler().GetAccount(new Guid(identifier.id));

                List<ICurrency> FilterCurrency = new List<ICurrency>();
                FilterCurrency.Add(ImperaturGlobal.GetSystemCurrency());

                IMoney AvailableSystemAmount = oA.GetAvailableFunds(FilterCurrency).First();
                IMoney TotalFunds = oA.GetTotalFunds(FilterCurrency).First();
                IMoney TotalDeposit = oA.GetDepositedAmount(FilterCurrency).First();

                var feeds2 =
                 new
                 {
                     accountname = oA.AccountName,
                     availablefunds = oA.GetAvailableFunds().First().ToString(),
                     identifier = oA.Identifier,
                     totalfunds = oA.GetTotalFunds().First().ToString(),
                     change = string.Format("{0}%", TotalDeposit.Amount > 0 ? TotalFunds.Subtract(TotalDeposit.Amount).Divide(TotalDeposit.Amount).Multiply(100).ToString(true, false) : "0"),
                     transactions = oA.Transactions.Select(t => new
                     {
                         transdate = t.TransactionDate,
                         amount = t.CreditAmount.ToString(),
                         transactiontype = t.TransactionType.ToString()
                     }).ToArray(),
                     customername = oA.GetCustomer().FullName,
                     holdings = oA.GetHoldings().Select(h => new
                     {
                         name = h.Name,
                         change = h.Change.ToString(),
                         aac = oA.GetAverageAcquisitionCostFromHolding(h.Name).ToString(),
                         purchaseamount = h.PurchaseAmount.ToString()
                     }),
                     orders = _imperaturMarket.OrderQueue.GetOrdersForAccount(new Guid(identifier.id)).Select(o=> new
                     {
                         ordertype = o.OrderType.ToString(),
                         symbol = o.Symbol,
                         quantity = o.Quantity.ToString(),
                         validtodate = o.ValidToDate.ToString()
                     })
                 };
                return Response.AsJson(feeds2);
            };


            Get[RestBase +"acount/{id}/holdings"] = identifier => {

                List<Imperatur_v2.trade.Holding> oH = _imperaturMarket.GetAccountHandler().GetAccount(new Guid(identifier.id)).GetHoldings();

                var holdings = oH.Select(h=>
                 new
                 {
                     name = h.Name,
                     change = h.Change,
                     symbol = h.Symbol,
                     currentamount = h.CurrentAmount
                 }).ToArray();
                return Response.AsJson(holdings);

            };
        }
    }
}
