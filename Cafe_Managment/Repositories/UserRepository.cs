using Cafe_Managment.Model;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Resources;

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
                command.CommandText = "SELECT ID, RoleId AS 'Роль', Name AS 'Имя', Surname AS 'Фамилия', " +
                    "Patronomic AS 'Отчество', PhoneNumber AS 'Номер телефона', " +
                    "Email AS 'Почта', DateOfBirth AS 'День рождения', " +
                    "Address AS 'Адрес' FROM employees";


                MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                adapter.Fill(dataTable);
                
                connection.Close();
            }

            
            return dataTable;
        }

    

        public void GetById(int id)
        {
            using (var connection = GetConnection())
            using (var command = new MySqlCommand())
            {

                connection.Open();

                command.Connection = connection;
                command.CommandText = "SELECT RoleId, Name, Surname, Patronomic, " +
                    "Phonenumber, Email, DateOfBirth, Address, ProfilePicture FROM employees WHERE Id = @userId";
                command.Parameters.AddWithValue("userId", id);

                using (var reader = command.ExecuteReader())
                {

                    reader.Read();


                    UserData.Id = id;

                    UserData.RoleId = int.Parse(reader[0].ToString());
                    UserData.Name = reader[1].ToString();
                    UserData.Surname = reader[2].ToString();
                    UserData.Patronomic = reader[3].ToString();
                    UserData.PhoneNumber = reader[4].ToString();
                    UserData.Email = reader[5].ToString();
                    UserData.BirthDay = reader.GetDateTime(6).ToString().Substring(0, 10);
                    UserData.Address = reader[7].ToString();

                    if (reader[8] != DBNull.Value )
                    {
                        byte[] imageData = (byte[])reader[8];
                        UserData.ProfileImage = ConvertByteArrayToBitmapImage(imageData);

                    }else UserData.ProfileImage = null;



                }
                connection.Close();
            }
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
        private string ArrayToString(byte[] byteArray)
        {
            string temp = string.Empty;
            for (int i = 0; i < byteArray.Length; i++)
            {
                temp += byteArray[i].ToString() + ' ';
            }
            return temp;
        }
    }
}

