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
        void UpdateCurrentUserPicture(byte[] picture);
        int AuthenticateUser(NetworkCredential credential);
        void Add(EmpData empData);
        bool IfUserExists(string Username);
        void EditCurrentUser(string NameOfProp, string Value);
        void Delete(int Id);
        void FireEmployee(int Id);
        void UpdateEmployee(EmpData data);
        void RememberCurrentUser();
        void ForgetCurrentUser();
        string GetRoleById(int RoleId);
        string GetBranchById(int BranchId);
        List<string> GetRoles();
        void GetById();
        bool GetByMac();   
        List<string> GetBranches();
        DataTable GetByAll();
        DataTable GetDismissedEmployees();
    }
}
