using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Imperatur_v2.handler;
using Imperatur_v2.monetary;
using Imperatur_v2.cache;
using Imperatur_v2.shared;
using Ninject;
using System.Reflection;
using System.IO;
using Imperatur_v2.json;
using Newtonsoft.Json.Linq;
using Imperatur_v2.account;
using Imperatur_v2.securites;
using Imperatur_v2.trade.analysis;

namespace Imperatur_v2
{
    public static class ImperaturContainer
    {
        /// <summary>
        /// Build the Imperatur Market Container
        /// </summary>
        /// <param name="SystemLocation">The directory of the system</param>
        /// <returns></returns>
        public static IImperaturMarket BuildImperaturContainer(string SystemLocation)
        {
            //Ninject bindings
            var kernel = new StandardKernel();
            kernel.Load(Assembly.GetExecutingAssembly());

            var ImpertaturContainer = kernel.Get<IImperaturMarket>(
                new Ninject.Parameters.ConstructorArgument("SystemLocation", SystemLocation)
                );

            return ImpertaturContainer;
        }
        public static IImperaturMarket BuildImperaturContainer(ImperaturData NewSystemData)
        {
            //Ninject bindings
            var kernel = new StandardKernel();
            kernel.Load(Assembly.GetExecutingAssembly());

            var ImpertaturContainer = kernel.Get<IImperaturMarket>(
                new Ninject.Parameters.ConstructorArgument("SystemData", NewSystemData)
                );

            return ImpertaturContainer;
        }

    }
    /// <summary>
    /// IOC of Imperatur Market
    /// </summary>
    public interface IImperaturMarket
    {


        /// <summary>
        /// Returns the last known error message that has occurred
        /// </summary>
        /// <returns>string</returns>
        string GetLastErrorMessage();
        ImperaturData GetSystemData();
        IAccountHandlerInterface GetAccountHandler();
        ITradeHandlerInterface GetTradeHandler();
        //IMoney GetMoney(decimal Amount, string CurrencyCode);
        event ImperaturMarket.QuoteUpdateHandler QuoteUpdateEvent;
        IMoney GetMoney(decimal Amount, string CurrencyCode);
        bool SetAutomaticTrading(bool Value);


    }


    public class ImperaturMarket : IImperaturMarket
    {
        private IAccountHandlerInterface m_oAccountHandler;
        private ITradeHandlerInterface m_oTradeHandler;
        private ICurrency m_oDisplayCurrency;
        private string m_oLastErrorMessage;
        private ImperaturData m_oImperaturData;
        private readonly string SystemDataFile = "imperatursettings.json";
     
        private System.Timers.Timer m_oQuoteTimer;



        public delegate void QuoteUpdateHandler(object sender, EventArgs e);
        public event QuoteUpdateHandler QuoteUpdateEvent;

        #region proctectedMethods
        protected virtual void OnQuoteUpdate(EventArgs e)
        {
            if (QuoteUpdateEvent != null)
                QuoteUpdateEvent(this, e);
        }
        #endregion


        #region General
        public string GetLastErrorMessage()
        {
            return m_oLastErrorMessage ?? "";
        }
        #endregion


        #region constructor
        public ImperaturMarket(ImperaturData SystemData)
        {
            //create the system based on the data
            if (!Directory.Exists(SystemData.SystemDirectory))
            {
                CreateImperaturDataFromSystemData(SystemData);
            }
            CreateImperaturMarket(SystemData);

        }


        public ImperaturMarket(string SystemLocation)
        {
            CreateImperaturMarket(ReadImperaturDataFromSystemLocation(SystemLocation));
        }
        #endregion


        #region public methods
        public ImperaturData GetSystemData()
        {
            return m_oImperaturData;
        }

        public bool SetAutomaticTrading(bool Value)
        {
            bool SaveValue = false;
            if (m_oImperaturData.IsAutomaticMaintained != Value)
            {
                SaveValue = true;
            }
            m_oImperaturData.IsAutomaticMaintained = Value;
            if (SaveValue)
            {
                CreateSystemSettingsFile(m_oImperaturData);
            }
            
            return true;
        }

         public IAccountHandlerInterface GetAccountHandler()
        {
            if (m_oAccountHandler == null)
            {
                m_oAccountHandler = ImperaturGlobal.Kernel.Get<IAccountHandlerInterface>();
            }
            return m_oAccountHandler;
        }
        #endregion

        #region private methods

        private void M_oQuoteTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            m_oTradeHandler.ForceUpdate();
        }

        private ImperaturData ReadImperaturDataFromSystemLocation(string SystemLocation)
        {
            ImperaturData oD = new ImperaturData();
            try
            {
                //JObject oJ = (JObject)DeserializeJSON.DeserializeObjectFromFile(string.Format(@"{0}\{1}", SystemLocation, SystemDataFile));
                //oD = oJ.ToObject<ImperaturData>();
                oD = (ImperaturData)DeserializeJSON.DeserializeObjectFromFile(string.Format(@"{0}\{1}", SystemLocation, SystemDataFile));
            }
            catch (Exception ex)
            {
                int ff = 0;
            }
            return oD;
            //return (ImperaturData)DeserializeJSON.DeserializeObjectFromFile(string.Format(@"{0}\{1}", SystemLocation, SystemDataFile));
        }
        private bool CreateImperaturDataFromSystemData(ImperaturData Systemdata)
        {
            if (
                CreateDirectory(Systemdata.SystemDirectory)
                &&
                CreateDirectory(string.Format(@"{0}\{1}", Systemdata.SystemDirectory, Systemdata.AcccountDirectory))
                &&
                CreateDirectory(string.Format(@"{0}\{1}\{2}", Systemdata.SystemDirectory, Systemdata.QuoteDirectory, Systemdata.DailyQuoteDirectory))
                &&
                CreateDirectory(string.Format(@"{0}\{1}\{2}", Systemdata.SystemDirectory, Systemdata.QuoteDirectory, Systemdata.HistoricalQuoteDirectory))
                &&
                CreateSystemSettingsFile(Systemdata)
                )
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private bool CreateDirectory(String NewDirectoryToCreate)
        {
            try
            {
                Directory.CreateDirectory(NewDirectoryToCreate);
            }
            catch (Exception ex)
            {
                m_oLastErrorMessage = ex.Message;
                return false;
            }
            return true;
        }
        private bool CreateSystemSettingsFile(ImperaturData SystemData)
        {
            if (SerializeJSONdata.SerializeObject(SystemData, string.Format(@"{0}\{1}", SystemData.SystemDirectory, SystemDataFile)))
                return true;
            else
            {
                m_oLastErrorMessage = string.Format("Could not save settings file to {0}", SystemData.SystemDirectory);
                return false;
            }
        }

        private StandardKernel InitiateNinjectKernel()
        {
            try
            {
                StandardKernel kernel = new StandardKernel();
                kernel.Load(Assembly.GetExecutingAssembly());
                return kernel;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Could not initiate Imperatur Market application: {0}", ex.Message));
            }
        }
        private void CreateImperaturMarket(ImperaturData SystemData)
        {

            m_oImperaturData = SystemData;
            ImperaturGlobal.Initialize(m_oImperaturData, InitiateNinjectKernel(), null);
            List<account.AccountCacheType> BusinessAccounts = new List<account.AccountCacheType>();

            List<IAccountInterface> oLAB = new List<IAccountInterface>();
            if (GetAccountHandler().Accounts().Where(a => !a.GetAccountType().Equals(account.AccountType.Bank)).Count() == 0)
            {
                //create internalbankaccount for balancetransactions
                //start by create the bankaccount
                oLAB.Add(
                    ImperaturGlobal.Kernel.Get<IAccountInterface>(
                        new Ninject.Parameters.ConstructorArgument("Customer", (object)null),
                        new Ninject.Parameters.ConstructorArgument("AccountType", AccountType.Bank),
                        new Ninject.Parameters.ConstructorArgument("AccountName", "INTERNALBANK")
                    )
                    );
            }
            if (GetAccountHandler().Accounts().Where(a => !a.GetAccountType().Equals(account.AccountType.House)).Count() == 0)
            {
                //create internalbankaccount for balancetransactions
                //start by create the bankaccount
                oLAB.Add(
                    ImperaturGlobal.Kernel.Get<IAccountInterface>(
                        new Ninject.Parameters.ConstructorArgument("Customer", (object)null),
                        new Ninject.Parameters.ConstructorArgument("AccountType", AccountType.House),
                        new Ninject.Parameters.ConstructorArgument("AccountName", "INTERNALHOUSE")
                    )
                    );
            }
            GetAccountHandler().CreateAccount(oLAB);
            //add all business accounts to cache
            BusinessAccounts = GetAccountHandler().Accounts().Where(a => !a.GetAccountType().Equals(account.AccountType.Customer)).
                Select(b =>
                new account.AccountCacheType
                {
                    AccountType = b.GetAccountType(),
                    Identifier = b.Identifier
                }).ToList();

            ImperaturGlobal.InitializeBusinessAccount(BusinessAccounts);
            ImperaturGlobal.Quotes = GetTradeHandler().GetQuotes();

            m_oQuoteTimer = new System.Timers.Timer();
            m_oQuoteTimer.Elapsed += M_oQuoteTimer_Elapsed;
            m_oQuoteTimer.Interval = 1000 * 60 * 2; Convert.ToInt32(m_oImperaturData.QuoteRefreshTime); //every 15 minutes
            m_oQuoteTimer.Enabled = true;

            m_oDisplayCurrency = ImperaturGlobal.Kernel.Get<ICurrency>(new Ninject.Parameters.ConstructorArgument("CurrencyCode", m_oImperaturData.SystemCurrency));
            m_oTradeHandler.QuoteUpdateEvent += M_oTradeHandler_QuoteUpdateEvent;
            return;
            if (m_oImperaturData.IsAutomaticMaintained)
            {

                int[] Intervals = Enumerable.Range(30, 365).ToArray();//new int[] { 30,50,70,90,150,180,240,260,360,720 };
                IAccountInterface oA = m_oAccountHandler.Accounts().Where(a => a.GetAccountType().Equals(AccountType.Customer)).Take(10).Last();
                foreach (Instrument i in ImperaturGlobal.Instruments)
                {
                   /*
                    if (oA.GetAvailableFunds(new List<ICurrency> { GetMoney(0, i.CurrencyCode).CurrencyCode }).Count > 0 && oA.GetAvailableFunds(new List<ICurrency> { GetMoney(0, i.CurrencyCode).CurrencyCode }).First().Amount < 1000m)
                    {
                        continue;
                    }*/
                    trade.analysis.SecurityAnalysis oSA = new trade.analysis.SecurityAnalysis(i);


                    if (!oSA.HasValue)
                    {
                        continue;
                    }
                    TradingRecommendation oTR = new TradingRecommendation();
                    foreach (int Interval in Intervals)
                    {
                        if (oSA.RangeConvergeWithElliotForBuy(DateTime.Now.AddDays(-Interval), DateTime.Now, out oTR))
                        {
                            int dfgdf = 0;
                        }

                    }
                    /*
                    foreach (int Interval in Intervals)
                    {
                        if (oA.GetAvailableFunds(new List<ICurrency> { GetMoney(0, i.CurrencyCode).CurrencyCode }).Count > 0 && oA.GetAvailableFunds(new List<ICurrency> { GetMoney(0, i.CurrencyCode).CurrencyCode }).First().Amount < 1000m)
                        {
                            break;
                        }
                        decimal SaleValue;
                        if (oSA.RangeConvergeWithElliotForBuy(Interval, out SaleValue))
                        {
                            
                            //calculate how many we can buy
                            //decimal Avail = oA.GetAvailableFunds(new List<ICurrency> { GetMoney(0, i.CurrencyCode).CurrencyCode }).First().Amount;
                            //decimal price = ImperaturGlobal.Quotes.Where(q => q.Symbol.Equals(i.Symbol)).First().LastTradePrice.Amount;

                            int Quantity = (int) (oA.GetAvailableFunds(new List<ICurrency> { GetMoney(0, i.CurrencyCode).CurrencyCode }).First().Amount
                                /
                                ImperaturGlobal.Quotes.Where(q => q.Symbol.Equals(i.Symbol)).First().LastTradePrice.Amount);


                            //ImperaturGlobal.Quotes.Where(q => q.Symbol.Equals(comboBox_Symbols.SelectedItem.ToString())).First().LastTradePrice.Multiply(Convert.ToDecimal(QuantityToBuy))
                            m_oAccountHandler.Accounts()[0].AddHoldingToAccount(
                                Quantity - 1,
                                i.Symbol,
                                m_oTradeHandler
                                );

                            
                        }
                    }*/
                }
            }

        }

        private void M_oTradeHandler_QuoteUpdateEvent(object sender, EventArgs e)
        {
            m_oTradeHandler.CacheQuotes();
            OnQuoteUpdate(e);
            ImperaturGlobal.Quotes = m_oTradeHandler.GetQuotes();
            //this is also the trading robot start. 
            if (m_oImperaturData.IsAutomaticMaintained)
            {
                //Add code here!
            }
        }

        public ITradeHandlerInterface GetTradeHandler()
        {
            if (m_oTradeHandler == null)
                m_oTradeHandler = ImperaturGlobal.Kernel.Get<ITradeHandlerInterface>();

            return m_oTradeHandler;
        }

        public IMoney GetMoney(decimal Amount, string CurrencyCode)
        {
            return ImperaturGlobal.GetMoney(Amount, CurrencyCode);
        }
        #endregion


    }
}
