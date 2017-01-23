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

        public ImperaturWebService()
        {
            Get["/imperatur"] = parameters =>
            {
                var feeds = new string[] { "foo", "bar" };
                return Response.AsJson(feeds);
            };

            Get["/transactions"] = parameters =>
            {
                core.ImperaturServiceCore oC = new core.ImperaturServiceCore(@"F:\dev\test3");
                List<IAccountInterface> oIA = oC.ImperaturMarket.GetAccountHandler().Accounts();
                var feeds = oIA.Where(x => x.GetAccountType().Equals(AccountType.Customer)).Select(f => f.AccountName).ToArray();

                var t = feeds.Select(x => new
                {
                    row = string.Format("<tr><td>{0}</td></tr>", x)
                });
                string res = "";
                foreach (string gf in t.Select(z=>z.row))
                {
                    res += gf;
                }
                var r = "<table>" + res + "</table>";
      


                return Response.AsJson(feeds);
            };
        }
    }
}
