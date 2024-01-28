using Cafe_Managment.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Cafe_Managment.Repositories
{
    public interface IUserRepository 
    {
        int AuthenticateUser(NetworkCredential credential, out int OutId);
        void Add(UserData userData);
        void Edit(UserData userData);
        void Delete(int Id);
        void FireEmployee(int Id);
        void RememberUser(int Id);
        void GetById(int Id);
        DataTable GetByAll();

    }
}
