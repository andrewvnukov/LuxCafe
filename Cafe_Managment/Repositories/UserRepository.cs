using Cafe_Managment.Model;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Cafe_Managment.Repositories
{
    internal class UserRepository : RepositoryBase, IUserRepository
    {
        public void Add(UserData userData)
        {
            throw new NotImplementedException();
        }

        public bool AuthenticateUser(NetworkCredential credential)
        {
            bool validUser;

            using(var connection  = GetConnection())
            using (var command = new MySqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "select * from employees where [employeeusername]=@employeeusername and [employeepassword]=@employeepassword and [status] = @status";
                command.Parameters.Add("@status", MySqlDbType.VarChar).Value="Работает";
                command.Parameters.Add("@employeeusername", MySqlDbType.VarChar).Value=credential.UserName;
                command.Parameters.Add("@employeepassword", MySqlDbType.VarChar).Value=credential.Password;
                validUser = command.ExecuteScalar() == null? false : true;
            }

                return validUser;
        }

        public void Delete(int Id)
        {
            throw new NotImplementedException();
        }

        public void Edit(UserData userData)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<UserData> GetByAll()
        {
            throw new NotImplementedException();
        }

        public void GetById(int id)
        {
            throw new NotImplementedException();
        }

        public void GetByUsername(string username)
        {
            throw new NotImplementedException();
        }
    }
}
