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
            if (GetAccountHandler().Accounts().Where(a => !a.GetAccountType().Equals(account.AccountType.Bank)).Count() == 0)
            {
                //create internalbankaccount for balancetransactions
                //start by create the bankaccount
                List<account.IAccountInterface> oLAB = new List<Imperatur_v2.account.IAccountInterface>();
                oLAB.Add(new account.Account(null, account.AccountType.Bank, "INTERNALBANK"));
                GetAccountHandler().CreateAccount(oLAB);
            }

            BusinessAccounts = GetAccountHandler().Accounts().Where(a => !a.GetAccountType().Equals(account.AccountType.Customer)).
                Select(b =>
                new account.AccountCacheType
                {
                    AccountType = b.GetAccountType(),
                    Identifier = b.Identifier
                }).ToList();
            ImperaturGlobal.InitializeBusinessAccount(BusinessAccounts);

            m_oDisplayCurrency = ImperaturGlobal.Kernel.Get<ICurrency>(new Ninject.Parameters.ConstructorArgument("CurrencyCode", m_oImperaturData.SystemCurrency));
            
        }
        #endregion
        

    }
}
