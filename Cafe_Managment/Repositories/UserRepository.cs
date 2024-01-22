using Cafe_Managment.Model;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Cafe_Managment.Repositories
{
    internal class UserRepository : RepositoryBase, IUserRepository
    {
        public void Add(UserData userData)
        {
            throw new NotImplementedException();
        }

        public int AuthenticateUser(NetworkCredential credential, out int OutId)
        {
            int validUser;
            int Id;

            using(var connection  = GetConnection())
            using (var command = new MySqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "SELECT employeepassword, employeerole, id, salt, status FROM employees WHERE employeeusername = @Username";
                command.Parameters.AddWithValue("username", credential.UserName);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows && reader.Read())
                    {
                        Id = int.Parse(reader["id"].ToString());
                        string storedPassword = reader["employeepassword"].ToString();
                        string userStatus = reader["status"].ToString();
                        if ("Работает" == userStatus)
                        {
                            if (BCrypt.Net.BCrypt.Verify(credential.Password, storedPassword))
                            {
                                validUser = 0;
                            }
                            else { validUser = 3;}
                        }
                        else { validUser = 1; }
                    }
                    else { validUser = 2; Id = -1; }
                }
                connection.Close();
            }

            OutId = Id;
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
            using (var connection = GetConnection())
            using (var command = new MySqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "SELECT * FROM employees WHERE id = @userId";
                command.Parameters.AddWithValue("userId", id);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows && reader.Read())
                    {
                        Id = int.Parse(reader["id"].ToString());
                        string storedPassword = reader["employeepassword"].ToString();
                        string userStatus = reader["status"].ToString();
                        if ("Работает" == userStatus)
                        {
                            if (BCrypt.Net.BCrypt.Verify(credential.Password, storedPassword))
                            {
                                validUser = 0;
                            }
                            else { validUser = 3; }
                        }
                        else { validUser = 1; }
                    }
                    else { validUser = 2; Id = -1; }
                }
                connection.Close();
            }

            OutId = Id;
            return validUser;
        }

        public void GetByUsername(string username)
        {
            throw new NotImplementedException();
        }
    }
}
