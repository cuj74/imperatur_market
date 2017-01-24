using System;
using System.Collections.Generic;
using System.Linq;
using Imperatur_v2.handler;
using Imperatur_v2.monetary;
using Imperatur_v2.shared;
using Ninject;
using System.Reflection;
using System.IO;
using Imperatur_v2.json;
using Imperatur_v2.account;
using Imperatur_v2.order;
using Imperatur_v2.trade.automation;
using Imperatur_v2.events;
using log4net;
using log4net.Config;
using System.Threading;

namespace Imperatur_v2
{
    public static class ImperaturContainer
    {
        public delegate void SystemNotificationHandler(object sender, IMPSystemNotificationEventArg e);

        public static event SystemNotificationHandler SystemNotificationEvent = delegate { };

        private static IImperaturMarket m_oImperaturMarket;

        private static void CreateMarketObject()
        {
            var kernel = new StandardKernel();
            kernel.Load(Assembly.GetExecutingAssembly());

            m_oImperaturMarket = kernel.Get<IImperaturMarket>();
            try
            {
                m_oImperaturMarket.SystemNotificationEvent += M_oImperaturMarket_SystemNotificationEvent;
            }
            catch(Exception ex)
            {
                int gg = 0;
            }
        }

        private static void M_oImperaturMarket_SystemNotificationEvent(object sender, IMPSystemNotificationEventArg e)
        {
            SystemNotificationEvent(sender, e);
        }

        /// <summary>
        /// Build the Imperatur Market Container
        /// </summary>
        /// <param name="SystemLocation">The directory of the system</param>
        /// <returns></returns>
        public static IImperaturMarket BuildImperaturContainer(string SystemLocation)
        {
            CreateMarketObject();
            m_oImperaturMarket.LoadImperaturMarket(SystemLocation);
            return m_oImperaturMarket;
        }
        public static IImperaturMarket BuildImperaturContainer(ImperaturData NewSystemData)
        {
            CreateMarketObject();
            m_oImperaturMarket.LoadImperaturMarket(NewSystemData);
            return m_oImperaturMarket;
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

        /// <summary>
        /// Returns the handler object for the order queue
        /// </summary>
        IOrderQueue OrderQueue { get; }
        /// <summary>
        /// Get the system data properties
        /// </summary>
        /// <returns></returns>
        ImperaturData GetSystemData();
        /// <summary>
        /// Get the reference to the account handler object
        /// </summary>
        /// <returns>An object of type IAccountHandlerInterface</returns>
        IAccountHandlerInterface GetAccountHandler();
        /// <summary>
        /// Get the reference to the Trand handler object
        /// </summary>
        /// <returns>An object of type ITradeHandlerInterface</returns>
        ITradeHandlerInterface GetTradeHandler();
        /// <summary>
        /// Set the automatic trading value
        /// </summary>
        /// <param name="Value">The value of the Automatic trading (true equals that the automatic trading will run)</param>
        /// <returns>Returns true if everything ok</returns>
        bool SetAutomaticTrading(bool Value);
        /// <summary>
        /// Retrive the current status of the currenct Exhange (open, closed or undefined if no info is available, which means that the system will interperate this as closed)
        /// </summary>
        ExchangeStatus SystemExchangeStatus { get; }
        void LoadImperaturMarket(ImperaturData SystemData);
        void LoadImperaturMarket(string SystemLocation);
        void StartTradeAutomationProcess();
        event ImperaturMarket.SystemNotificationHandler SystemNotificationEvent;
        event ImperaturMarket.QuoteUpdateHandler QuoteUpdateEvent;

    }


    public class ImperaturMarket : IImperaturMarket
    {
        #region Imperatur handler objects
        private IAccountHandlerInterface m_oAccountHandler;
        private ITradeHandlerInterface m_oTradeHandler;
        private ICurrency m_oDisplayCurrency;
        private IOrderQueue m_oOrderQueue;
        private ITradeAutomation m_oTradeAutomation;
        private bool AutomationThreadRunning;

        #endregion

        #region Imperatur system data
        private ImperaturData m_oImperaturData;
        private readonly string SystemDataFile = "imperatursettings.json";
        #endregion

        #region misc
        private string m_oLastErrorMessage;
        private System.Timers.Timer m_oQuoteTimer;

        #endregion

        #region events and delegates
        public delegate void QuoteUpdateHandler(object sender, EventArgs e);
        public delegate void SystemNotificationHandler(object sender, IMPSystemNotificationEventArg e);

        public event SystemNotificationHandler SystemNotificationEvent;
        public event QuoteUpdateHandler QuoteUpdateEvent;

        #endregion

        #region proctectedMethods
        protected virtual void OnQuoteUpdate(EventArgs e)
        {
            if (QuoteUpdateEvent != null)
                QuoteUpdateEvent(this, e);
        }

        protected virtual void OnSystemNotification(IMPSystemNotificationEventArg e)
        {
            if (SystemNotificationEvent != null)
                SystemNotificationEvent(this, e);
        }

        #endregion
        
        #region General
        public string GetLastErrorMessage()
        {
            return m_oLastErrorMessage ?? "";
        }
        #endregion

        #region constructor


        public ImperaturMarket()
        {
            AutomationThreadRunning = false;
        }

        public void LoadImperaturMarket(ImperaturData SystemData)
        {
            //create the system based on the data
            if (!Directory.Exists(SystemData.SystemDirectory))
            {
               if (!CreateImperaturDataFromSystemData(SystemData))
                {
                    int gg = 0;
                }
            }
            CreateImperaturMarket(SystemData);
        }

        public void LoadImperaturMarket(string SystemLocation)
        {
            CreateImperaturMarket(ReadImperaturDataFromSystemLocation(SystemLocation));
        }
        #endregion
        
        #region public methods
        public IOrderQueue OrderQueue
        {
            get
            {
                if (m_oOrderQueue == null)
                {
                    m_oOrderQueue = ImperaturGlobal.Kernel.Get<IOrderQueue>(
                         new Ninject.Parameters.ConstructorArgument("AccountHandler", GetAccountHandler()),
                         new Ninject.Parameters.ConstructorArgument("TradeHandler", GetTradeHandler())
                     );
                }
                return m_oOrderQueue;
            }
        }

        public ExchangeStatus SystemExchangeStatus
        {
            get
            {
                return ImperaturGlobal.ExchangeStatus;
            }
        }

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
            //start the tradeautomation process
            //TradingRobotMain();
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
            if (ImperaturGlobal.ExchangeStatus != ExchangeStatus.Open)
            {
                return;
            }
            m_oTradeHandler.ForceUpdate();
        }

        private ImperaturData ReadImperaturDataFromSystemLocation(string SystemLocation)
        {
            ImperaturData oD = new ImperaturData();
            try
            {
                oD = (ImperaturData)DeserializeJSON.DeserializeObjectFromFile(string.Format(@"{0}\{1}", SystemLocation, SystemDataFile));
            }
            catch (Exception ex)
            {
                int ff = 0;
            }
            return oD;
         }
        private bool CreateImperaturDataFromSystemData(ImperaturData Systemdata)
        {
            ImperaturGlobal.GetLogWithDirectory(Systemdata.SystemDirectory).Info("Creating the Imperatur Market System");
            if (
                CreateDirectory(Systemdata.SystemDirectory)
                &&
                CreateDirectory(string.Format(@"{0}\{1}", Systemdata.SystemDirectory, Systemdata.AcccountDirectory))
                &&
                CreateDirectory(string.Format(@"{0}\{1}\{2}", Systemdata.SystemDirectory, Systemdata.QuoteDirectory, Systemdata.DailyQuoteDirectory))
                &&
                CreateDirectory(string.Format(@"{0}\{1}\{2}", Systemdata.SystemDirectory, Systemdata.QuoteDirectory, Systemdata.HistoricalQuoteDirectory))
                &&
                CreateDirectory(string.Format(@"{0}\{1}", Systemdata.SystemDirectory, Systemdata.OrderDirectory))
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
                ImperaturGlobal.GetLog().Error(string.Format("Could not create directory {0}", NewDirectoryToCreate), ex);
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
                ImperaturGlobal.GetLog().Error(string.Format("Could not Initiate Ninject Kernel"), ex);
                throw new Exception(string.Format("Could not initiate Imperatur Market application: {0}", ex.Message));
            }
        }
        private void CreateImperaturMarket(ImperaturData SystemData)
        {
            m_oImperaturData = SystemData;

            CreateSystemNotification("Starting the initializing sequence");
            ImperaturGlobal.GetLogWithDirectory(m_oImperaturData.SystemDirectory).Info("Starting the initialize sequence");
            CreateSystemNotification("Creating cache objects");
            ImperaturGlobal.Initialize(m_oImperaturData, InitiateNinjectKernel(), null);
            

            List<account.AccountCacheType> BusinessAccounts = new List<account.AccountCacheType>();
            List<IAccountInterface> oLAB = new List<IAccountInterface>();

            CreateSystemNotification("Loading accounts");
            if (GetAccountHandler().Accounts().Where(a => a.GetAccountType().Equals(account.AccountType.Bank)).Count() == 0)
            {
                //create internalbankaccount for balance transactions
                //start by create the bankaccount
                oLAB.Add(
                    ImperaturGlobal.Kernel.Get<IAccountInterface>(
                        new Ninject.Parameters.ConstructorArgument("Customer", (object)null),
                        new Ninject.Parameters.ConstructorArgument("AccountType", AccountType.Bank),
                        new Ninject.Parameters.ConstructorArgument("AccountName", "INTERNALBANK")
                    )
                    );
            }
            if (GetAccountHandler().Accounts().Where(a => a.GetAccountType().Equals(account.AccountType.House)).Count() == 0)
            {
                //create internalbankaccount for balance transactions
                //start by create the bankaccount
                oLAB.Add(
                    ImperaturGlobal.Kernel.Get<IAccountInterface>(
                        new Ninject.Parameters.ConstructorArgument("Customer", (object)null),
                        new Ninject.Parameters.ConstructorArgument("AccountType", AccountType.House),
                        new Ninject.Parameters.ConstructorArgument("AccountName", "INTERNALHOUSE")
                    )
                    );
            }
            if (oLAB.Count() > 0)
                GetAccountHandler().CreateAccount(oLAB);

            //add all business accounts to cache
            BusinessAccounts = GetAccountHandler().Accounts().Where(a => !a.GetAccountType().Equals(account.AccountType.Customer)).
                Select(b =>
                new account.AccountCacheType
                {
                    AccountType = b.GetAccountType(),
                    Identifier = b.Identifier
                }).ToList();

            CreateSystemNotification("Initializing business accounts");
            ImperaturGlobal.InitializeBusinessAccount(BusinessAccounts);

            CreateSystemNotification("Loading instruments and qoutes");
            ImperaturGlobal.Quotes = GetTradeHandler().GetQuotes();

            CreateSystemNotification("Setting events");
            m_oQuoteTimer = new System.Timers.Timer();
            m_oQuoteTimer.Elapsed += M_oQuoteTimer_Elapsed;
            m_oQuoteTimer.Interval = 1000 * 60 * 1; //Convert.ToInt32(m_oImperaturData.QuoteRefreshTime); //every 15 minutes
            m_oQuoteTimer.Enabled = true;

            m_oDisplayCurrency = ImperaturGlobal.Kernel.Get<ICurrency>(new Ninject.Parameters.ConstructorArgument("CurrencyCode", m_oImperaturData.SystemCurrency));
            m_oTradeHandler.QuoteUpdateEvent += M_oTradeHandler_QuoteUpdateEvent;

            m_oOrderQueue = null;
            m_oTradeAutomation = ImperaturGlobal.Kernel.Get<ITradeAutomation>(new Ninject.Parameters.ConstructorArgument("AccountHandler", GetAccountHandler()));

            m_oTradeAutomation.SystemNotificationEvent += M_oTradeAutomation_SystemNotificationEvent;

            ImperaturGlobal.GetLog().Info("End of the initialize sequence");

        }

        private void CreateSystemNotification(string Message)
        {
            OnSystemNotification(new IMPSystemNotificationEventArg
            {
                Message = Message
            });
        }

        private void M_oTradeAutomation_SystemNotificationEvent(object sender, IMPSystemNotificationEventArg e)
        {
            OnSystemNotification(e);
        }

        private void EvaluateOrdersInQueue()
        {
            CreateSystemNotification("Processing orders in the order queue");
            OrderQueue.EvaluateOrdersInQueue();
        }

        private void TradingRobotMain()
        {
            if (!m_oImperaturData.IsAutomaticMaintained || ImperaturGlobal.ExchangeStatus != ExchangeStatus.Open)
            {
                return;
            }

            CreateSystemNotification("Trading automation process running - processing order queue first");
            //start with an evaluation of the orders
            OrderQueue.EvaluateOrdersInQueue();

            CreateSystemNotification("Trading automation process running - retrieving info to calculate trading recommendations");
            List<IOrder> NewOrders = m_oTradeAutomation.RunTradeAutomation();

            CreateSystemNotification("Adding orders to queue");
            OrderQueue.AddOrders(NewOrders.Where(x=>x!=null).ToList());

            //do all the others that might have ended up in the list.
            CreateSystemNotification("processing order queue after recommendations");
            OrderQueue.EvaluateOrdersInQueue();

            CreateSystemNotification("processing order queue maintence");
            OrderQueue.QueueMaintence(m_oAccountHandler);

            CreateSystemNotification("Trading automation process finished");

        }
        private void AutomationThreadFinished()
        {
            AutomationThreadRunning = false;
        }

        private void M_oTradeHandler_QuoteUpdateEvent(object sender, EventArgs e)
        {
            m_oTradeHandler.CacheQuotes();
            OnQuoteUpdate(e);
            ImperaturGlobal.Quotes = m_oTradeHandler.GetQuotes();
            if (!AutomationThreadRunning)
            {
                ThreadStart TradingRobotMain_starter = TradingRobotMain;
                TradingRobotMain_starter += () =>
                {
                    AutomationThreadFinished();
                };
                Thread m_oAutomationThread = new Thread(TradingRobotMain_starter) { IsBackground = true };
                if (!m_oAutomationThread.ThreadState.Equals(ThreadState.Background))
                {
                    AutomationThreadRunning = true;
                    m_oAutomationThread.Start();
                }
            }
        }

        public ITradeHandlerInterface GetTradeHandler()
        {
            if (m_oTradeHandler == null)
                m_oTradeHandler = ImperaturGlobal.Kernel.Get<ITradeHandlerInterface>();

            return m_oTradeHandler;
        }

        public void StartTradeAutomationProcess()
        {
            OnQuoteUpdate(new EventArgs());
        }

        #endregion


    }
}
