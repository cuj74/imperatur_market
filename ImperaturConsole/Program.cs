using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Imperatur_Market_Core;

namespace ImperaturConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Imperatur_Market_Core.ImperaturMarket oM = new ImperaturMarket(@"f:\dev\test10");
            Console.WriteLine(oM.UserHandler.GetUser(1)._firstName + "sdfsdf");
           // Console.ReadLine();
            //oM.AccountHandler.GetAccount()
            foreach(var a in oM.AccountHandler.Accounts())
            {
                Console.WriteLine("name:"+a.Owner._firstName);
            }
            Console.ReadLine();
            int gfdsg = 0;
        }
    }
}
