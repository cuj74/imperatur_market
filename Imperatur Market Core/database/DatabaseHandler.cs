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

        public LiteCollection<T> GetCollectionFromDataBase<T>()
        {
            if (liteDatabase == null)
            {
                liteDatabase = new LiteDatabase(ImperaturGlobal.GetDataBaseFilePath);
            }
            return liteDatabase.GetCollection<T>(typeof(T).Name);
        }

    }
}
