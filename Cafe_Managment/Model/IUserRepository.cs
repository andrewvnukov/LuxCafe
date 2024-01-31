
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
        int AuthenticateUser(NetworkCredential credential);
        void Add();
        void Edit();
        void Delete(int Id);
        void FireEmployee(int Id);
        void RememberUser();
        void GetById();
        DataTable GetByAll();
        void GetByMac(string Mac);

    }
}
