
using Cafe_Managment.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Cafe_Managment.Repositories
{
    public interface IUserRepository
    {
        int AuthenticateUser(NetworkCredential credential);
        void Add(EmpData empData);
        void Edit();
        void Delete(int Id);
        void FireEmployee(int Id);
        void RememberCurrentUser();
        void ForgetCurrentUser();
        void GetById();

        bool GetByMac();
        string GetRoleById(int RoleId);
        List<string> GetRoles();
        DataTable GetByAll();
        

    }
}
