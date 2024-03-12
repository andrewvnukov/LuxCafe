﻿using Cafe_Managment.Model;
using Cafe_Managment;
using Microsoft.SqlServer.Server;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Resources;

namespace Cafe_Managment.Repositories
{
    internal class UserRepository : RepositoryBase, IUserRepository
    {
        public void Add()
        {
            //using (var connection = GetConnection())

            //using (var command = new MySqlCommand())
            //{
            //    connection.Open();
            //    command.Connection = connection;
            //    command.CommandText = "INSERT INTO employees VALUES Username = @username, Password = @password, Name = @name," +
            //        "Surname = @surname, Patronomic = @patronomic, DateOfBirth = @birthday, Passport = @passport";
            //    command.Parameters.AddWithValue("username", credential.UserName);

            //    using (var reader = command.ExecuteReader())
            //    {
            //        if (reader.HasRows && reader.Read())
            //        {

            //            string storedPassword = reader[1].ToString();
            //            string salt = reader[2].ToString();
            //            short userStatus = reader.GetInt16("Status");

            //            if (userStatus == 1)
            //            {
            //                if (BCrypt.Net.BCrypt.HashPassword(credential.Password, salt) == storedPassword)
            //                {
            //                    UserData.Id = int.Parse(reader[0].ToString());
            //                    validUser = 0;
            //                }
            //                else { validUser = 3; }
            //            }
            //            else { validUser = 1; }
            //        }
            //        else { validUser = 2; }
            //    }
            //    connection.Close();
            //}
            //return validUser;
        }

        public int AuthenticateUser(NetworkCredential credential)
        {
            int validUser;

            using (var connection = GetConnection())

            using (var command = new MySqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "SELECT Id, Password, Salt, Status FROM Employees WHERE Login = @Username";
                command.Parameters.AddWithValue("username", credential.UserName);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows && reader.Read())
                    {
                       
                        string storedPassword = reader[1].ToString();
                        string salt = reader[2].ToString();
                        short userStatus = reader.GetInt16("Status");

                        if (userStatus == 1)
                        {
                            if (BCrypt.Net.BCrypt.HashPassword(credential.Password, salt) == storedPassword)
                            {
                                
                                UserData.Id = int.Parse(reader[0].ToString());
                                validUser = 0;
                            }
                            else { validUser = 3; }
                        }
                        else { validUser = 1; }
                    }
                    else { validUser = 2; }
                }
                connection.Close();
            }
            return validUser;
        }

        public void Delete(int Id)
        {
            throw new NotImplementedException();
        }

        public void Edit()
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
                command.CommandText = "SELECT ID, RoleId AS 'Роль', Name AS 'Имя', Surname AS 'Фамилия', " +
                    "Patronomic AS 'Отчество', PhoneNumber AS 'Номер телефона', " +
                    "Email AS 'Почта', BirthDay AS 'День рождения', " +
                    "Address AS 'Адрес' FROM Employees";


                MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                adapter.Fill(dataTable);
                
                connection.Close();
            }

            
            return dataTable;
        }

    

        public void GetById()
        {
            using (var connection = GetConnection())
            using (var command = new MySqlCommand())
            {

                connection.Open();

                command.Connection = connection;
                command.CommandText = "SELECT RoleId, Name, Surname, Patronomic, " +
                    "Phonenumber, Email, BirthDay, Address, ProfileImage FROM Employees WHERE Id = @userId";
                command.Parameters.AddWithValue("userId", UserData.Id);

                using (var reader = command.ExecuteReader())
                {

                    reader.Read();

                    UserData.RoleId = int.Parse(reader[0].ToString());
                    UserData.Name = reader[1].ToString();
                    UserData.Surname = reader[2].ToString();
                    UserData.Patronomic = reader[3].ToString();
                    UserData.PhoneNumber = reader[4].ToString() != null ? reader[4].ToString() : "Не введен";
                    UserData.Email = reader[5].ToString() != null ? reader[5].ToString() : "Не введен"; ;
                    UserData.BirthDay = reader.GetDateTime(6).ToString().Substring(0, 10);
                    UserData.Address = reader[7].ToString() != null ? reader[7].ToString() : "Не введен"; ;

                    if (reader[8] != DBNull.Value )
                    {
                        byte[] imageData = (byte[])reader[8];
                        UserData.ProfileImage = ConvertByteArrayToBitmapImage(imageData);
                    }else UserData.ProfileImage = new BitmapImage(new Uri("/Images/EmptyImage.jpg", UriKind.Relative)); ;

                }
                connection.Close();
            }
        }

        public void RememberCurrentUser()
        {
            using (var connection = GetConnection())
            using (var command = new MySqlCommand())
            {

                connection.Open();

                var MacAddress = (from nic in NetworkInterface.GetAllNetworkInterfaces()
                              where nic.OperationalStatus == OperationalStatus.Up
                              select nic.GetPhysicalAddress().ToString()).FirstOrDefault();
                
                
                command.Connection = connection; 
                command.CommandText = "INSERT IGNORE INTO AutorizedDevices VALUES (@Mac, @UserId)";
                command.Parameters.AddWithValue("Mac", MacAddress);
                command.Parameters.AddWithValue("UserId", UserData.Id);
                command.ExecuteNonQuery();
                connection.Close();
            }
        }

        private BitmapImage ConvertByteArrayToBitmapImage(byte[] byteArray)
        {
            try
            {
                using (MemoryStream stream = new MemoryStream(byteArray))
                {
                    BitmapImage image = new BitmapImage();
                    image.BeginInit();
                    image.StreamSource = stream;
                    image.CacheOption = BitmapCacheOption.OnLoad;

                    image.EndInit();
                    return image;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error converting byte array to BitmapImage: " + ex.Message);
                return null;
            }
        }

        public bool GetByMac()
        {
            var mac = (from nic in NetworkInterface.GetAllNetworkInterfaces()
                      where nic.OperationalStatus == OperationalStatus.Up
                      select nic.GetPhysicalAddress().ToString()).FirstOrDefault();
            using (var connection = GetConnection())
            using (var command = new MySqlCommand())
            {

                connection.Open();

                command.Connection = connection;
                command.CommandText = "SELECT EmployeeId FROM AutorizedDevices WHERE DeviceMac=@Mac";
                command.Parameters.AddWithValue("Mac", mac);
                
                using(var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        UserData.Id = int.Parse(reader[0].ToString());
                        connection.Close();
                        return true;
                    }
                    else
                    {
                        connection.Close();
                        return false;
                    }
                }

                
            }
        }

        public void ForgetCurrentUser()
        {
            throw new NotImplementedException();
        }

        public string GetRoleById(int RoleId)
        {
            using (var connection = GetConnection())
            using (var command = new MySqlCommand())
            {

                connection.Open();

                command.Connection = connection;
                command.CommandText = "SELECT Title FROM Roles WHERE Id=@RoleId";
                command.Parameters.AddWithValue("RoleId", RoleId);

                using (var reader = command.ExecuteReader())
                {
                    reader.Read();

                    return reader[0].ToString();
                }


            }
        }

        public void Add(EmpData empData)
        {
            throw new NotImplementedException();
        }

        public List<string> GetRoles()
        {
            List<string> roles = new List<string>();
            
            using (var connection = GetConnection())
            using (var command = new MySqlCommand())
            {
                connection.Open();

                command.Connection = connection;
                command.CommandText = "SELECT Title FROM roles";
                
                using(var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        roles.Add(reader["Title"].ToString());
                    }
                }
                connection.Close();
                return roles;

            }
        }
    }
}

