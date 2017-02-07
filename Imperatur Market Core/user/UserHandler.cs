using Imperatur_Market_Core.shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imperatur_Market_Core.user
{
    public class UserHandler : IUserHandler
    {
        public int AddUser(User UserToAdd)
        {
            var ret = GetUserCollection().Insert(UserToAdd);
            return (int)ret;
        }

        public User GetUser(int Id)
        {
            return GetUserCollection().FindById(Id);
        }
        private LiteDB.LiteCollection<User> GetUserCollection()
        {
            return ImperaturGlobal.DatabaseHandler.GetCollectionFromDataBase<User>();
        }
    }
}
