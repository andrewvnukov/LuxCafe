using Cafe_Managment.Model;
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
using ExCSS;
using System.Xml.Linq;

namespace Cafe_Managment.Repositories
{
    internal class UserRepository : RepositoryBase, IUserRepository
    {
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
            using (var connection = GetConnection())

            using (var command = new MySqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "UPDATE employees SET status = 0 WHERE Id=@id";
                command.Parameters.AddWithValue("id", Id);

                command.ExecuteNonQuery();

                connection.Close();
            }
        }

        public DataTable GetByAll()
        {
            DataTable dataTable = new DataTable();

            using (var connection = GetConnection())
            using (var command = new MySqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = @"SELECT e.Id AS '№',
                                        e.BranchId AS 'Филиал', r.Title AS 'Роль', 
                                        e.Name AS 'Имя', e.Surname AS 'Фамилия', 
                                        e.Patronomic AS 'Отчество', e.PhoneNumber AS 'Номер телефона', 
                                        e.Email AS 'Почта', DATE_FORMAT(e.BirthDay, '%d-%m-%Y') AS 'Дата рождения', 
                                        e.Address AS 'Адрес' 
                                FROM Employees e 
                                INNER JOIN Roles r ON e.RoleId = r.Id";

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
                command.CommandText = "SELECT RoleId, BranchId, CreatedAt, Name, Surname, Patronomic, " +
                    "Phonenumber, Email, BirthDay, Address, ProfileImage FROM Employees WHERE Id = @userId";
                command.Parameters.AddWithValue("userId", UserData.Id);

                using (var reader = command.ExecuteReader())
                {

                    reader.Read();

                    UserData.RoleId = int.Parse(reader[0].ToString());
                    UserData.BranchId = int.Parse(reader[1].ToString());
                    UserData.CreatedAt = DateTime.Parse(reader[2].ToString());
                    UserData.Name = reader[3].ToString();
                    UserData.Surname = reader[4].ToString();
                    UserData.Patronomic = reader[5].ToString();
                    UserData.PhoneNumber = reader[6].ToString() != null ? reader[6].ToString() : "Не введен";
                    UserData.Email = reader[7].ToString() != null ? reader[7].ToString() : "Не введен"; ;
                    UserData.BirthDay = reader.GetDateTime(8).ToString("yyyy-MM-dd");
                    UserData.Address = reader[9].ToString() != null ? reader[9].ToString() : "Не введен"; ;

                    if (reader[10] != DBNull.Value)
                    {
                        byte[] imageData = (byte[])reader[8];
                        UserData.ProfileImage = ConvertByteArrayToBitmapImage(imageData);
                    }
                    else UserData.ProfileImage = new BitmapImage(new Uri("/Images/EmptyImage.jpg", UriKind.Relative)); ;

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

                using (var reader = command.ExecuteReader())
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
            using (var connection = GetConnection())
            using (var command = new MySqlCommand())
            {

                connection.Open();

                command.Connection = connection;
                command.CommandText = "DELETE IGNORE FROM autorizeddevices WHERE EmployeeId = @EmpId";
                command.Parameters.AddWithValue("EmpId", UserData.Id);

                command.ExecuteNonQuery();
            }
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
            using (var connection = GetConnection())

            using (var command = new MySqlCommand())
            {
                connection.Open();
                command.Connection = connection;

                command.CommandText = "INSERT INTO employees (RoleId, BranchId, Login, Password, Salt, Name, Surname, " +
                    "Patronomic, PhoneNumber, BirthDay, CreatedAt, Status) "
                    +"VALUES(@role, @branch, @username, @password, @Salt, @name, @surname, " +
                    "@patronomic, @number, @birthday, @NowDate, @status)";
                command.Parameters.AddWithValue("role", empData.Role+1);
                command.Parameters.AddWithValue("branch", empData.Branch+1);
                command.Parameters.AddWithValue("username", empData.Login);
                string salt = BCrypt.Net.BCrypt.GenerateSalt();
                command.Parameters.AddWithValue("password", BCrypt.Net.BCrypt.HashPassword(empData.Password, salt));
                command.Parameters.AddWithValue("Salt", salt);
                command.Parameters.AddWithValue("name", empData.Name);
                command.Parameters.AddWithValue("surname", empData.Surname);
                command.Parameters.AddWithValue("patronomic", empData.Patronomic);
                command.Parameters.AddWithValue("birthday", empData.BirthDay);
                command.Parameters.AddWithValue("NowDate", DateTime.Now);
                command.Parameters.AddWithValue("status", 1);
                command.Parameters.AddWithValue("number", "1234");

                
                try
                {
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при передачи в БД: {ex.Message}");
                    Console.WriteLine(ex.ToString());
                }

                connection.Close();
            }
        }

        public bool IfUserExists(string Username)
        {
            using (var connection = GetConnection())
            using (var command = new MySqlCommand())
            {

                connection.Open();

                command.Connection = connection;
                command.CommandText = "SELECT * FROM employees WHERE Login=@Username";
                command.Parameters.AddWithValue("Username", Username);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return true;
                    }
                    else return false;
                }
            }
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

                using (var reader = command.ExecuteReader())
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

        public List<string> GetBranches()
        {
            List<string> branches = new List<string>();

            using (var connection = GetConnection())
            using (var command = new MySqlCommand())
            {
                connection.Open();

                command.Connection = connection;
                command.CommandText = "SELECT address FROM branches";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        branches.Add(reader["address"].ToString());
                    }
                }
                connection.Close();
                return branches;
            }
        }
    }
}

