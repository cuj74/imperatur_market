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
                /*
                new Ninject.Parameters.ConstructorArgument("BFSDataHandler", BFSDataHandler),
                new Ninject.Parameters.ConstructorArgument("SQLConnection", SQLConnection),
                new Ninject.Parameters.ConstructorArgument("DisplayCurrency", DisplayCurrencyCode)*/
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
                /*
                new Ninject.Parameters.ConstructorArgument("BFSDataHandler", BFSDataHandler),
                new Ninject.Parameters.ConstructorArgument("SQLConnection", SQLConnection),
                new Ninject.Parameters.ConstructorArgument("DisplayCurrency", DisplayCurrencyCode)*/
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


    }


    //public delegate void RefreshEventHandler(object sender, EventArgs e);

    public class ImperaturMarket : IImperaturMarket
    {
        private IAccountHandlerInterface m_oAccountHandler;
        private ICurrency m_oDisplayCurrency;
        private string m_oLastErrorMessage;
        //private StandardKernel m_oKernel;
        private ImperaturData m_oImperaturData;
        private readonly string SystemDataFile = "imperatursettings.json";

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
        private ImperaturData ReadImperaturDataFromSystemLocation(string SystemLocation)
        {
            ImperaturData oD = new ImperaturData();
            try
            {
                JObject oJ = (JObject)DeserializeJSON.DeserializeObjectFromFile(string.Format(@"{0}\{1}", SystemLocation, SystemDataFile));
                oD = oJ.ToObject<ImperaturData>();
               // oD = (ImperaturData)DeserializeJSON.DeserializeObjectFromFile(string.Format(@"{0}\{1}", SystemLocation, SystemDataFile));
            }
            catch(Exception ex)
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
                CreateDirectory(string.Format(@"{0}\{1}", Systemdata.SystemDirectory, Systemdata.QuoteDirectory))
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
            BusinessAccounts = GetAccountHandler().Accounts().Where(a => !a.GetAccountType().Equals(account.AccountType.Customer)).
                Select(b =>
                new account.AccountCacheType
                {
                    AccountType = b.GetAccountType(),
                    Identifier = b.Identifier
                }).ToList();
            ImperaturGlobal.InitializeBusinessAccount(BusinessAccounts);

            m_oDisplayCurrency = ImperaturGlobal.Kernel.Get<ICurrency>(new Ninject.Parameters.ConstructorArgument("CurrencyCode", m_oImperaturData.SystemCurrency));
            

            /*
            m_oAccountHandler = m_oKernel.Get<IAccountHandlerInterface>(
    new Ninject.Parameters.ConstructorArgument("BFS", m_oBFS),
    new Ninject.Parameters.ConstructorArgument("User", m_oUser),
    new Ninject.Parameters.ConstructorArgument("DisplayCurrency", m_oDisplayCurrency)
    );*/
        }
        #endregion
        /*
        private ICurrency m_oDisplayCurrency;
        private IUserHandler m_oUser;
        private IFundCompanyHandler m_oFundCompany;
        private Dictionary<Type, Action> @SwitchObjectEvent;
        private StandardKernel m_oKernel;
        private ISQL m_oSQLHandler;

        private string m_oLastErrorMessage;

        private static Timer RefreshCurrencyExhangeCache;

        #region General
        public string GetLastErrorMessage()
        {
            return m_oLastErrorMessage ?? "";
        }
        #endregion

        #region constructor
        public FundAdministration(IBFSDataHandler BFSDataHandler, string SQLConnection, string DisplayCurrency)
        {
            try
            {
                m_oKernel = new StandardKernel();
                m_oKernel.Load(Assembly.GetExecutingAssembly());
                Logger.Initialize();
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Could not initiate Fund Administration application: {0}", ex.Message));
            }
            //Use

            m_oBFS = BFSDataHandler;
            if (!m_oBFS.InitClient())
            {
                throw new Exception(string.Format("Could not initiate BFS client: {0}", m_oBFS.GetLastException()));
            }

            Logger.Instance.Info(string.Format("Connected to BFS system {0}", m_oBFS.GetEndpoint()));
            shared.FAMGlobal.Initialize(m_oBFS);
            Logger.Instance.Info("Created cache");

            m_oDisplayCurrency = m_oKernel.Get<ICurrency>(new Ninject.Parameters.ConstructorArgument("CurrencyCode", DisplayCurrency));

            try
            {
                m_oSQLHandler =
                  m_oKernel.Get<ISQL>(
                new Ninject.Parameters.ConstructorArgument("SQLConnection", SQLConnection)
                );


                new SQLHandler(new System.Data.SqlClient.SqlConnection(SQLConnection));
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(string.Format("Could not connect to SQL database {0}, {1}", m_oSQLHandler != null ? m_oSQLHandler.GetDatabaseName() : "<not stated>", ex.Message));
                throw new Exception(ex.Message);
            }
            if (m_oSQLHandler != null)
                Logger.Instance.Info(string.Format("Connected to SQL database {0}", m_oSQLHandler.GetDatabaseName()));


            Logger.Instance.Info("Fund Administration started");
        }
        #endregion

        /// <summary>
        /// Handles the event of resfreshing data on the handler level. Different handlers will use the same method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Handler_OnNeedsRefresh(object sender, EventArgs e)
        {
            Type[] oInterfaceTypes = sender.GetType().GetInterfaces();
            if (oInterfaceTypes != null && oInterfaceTypes.Count() == 1)
            {
                @SwitchObjectEvent[oInterfaceTypes[0]]();
            }
        }


        public void LoginUser(string Username, string Password)
        {

            //populate User
            m_oUser = m_oKernel.Get<IUserHandler>(
                new Ninject.Parameters.ConstructorArgument("BFS", m_oBFS),
                new Ninject.Parameters.ConstructorArgument("UserName", Username),
                new Ninject.Parameters.ConstructorArgument("Password", Password)
                );

            //Return if no user is logged in
            if (m_oUser.SignInManager() == null || !m_oUser.SignInManager().Result.Equals(SignInManager.SignInResult.Success))
            {
                return;
            }

            //Populate FundCompany
            m_oFundCompany = m_oKernel.Get<IFundCompanyHandler>(
                new Ninject.Parameters.ConstructorArgument("BFS", m_oBFS),
                new Ninject.Parameters.ConstructorArgument("User", m_oUser),
                new Ninject.Parameters.ConstructorArgument("DisplayCurrency", m_oDisplayCurrency)
                );

            //the dictionary for handling the refresh event
            @SwitchObjectEvent = new Dictionary<Type, Action> {
                { typeof(IFundEntityHandler), () => GetFundCompanyHandler().GetFundEntities().ForEach(fe=>fe.Refresh())},
                { typeof(IFundCompanyHandler), () => GetFundCompanyHandler().Refresh()}
            };

            m_oFundCompany.GetFundEntities().ForEach(fe => fe.OnNeedsRefresh += Handler_OnNeedsRefresh);
            m_oFundCompany.OnNeedsRefresh += Handler_OnNeedsRefresh;

        }

        public void LogoutUser()
        {
            m_oUser.LogOut();
            m_oUser = null;
            m_oFundCompany = null;
            @SwitchObjectEvent = null;

        }

        #region read

        public IUserHandler GetUserHandler()
        {
            return m_oUser;
        }
        public IFundCompanyHandler GetFundCompanyHandler()
        {
            if (m_oUser.SignInManager().Result == SignInManager.SignInResult.Success)
                return m_oFundCompany;

            return null;
        }

        public ICache GetCacheType(string TypeOfCache)
        {

            if (!GlobalCachingProvider.Instance.FindItem(TypeOfCache))
                return null;

            return (ICache)GlobalCachingProvider.Instance.GetItem(TypeOfCache);

        }
        #endregion

        #region create

        public FundCompany CreateFundCompany(FundCompany NewFundCompany)
        {
            FundCompany oFundCompany = m_oBFS.CreateFundCompany(NewFundCompany);
            if (oFundCompany.Equals(null))
            {
                m_oLastErrorMessage = m_oBFS.GetLastException();
                // TODO Add the recently created fund company to the current FundCompanyHandler
                Handler_OnNeedsRefresh(this.GetFundCompanyHandler(), new EventArgs());
                return null;
            }
            return oFundCompany;
        }


        public Instrument CreateInstrument(Instrument NewInstrument)
        {
            Instrument oInstrument = m_oBFS.CreateInstrument(NewInstrument);
            if (oInstrument == null)
            {
                m_oLastErrorMessage = m_oBFS.GetLastException();
                return null;
            }
            return oInstrument;
        }
        #endregion

        #region PrivateMethods



        #endregion
        */

    }
}
