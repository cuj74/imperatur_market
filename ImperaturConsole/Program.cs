using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Imperatur_Market_Core;
using Imperatur_Market_Core.monetary;
using Imperatur_Market_Core.shared;

namespace ImperaturConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Imperatur_Market_Core.ImperaturMarket oM = new ImperaturMarket(@"C:\temp\impe10");
            /*
            var t1 = new Transaction
            {
                Account = oM.AccountHandler.GetAccount(1),
                Monetary = ImperaturGlobal.GetMoney(120),
                TransactionType = "Transfer"
            };
            var t2 = new Transaction
            {
                Account = oM.AccountHandler.GetBankAccount(),
                Monetary = ImperaturGlobal.GetMoney(-120),
                TransactionType = "Transfer"
            };
            var t = new List<Transaction> { t1, t2 };
            oM.LogicalTransactionHandler.AddLogicalTransaction(
                new LogicalTransaction
                {
                    LogDateTime = DateTime.Now,
                    LogicalTransactionType = "Tjolahopp",
                    Transactions = t
                }
                );
            Console.WriteLine(oM.UserHandler.GetUser(1)._firstName + "sdfsdf");
            // Console.ReadLine();
            //oM.AccountHandler.GetAccount()
            foreach (var a in oM.AccountHandler.Accounts())
            {
                Console.WriteLine("name:" + a.Owner._firstName);
            }
            Console.ReadLine();
            int gfdsg = 0;
            */
        }
    }
}
