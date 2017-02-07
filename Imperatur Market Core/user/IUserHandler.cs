using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imperatur_Market_Core.user
{
    public interface IUserHandler
    {
        int AddUser(User UserToAdd);
        User GetUser(int Id);
    }
}
