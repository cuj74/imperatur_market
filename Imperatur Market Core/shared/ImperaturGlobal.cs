﻿using log4net;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Imperatur_Market_Core.database;

namespace Imperatur_Market_Core.shared
{


    public sealed class ImperaturGlobal
    {
        private static readonly Lazy<ImperaturGlobal> lazy =
             new Lazy<ImperaturGlobal>(() => new ImperaturGlobal());
        //private static ImperaturGlobal _instance;
        private static readonly string _LogName = "log4net";
        private static StandardKernel _kernel;
        private static IDatabaseHandler databaseHandler;

        public static List<Tuple<int, int>> _BankDays;
        public static List<Tuple<int, int>> _HalfDay;

        private ImperaturGlobal()
        {
        }
        public static ImperaturGlobal Instance { get { return lazy.Value; } }

        #region public methods
        public static StandardKernel Kernel
        {
            get
            {
                if (_kernel == null)
                {
                    _kernel = new StandardKernel();
                    _kernel.Load(Assembly.GetExecutingAssembly());
                }
                return _kernel;
            }
        }

        public static IDatabaseHandler DatabaseHandler
        {
            get
            {
                return databaseHandler;
            }
        }

        public static bool InitHandlers(IDatabaseHandler DataBaseHandler)
        {
            if (databaseHandler == null)
            {
                databaseHandler = DataBaseHandler;
            }
            return true;
        }

        public static readonly string DataBaseFileName = "imp.db";


        public static ILog GetLog()
        {
            return GetLog(null, ""); //todo, get the systemdirectory!
        }

        public static ILog GetLogWithDirectory(string LogDirectory)
        {
            return GetLog(null, LogDirectory);
        }

        private static ILog GetLog(string logName, string LogDirectory)
        {
            if (logName == null)
            {
                logName = _LogName;
            }
            log4net.GlobalContext.Properties["LogFileName"] = string.Format("{0}\\{1}{2}", LogDirectory, "log\\imp", DateTime.Now.ToString("yyyy-MM-dd")); //log file path
            ILog log = LogManager.GetLogger(logName);
            return log;
        }
        #endregion

    }
}
