using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Imperatur.account;
using Imperatur.monetary;
using Imperatur;
using Newtonsoft.Json;
using System.Reflection;
using System.Threading;

namespace Imperatur_Test
{
    class Program
    {
        private static Imperatur.ImperaturContainer Ic;
        static void Main(string[] args)
        {
            string SystemLocation = "";
            decimal InitialDeposit = 0;
            if (args == null || args.Count() == 0) //no system specified
            {
                Console.WriteLine("Exiting...");
                return;
            }
            try
            {
                SystemLocation = args[0];
                InitialDeposit = Convert.ToDecimal(args[1]);
            }
            catch(Exception ex)
            {
                Console.WriteLine(string.Format("Error in input data {0}", ex.Message));
            }
            if (!Directory.Exists(SystemLocation))
            {
                Console.WriteLine(string.Format("{0} doesn't exists, create new system and test? (y/n)", SystemLocation));
                ConsoleKeyInfo info = Console.ReadKey();
                if (info.Key != ConsoleKey.Y)
                {
                    Console.WriteLine("Exiting...");
                    return;
                }
                if (!CreateSystemForTest(SystemLocation, InitialDeposit, 200))
                    Console.WriteLine("Couldn't create system");


                return;
            }
            else
            {
                Ic = new ImperaturContainer(SystemLocation);
                if (!File.Exists(string.Format(@"{0}\{1}", SystemLocation, "accounttrade.json")))
                {
                    if (!CreateSystemForTest(SystemLocation, InitialDeposit, 200))
                        Console.WriteLine("Couldn't create system");
                }
            }
            RunBuySellProcess(SystemLocation, Ic);
        }

        private static bool RunBuySellProcess(string SystemLocation, ImperaturContainer Ic)
        {
            //börja med att köra igenom alla körningar för att hitta rätt typ av värdepapper att köpa.
            var Tickers = Ic.GetQuotes().Where(h=>h.Dividend.CurrencyCode.Equals("SEK")).GroupBy(h => h.Symbol)
               .Select(grp => grp.First())
               .ToList();
            List<InstrumentRating> InstrumentRating = new List<Imperatur_Test.InstrumentRating>();
            foreach(Imperatur.cache.Quote oQ in Tickers)
            {
                if (oQ.Symbol.Equals("ABB"))
                {
                    //predict the historical analysis of the instrument
                    //Get the historical data for 12 months back
                    List<HistoricalStock> HistData = HistoricalStockDownloader.DownloadData(oQ.Symbol, 12);
                    int gg = 0;
                }


            }

            return true;
        }


        private static bool CreateTestSystem(ImperaturContainer Ic, int NumberOfAccounts, string SystemLocation, Money InitialDeposit)
        {
            List<Account> HouseAndBank = new List<Account>();
            HouseAndBank.Add(new Account("House", AccountType.House));
            HouseAndBank.Add(new Account("Bank", AccountType.Bank));
            Ic.AccountHandler.CreateAccount(HouseAndBank);
            List<Account> CustomerAccounts = new List<Account>();
            for (int i = 0; i < 200; i++)
            {
                CustomerAccounts.Add(new Account(string.Format("Customer{0}", i.ToString()), AccountType.Customer));
            }
            Ic.AccountHandler.CreateAccount(CustomerAccounts);

            //deposit on each account
            Ic.AccountHandler.Accounts.Where(a => a.AccountType.Equals(AccountType.Customer)).ToList().ForEach(a => Ic.AccountHandler.DepositAmount(a.Identifier, InitialDeposit));

            List<string> oDistribution = new List<string>
            {
                "HistoricalAnalysis",
                "StandardDeviation3M",
                "StandardDeviation12M",
                "InternetSearch",
                "RSSSearch",
                "TwitterSearch"
            };

            List<AccounTradingSettings> oATS = new List<AccounTradingSettings>();
            //create account settings for each account
            for (int i = 0; i < Ic.AccountHandler.Accounts.Count(); i++)
            {
                AccounTradingSettings oAT = new AccounTradingSettings();
                oAT.AccountIdentifier = Ic.AccountHandler.Accounts[i].Identifier;
                oDistribution.Shuffle();
                int id = 1;
                foreach(string oD in oDistribution)
                {
                    switch (oD)
                    {
                        case "HistoricalAnalysis":
                            {
                                oAT.HistoricalAnalysis = id; 
                                break;
                            }
                        case "StandardDeviation3M":
                            {
                                oAT.StandardDeviation3M = id;
                                break;
                            }
                        case "StandardDeviation12M":
                            {
                                oAT.StandardDeviation12M = id;
                                break;
                            }
                        case "InternetSearch":
                            {
                                oAT.InternetSearch = id;
                                break;
                            }
                        case "RSSSearch":
                            {
                                oAT.RSSSearch = id;
                                break;
                            }
                        case "TwitterSearch":
                            {
                                oAT.TwitterSearch = id;
                                break;
                            }
                    }
                    id = id + 6;
                }



                oATS.Add(
                    new AccounTradingSettings
                    {
                        AccountIdentifier = oAT.AccountIdentifier,
                        HistoricalAnalysis = oAT.Getpercentage("HistoricalAnalysis"),
                        StandardDeviation3M = oAT.Getpercentage("StandardDeviation3M"),
                        StandardDeviation12M = oAT.Getpercentage("StandardDeviation12M"),
                        InternetSearch = oAT.Getpercentage("InternetSearch"),
                        RSSSearch = oAT.Getpercentage("RSSSearch"),
                        TwitterSearch = oAT.Getpercentage("TwitterSearch")
                    }
                    );
                        
            }
            //save all accounttradesettings
            return SaveAllAccountTradeSetting(oATS, SystemLocation);
        }

      
        private static bool CreateSystemForTest(string SystemLocation, decimal InitalDeposit, int NumberOfAccounts)
        {
            if (!Directory.Exists(SystemLocation))
            {
                try
                {
                    Directory.CreateDirectory(SystemLocation);
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
            Ic = new ImperaturContainer(SystemLocation);
            //create 200 accounts + house and bank
            return CreateTestSystem(Ic, NumberOfAccounts, SystemLocation, new Money(InitalDeposit, "SEK"));
           
        }
        public static bool SaveAllAccountTradeSetting(List<AccounTradingSettings> oAccountsTradeSetting, string SystemLocaction)
        {
            //var SerializeSettings = new JsonSerializerSettings() { ContractResolver = new JsonContractResolver() };
            //var json = JsonConvert.SerializeObject(obj, settings);

            using (FileStream fs = File.Open(@SystemLocaction + @"\accounttrade.json", FileMode.Create))
            using (StreamWriter sw = new StreamWriter(fs))
            {
                sw.Write(SerializeAllFields.Dump(oAccountsTradeSetting, true));
            }
            /*
            using (JsonTextWriter jw = new JsonTextWriter(sw))
            {
                jw.Formatting = Formatting.Indented;

                JsonSerializer serializer = new JsonSerializer();
                serializer.s
                //serializer.Serialize(jw, oAccounts);
            }*/
            return true;
        }
    }



    public static class SerializeAllFields
    {
        public static string Dump(object o, bool indented = true)
        {
            var settings = new Newtonsoft.Json.JsonSerializerSettings() { ContractResolver = new AllFieldsContractResolver() };
            if (indented)
            {
                settings.Formatting = Newtonsoft.Json.Formatting.Indented;
            }
            return Newtonsoft.Json.JsonConvert.SerializeObject(o, settings);
        }
    }

    public class AllFieldsContractResolver : Newtonsoft.Json.Serialization.DefaultContractResolver
    {
        protected override IList<Newtonsoft.Json.Serialization.JsonProperty> CreateProperties(Type type, Newtonsoft.Json.MemberSerialization memberSerialization)
        {
            var props = type
                .GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Select(p => base.CreateProperty(p, memberSerialization))
                .Union(type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Select(f => base.CreateProperty(f, memberSerialization)))
                .ToList();
            props.ForEach(p => { p.Writable = true; p.Readable = true; });
            return props;
        }
    }

    
}
