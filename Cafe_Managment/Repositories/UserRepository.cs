﻿using Cafe_Managment.Model;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
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

        public void FireEmployee(int Id)
        {
            throw new NotImplementedException();
        }

        public DataTable GetByAll()
        {
            DataTable dataTable = new DataTable();

            using (var connection = GetConnection())

            using (var command = new MySqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "SELECT ID AS IDD FROM employees";


                MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                adapter.Fill(dataTable);
                
                connection.Close();
            }

            
            return dataTable;
        }

    

        public UserData GetById(int id)
        {
            UserData CurrentData;

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

                    CurrentData = new UserData
                    {
                        Id = id,

                        RoleId = int.Parse(reader[0].ToString()),
                        Name = reader[1].ToString(),
                        Surname = reader[2].ToString(),
                        Patronomic = reader[3].ToString(),
                        PhoneNumber = reader[4].ToString(),
                        Email = reader[5].ToString(),
                        BirthDay = reader.GetDateTime(6).ToString().Substring(0, 10),
                        Address = reader[7].ToString(),
                        ProfileImage = (byte[])reader[8]

                    };
                }
                connection.Close();

            }
            return CurrentData;
        }

        public void RememberUser(int Id)
        {
            using (var connection = GetConnection())
            using (var command = new MySqlCommand())
            {

                connection.Open();

                var MacAddress = (from nic in NetworkInterface.GetAllNetworkInterfaces()
                              where nic.OperationalStatus == OperationalStatus.Up
                              select nic.GetPhysicalAddress().ToString()).FirstOrDefault();
                
                
                command.Connection = connection;
                command.CommandText = "INSERT IGNORE INTO autorizeddevices VALUES (@Mac, @UserId)";
                command.Parameters.AddWithValue("Mac", MacAddress);
                command.Parameters.AddWithValue("UserId", Id);
                command.ExecuteNonQuery();
                connection.Close();
            }
        }
    }
}
