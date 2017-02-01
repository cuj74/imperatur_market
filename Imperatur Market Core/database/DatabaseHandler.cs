using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Imperatur_Market_Core.shared;
using LiteDB;


namespace Imperatur_Market_Core.database
{
    public class DatabaseHandler : IDatabaseHandler
    {
        private LiteDatabase liteDatabase;
        /*
        public static T ConfigSetting<T>(string settingName)
        {
            object value = ConfigurationManager.AppSettings[settingName];
            return (T)Convert.ChangeType(value, typeof(T));
        }*/

        public LiteCollection<T> GetCollectionFromDataBase<T>()
        {
            if (liteDatabase == null)
            {
                liteDatabase = new LiteDatabase(ImperaturGlobal.DataBaseFileName);
            }
            return liteDatabase.GetCollection<T>(typeof(T).Name);
            /*
            using (var db = new LiteDatabase(ImperaturGlobal.DataBaseFileName))
            {
                var col = db.GetCollection<Customer>("customer");

                using (var trans = db.BeginTrans())
                {
                    col.Insert(new Customer { Name = "John Doe" });
                    col.Insert(new Customer { Name = "Joanna Doe" });
                    col.Insert(new Customer { Name = "Foo Bar" });

                    trans.Commit();
                } // all or none!
            }*/

        }

    }
}
