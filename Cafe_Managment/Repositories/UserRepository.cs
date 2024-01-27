using Cafe_Managment.Model;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;

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


            using (var connection = GetConnection())

            using (var command = new MySqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "SELECT Id, Password, Salt, Status FROM employees WHERE Username = @Username";
                command.Parameters.AddWithValue("username", credential.UserName);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows && reader.Read())
                    {
                        Id = int.Parse(reader[0].ToString());
                        string storedPassword = reader[1].ToString();
                        string salt = reader[2].ToString();
                        short userStatus = reader.GetInt16("Status");

                        if (userStatus == 1)
                        {
                            if (BCrypt.Net.BCrypt.HashPassword(credential.Password, salt) == storedPassword)
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

        public UserAccountData GetById(int id)
        {
            UserAccountData CurrentData;

            using (var connection = GetConnection())
            using (var command = new MySqlCommand())
            {

                connection.Open();
                
                command.Connection = connection;
                command.CommandText = "SELECT RoleId, Name, Surname, Patronomic, Phonenumber, Email, DateOfBirth, Address, ProfilePicture FROM employees WHERE Id = @userId";
                command.Parameters.AddWithValue("userId", id);

                using (var reader = command.ExecuteReader())
                {

                    reader.Read();

                    CurrentData = new UserAccountData
                    {
                        Id = id,

                        RoleId = int.Parse(reader[0].ToString()),
                        Name = reader[1].ToString(),
                        Surname = reader[2].ToString(),
                        Patronomic = reader[3].ToString(),
                        PhoneNumber = reader[4].ToString(),
                        Email = reader[5].ToString(),
                        BirthDay = reader.GetDateTime("DateOfBirth").ToString(),
                        Address = reader[7].ToString(),
                        ProfileImage = (byte[])reader[8]

                    };

                }
                connection.Close();

            }
            return CurrentData;
        }

        public void GetByUsername(string username)
        {
            throw new NotImplementedException();
        }


    }
}
