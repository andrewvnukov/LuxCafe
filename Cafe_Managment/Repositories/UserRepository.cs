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
using Google.Protobuf.WellKnownTypes;

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
                command.CommandText = "SELECT Id, Password, Salt FROM Employees WHERE Login = @Username";
                command.Parameters.AddWithValue("username", credential.UserName);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows && reader.Read())
                    {
                        string storedPassword = reader[1].ToString();
                        string salt = reader[2].ToString();

                            if (BCrypt.Net.BCrypt.HashPassword(credential.Password, salt) == storedPassword)
                            {

                                UserData.Id = int.Parse(reader[0].ToString());
                                validUser = 0;
                            }
                            else { validUser = 3; }
                    }
                    else { validUser = 2; }
                }
                connection.Close();
            }
            return validUser;
        }

        public DataTable GetDismissedEmployees()
        {
            DataTable dataTable = new DataTable();

            using (var connection = GetConnection())
            using (var command = new MySqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = @"SELECT ROW_NUMBER() OVER() AS '№',
                                        
                                        c.Address AS 'Филиал', r.Title AS 'Роль', 
                                        e.Name AS 'Имя', e.Surname AS 'Фамилия', 
                                        e.Patronomic AS 'Отчество', e.PhoneNumber AS 'Номер телефона', 
                                        e.Email AS 'Почта', DATE_FORMAT(e.BirthDay, '%d-%m-%Y') AS 'Дата рождения', 
                                        e.Address AS 'Адрес' 
                                FROM dismissed_employees e 
                                INNER JOIN Roles r ON e.RoleId = r.Id
                                INNER JOIN Branches c ON e.BranchId = c.Id";

                MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                adapter.Fill(dataTable);

                connection.Close();
            }

            return dataTable;
        }

        public void Delete(int Id)
        {
            throw new NotImplementedException();
        }

        public void EditCurrentUser(string NameOfProp, string Value)
        {
            using (var connection = GetConnection())
            using (var command = new MySqlCommand())
            {

                connection.Open();

                command.Connection = connection;
                command.CommandText = $"UPDATE employees SET {NameOfProp} = @value, UpdatedAt = NOW() " +
                    $"WHERE Id = {UserData.Id}";
                command.Parameters.AddWithValue("value", Value);

                try
                {
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        public void FireEmployee(int Id)
        {
            using (var connection = GetConnection())

            using (var command = new MySqlCommand())
            {
                
                connection.Open();

                command.Connection = connection;
                command.CommandText = "SET foreign_key_checks = 0";
                command.ExecuteNonQuery();

                command.CommandText = @"INSERT INTO dismissed_employees (BranchId, RoleId, CreatedAt, UpdatedAt, DeletedAt, " +
                            "Login, Password, Salt, Name, Surname, Patronomic, PhoneNumber, Email, BirthDay, Address, ProfileImage) "+
                            "SELECT BranchId, RoleId, CreatedAt, UpdatedAt, NOW() as DeletedAt, Login, Password, Salt, Name, " + 
                            "Surname, Patronomic, PhoneNumber, Email, BirthDay, Address, ProfileImage " +
                            "FROM employees " +
                            "WHERE Id = @Id";

                command.Parameters.AddWithValue("@Id", Id);
                command.ExecuteNonQuery();

                command.CommandText = "DELETE FROM employees WHERE Id = @Id";
                command.ExecuteNonQuery();

                command.CommandText = "SET foreign_key_checks = 1";
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
                command.CommandText = @"SELECT ROW_NUMBER() OVER() AS '№',
                                e.Id,
                                c.Address AS 'Филиал', r.Title AS 'Роль', 
                                e.Name AS 'Имя', e.Surname AS 'Фамилия', 
                                e.Patronomic AS 'Отчество', e.PhoneNumber AS 'Номер телефона', 
                                e.Email AS 'Почта', DATE_FORMAT(e.BirthDay, '%d-%m-%Y') AS 'Дата рождения',
                                e.Address AS 'Адрес' 
                        FROM Employees e 
                        INNER JOIN Roles r ON e.RoleId = r.Id
                        INNER JOIN Branches c ON e.BranchId = c.Id";

                MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                adapter.Fill(dataTable);

                connection.Close();
            }

            // Удалите время из даты рождения
            foreach (DataRow row in dataTable.Rows)
            {
                DateTime birthDay = DateTime.Parse(row["Дата рождения"].ToString());
                row["Дата рождения"] = birthDay.ToShortDateString(); // Преобразовать в строку даты без времени
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
                    UserData.CreatedAt = reader.GetDateTime(2).ToString("yyyy-MM-dd");
                    UserData.Name = reader[3].ToString();
                    UserData.Surname = reader[4].ToString();
                    UserData.Patronomic = reader[5].ToString();
                    UserData.PhoneNumber = reader[6].ToString() != null ? reader[6].ToString() : "Не введен";
                    UserData.Email = reader[7].ToString() != null ? reader[7].ToString() : "Не введен"; ;
                    UserData.BirthDay = reader.GetDateTime(8).ToString("yyyy-MM-dd");
                    UserData.Address = reader[9].ToString() != null ? reader[9].ToString() : "Не введен"; ;

                    if (reader[10] != DBNull.Value)
                    {
                        byte[] imageData = (byte[])reader[10];
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
            string mac = (from nic in NetworkInterface.GetAllNetworkInterfaces()
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
            string mac = (from nic in NetworkInterface.GetAllNetworkInterfaces()
                          where nic.OperationalStatus == OperationalStatus.Up
                          select nic.GetPhysicalAddress().ToString()).FirstOrDefault();
            using (var connection = GetConnection())
            using (var command = new MySqlCommand())
            {

                connection.Open();

                command.Connection = connection;
                command.CommandText = "DELETE IGNORE FROM autorizeddevices WHERE DeviceMac = @EmpId";
                command.Parameters.AddWithValue("EmpId", mac);

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
                    "Patronomic, BirthDay, CreatedAt) "
                    + "VALUES(@role, @branch, @username, @password, @Salt, @name, @surname, " +
                    "@patronomic, @birthday, NOW())";
                command.Parameters.AddWithValue("role", empData.Role + 1);
                command.Parameters.AddWithValue("branch", empData.Branch + 1);
                command.Parameters.AddWithValue("username", empData.Login);
                string salt = BCrypt.Net.BCrypt.GenerateSalt();
                command.Parameters.AddWithValue("password", BCrypt.Net.BCrypt.HashPassword(empData.Password, salt));
                command.Parameters.AddWithValue("Salt", salt);
                command.Parameters.AddWithValue("name", empData.Name);
                command.Parameters.AddWithValue("surname", empData.Surname);
                command.Parameters.AddWithValue("patronomic", empData.Patronomic);

                command.Parameters.AddWithValue("birthday", DateTime.Parse(empData.BirthDay).ToString("yyyy-MM-dd"));

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

        public string GetBranchById(int BranchId)
        {
            using (var connection = GetConnection())
            using (var command = new MySqlCommand())
            {
                connection.Open();

                command.Connection = connection;
                command.CommandText = "SELECT address FROM branches WHERE Id = @Branchid";

                command.Parameters.AddWithValue("Branchid", BranchId);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {

                        return reader[0].ToString();
                    }
                    else return "";
                }

            }
        }

        public void UpdateCurrentUserPicture(byte[] picture)
        {
            using (var connection = GetConnection())
            using (var command = new MySqlCommand())
            {

                connection.Open();

                command.Connection = connection;
                command.CommandText = $"UPDATE employees SET ProfileImage = @value, UpdatedAt = NOW() " +
                    $"WHERE Id = {UserData.Id}";
                command.Parameters.AddWithValue("value", picture);

                try
                {
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        public void UpdateEmployee(EmpData data)
        {
            using (var connection = GetConnection())
            using (var command = new MySqlCommand())
            {
                connection.Open();

                command.Connection = connection;
                command.CommandText = @"UPDATE employees 
                                SET
                                    UpdatedAt = NOW(),
                                    Name = @Name, 
                                    Surname = @Surname, 
                                    Patronomic = @Patronomic, 
                                    PhoneNumber = @PhoneNumber, 
                                    Email = @Email, 
                                    Address = @Address
                                WHERE Id = @Id";
                command.Parameters.AddWithValue("Id", data.Id); // Замените UserData на имя объекта данных сотрудника, переданного в параметре
                command.Parameters.AddWithValue("Name", data.Name);
                command.Parameters.AddWithValue("Surname", data.Surname);
                command.Parameters.AddWithValue("Patronomic", data.Patronomic);
                command.Parameters.AddWithValue("PhoneNumber", data.PhoneNumber);
                command.Parameters.AddWithValue("Email", data.Email);
                command.Parameters.AddWithValue("Address", data.Address);

                command.ExecuteNonQuery();

                connection.Close();
            }
        }



    }
}

