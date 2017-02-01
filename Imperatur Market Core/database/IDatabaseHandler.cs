using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;

namespace Imperatur_Market_Core.database
{
    public interface IDatabaseHandler
    {
        LiteCollection<T> GetCollectionFromDataBase<T>();
    }
}
