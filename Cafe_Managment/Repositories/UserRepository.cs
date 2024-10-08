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
using ExCSS;
using System.Xml.Linq;
using Google.Protobuf.WellKnownTypes;
using System.Diagnostics;

namespace Cafe_Managment.Repositories
{
    internal class UserRepository : RepositoryBase, IUserRepository
    {
        public int AuthenticateUser(NetworkCredential credential)
        {
            MySqlConnection connection = null;

            try
            {
                connection = GetConnection();
                connection.Open();

                using (var command = new MySqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = "SELECT Id, Password, Salt, IsDismissed FROM employees WHERE Login = @Username";
                    command.Parameters.AddWithValue("@Username", credential.UserName);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            
                            if (reader.FieldCount < 4)
                            {
                                return -1; 
                            }
                            bool isDismissed = reader.GetBoolean(3);
                            if (isDismissed)
                            {
                                return 1; // 1 - сотрудник уволен
                            }
                            string storedPassword = reader.GetString(1); // Пароль из базы данных
                            string salt = reader.GetString(2); // Соль для пароля

                            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(credential.Password, storedPassword); // Проверка пароля

                            if (isPasswordValid)
                            {
                                UserData.Id = reader.GetInt32(0); // Установить идентификатор
                                return 0; // 0 - успешная аутентификация
                            }

                            return 3; // 3 - неверный пароль
                        }

                        return 2; // 2 - пользователь не найден
                    }
                }
            }
            catch (MySqlException ex)
            {
                Debug.WriteLine($"Ошибка базы данных MySQL: {ex.Message}");
                return -1; // -1 - ошибка базы данных
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Произошла ошибка: {ex.Message}");
                return -1; // -1 - общая ошибка
            }
            finally
            {
                if (connection != null && connection.State == ConnectionState.Open)
                {
                    connection.Close(); // Закрыть соединение
                }
            }
        }




        public DataTable GetDismissedEmployees()
        {
            DataTable dataTable = new DataTable();
            MySqlConnection connection = null;

            try
            {
                connection = GetConnection();
                connection.Open();

                using (var command = new MySqlCommand())
                {
                    command.Connection = connection;
                    //ROW_NUMBER() OVER() AS '№'
                    command.CommandText =
                        @"SELECT c.Address AS 'Branch', r.Title, e.Name, e.Surname, e.Patronomic, e.PhoneNumber, e.Email, DATE_FORMAT(e.BirthDay, '%d-%m-%Y') AS 'BirthDay', e.Login, e.Address, e.DeletedAt, e.CreatedAt, e.IsDismissed
                                FROM dismissed_employees e 
                                INNER JOIN roles r ON e.RoleId = r.Id
                                INNER JOIN branches c ON e.BranchId = c.Id";

                    MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                    adapter.Fill(dataTable); // Заполняем DataTable
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"Ошибка базы данных MySQL: {ex.Message}");
                dataTable = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка: {ex.Message}");
                dataTable = null;
            }
            finally
            {
                if (connection != null && connection.State == ConnectionState.Open)
                {
                    connection.Close(); // Закрываем соединение
                }
            }

            return dataTable; // Возвращаем заполненную таблицу
        }


        public void Delete(int Id)
        {
            throw new NotImplementedException();
        }

        public void EditCurrentUser(string nameOfProp, string value)
        {
            MySqlConnection connection = null;

            try
            {
                connection = GetConnection();
                connection.Open();

                using (var command = new MySqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = $"UPDATE employees SET {nameOfProp} = @value, UpdatedAt = NOW() WHERE Id = @userId";
                    command.Parameters.AddWithValue("value", value);
                    command.Parameters.AddWithValue("userId", UserData.Id);

                    command.ExecuteNonQuery();
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"Ошибка при работе с базой данных: {ex.Message}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Общая ошибка: {ex.Message}");
            }
            finally
            {
                if (connection != null && connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }


        public void FireEmployee(int id)
        {
            MySqlConnection connection = null;

            try
            {
                connection = GetConnection();
                connection.Open();

                using (var command = new MySqlCommand())
                {
                    command.Connection = connection;

                    // Отключаем проверки внешних ключей
                    command.CommandText = "SET foreign_key_checks = 0";
                    command.ExecuteNonQuery();

                    // Сначала установим IsDismissed в 1
                    command.CommandText = "UPDATE employees SET IsDismissed = 1 WHERE Id = @Id";
                    command.Parameters.AddWithValue("@Id", id);
                    command.ExecuteNonQuery();

                    // Теперь перемещаем сотрудника в dismissed_employees
                    command.CommandText = @"INSERT INTO dismissed_employees 
                                    (BranchId, RoleId, CreatedAt, UpdatedAt, DeletedAt, Login, Password, Salt, Name, Surname, 
                                    Patronomic, PhoneNumber, Email, BirthDay, Address, ProfileImage) 
                                    SELECT BranchId, RoleId, CreatedAt, UpdatedAt, NOW() as DeletedAt, Login, Password, Salt, Name, 
                                    Surname, Patronomic, PhoneNumber, Email, BirthDay, Address, ProfileImage 
                                    FROM employees 
                                    WHERE Id = @Id";
                    command.ExecuteNonQuery();

                    // Удаляем сотрудника из employees
                    command.CommandText = "DELETE FROM employees WHERE Id = @Id";
                    command.ExecuteNonQuery();

                    // Включаем проверки внешних ключей
                    command.CommandText = "SET foreign_key_checks = 1";
                    command.ExecuteNonQuery();
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"Ошибка при работе с базой данных: {ex.Message}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка: {ex.Message}");
            }
            finally
            {
                if (connection != null && connection.State == ConnectionState.Open)
                {
                    connection.Close(); 
                }
            }
        }



        public DataTable GetByAll()
        {
            DataTable dataTable = new DataTable();
            MySqlConnection connection = null;

            try
            {
                connection = GetConnection();
                connection.Open();

                using (var command = new MySqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = $@"SELECT e.Id, c.Address AS 'Branch', r.Title, e.Name, e.Surname, e.Patronomic, e.PhoneNumber, e.Email, DATE_FORMAT(e.BirthDay, '%d-%m-%Y') AS 'BirthDay', e.Login, e.Address, e.UpdatedAt, e.CreatedAt, e.ProfileImage
                        FROM employees e 
                        INNER JOIN roles r ON e.RoleId = r.Id
                        INNER JOIN branches c ON e.BranchId = c.Id
                        WHERE e.BranchId = { UserData.BranchId}";

                    MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                    adapter.Fill(dataTable);

                    // Удаление времени из даты рождения
                    foreach (DataRow row in dataTable.Rows)
                    {
                        DateTime birthDay = DateTime.Parse(row["BirthDay"].ToString());
                        row["BirthDay"] = birthDay.ToShortDateString();
                    }
                }
            }
            catch (MySqlException ex)
            {
                Debug.WriteLine($"Ошибка базы данных: {ex.Message}");
            }
            catch (FormatException ex)
            {
                Debug.WriteLine($"Ошибка преобразования данных: {ex.Message}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Произошла непредвиденная ошибка: {ex.Message}");
            }
            finally
            {
                if (connection != null && connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }

            return dataTable;
        }


        public void GetById()
        {
            MySqlConnection connection = null;

            try
            {
                connection = GetConnection();
                connection.Open();

                using (var command = new MySqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = "SELECT RoleId, BranchId, CreatedAt, Name, Surname, Patronomic, " +
                        "PhoneNumber, Email, BirthDay, Address, ProfileImage FROM employees WHERE Id = @userId";
                    command.Parameters.AddWithValue("userId", UserData.Id);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.HasRows && reader.Read())
                        {
                            UserData.RoleId = Convert.ToInt32(reader["RoleId"]);
                            UserData.BranchId = Convert.ToInt32(reader["BranchId"]);
                            UserData.CreatedAt = reader.GetDateTime("CreatedAt").ToString("yyyy-MM-dd");
                            UserData.Name = reader["Name"].ToString();
                            UserData.Surname = reader["Surname"].ToString();
                            UserData.Patronomic = reader["Patronomic"].ToString();
                            UserData.PhoneNumber = reader.IsDBNull(reader.GetOrdinal("PhoneNumber"))
                                ? "Не введен"
                                : reader["PhoneNumber"].ToString();
                            UserData.Email = reader.IsDBNull(reader.GetOrdinal("Email"))
                                ? "Не введен"
                                : reader["Email"].ToString();
                            UserData.BirthDay = reader.GetDateTime("BirthDay").ToString("yyyy-MM-dd");
                            UserData.Address = reader.IsDBNull(reader.GetOrdinal("Address"))
                                ? "Не введен"
                                : reader["Address"].ToString();

                            if (reader.IsDBNull(reader.GetOrdinal("ProfileImage")))
                            {
                                UserData.ProfileImage = new BitmapImage(new Uri("/Images/EmptyImage.png", UriKind.Relative));
                            }
                            else
                            {
                                byte[] imageData = (byte[])reader["ProfileImage"];
                                UserData.ProfileImage = ConvertByteArrayToBitmapImage(imageData);
                            }
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"Ошибка базы данных: {ex.Message}");
            }
            catch (FormatException ex)
            {
                MessageBox.Show($"Ошибка преобразования данных: {ex.Message}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла непредвиденная ошибка: {ex.Message}");
            }
            finally
            {
                if (connection != null && connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }


        public void RememberCurrentUser()
        {
            MySqlConnection connection = null;

            try
            {
                connection = GetConnection();
                connection.Open();

                var macAddress = (from nic in NetworkInterface.GetAllNetworkInterfaces()
                                  where nic.OperationalStatus == OperationalStatus.Up
                                  select nic.GetPhysicalAddress().ToString()).FirstOrDefault();

                using (var command = new MySqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = "INSERT IGNORE INTO autorizeddevices VALUES (@Mac, @UserId)";
                    command.Parameters.AddWithValue("@Mac", macAddress);
                    command.Parameters.AddWithValue("@UserId", UserData.Id);

                    command.ExecuteNonQuery();
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"Ошибка базы данных: {ex.Message}");
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show($"Ошибка при получении MAC-адреса: {ex.Message}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла непредвиденная ошибка: {ex.Message}");
            }
            finally
            {
                if (connection != null && connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }


        public BitmapImage ConvertByteArrayToBitmapImage(byte[] byteArray)
        {
            if (byteArray == null || byteArray.Length == 0)
            {
                MessageBox.Show("The byte array is null or empty.");
                return null;
            }

            try
            {
                using (MemoryStream stream = new MemoryStream(byteArray))
                {
                    BitmapImage image = new BitmapImage();
                    image.BeginInit();
                    image.StreamSource = stream;
                    image.CacheOption = BitmapCacheOption.OnLoad;

                    image.EndInit();
                    stream.Close(); // Закрытие потока после использования

                    return image;
                }
            }
            catch (OutOfMemoryException ex)
            {
                MessageBox.Show("The image is too large or memory is insufficient: " + ex.Message);
                return null;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error converting byte array to BitmapImage: " + ex.Message);
                return null;
            }
        }


        public bool GetByMac()
        {
            string mac = null;

            try
            {
                mac = (from nic in NetworkInterface.GetAllNetworkInterfaces()
                       where nic.OperationalStatus == OperationalStatus.Up
                       select nic.GetPhysicalAddress().ToString()).FirstOrDefault();

                if (string.IsNullOrEmpty(mac))
                {
                    throw new Exception("No valid MAC address found.");
                }

                using (var connection = GetConnection())
                using (var command = new MySqlCommand())
                {
                    connection.Open();

                    command.Connection = connection;
                    command.CommandText = "SELECT EmployeeId FROM autorizeddevices WHERE DeviceMac=@Mac";
                    command.Parameters.AddWithValue("Mac", mac);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            UserData.Id = int.Parse(reader[0].ToString());
                            return true; // Сотрудник найден
                        }
                    }
                }

                return false; // Сотрудник не найден
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"Database error: {ex.Message}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }

            return false; // Возвращаем false в случае ошибки или неудачи
        }


        public void ForgetCurrentUser()
        {
            string mac = null;
            MySqlConnection connection = null;

            try
            {
                mac = (from nic in NetworkInterface.GetAllNetworkInterfaces()
                       where nic.OperationalStatus == OperationalStatus.Up
                       select nic.GetPhysicalAddress().ToString()).FirstOrDefault();

                if (string.IsNullOrEmpty(mac))
                {
                    throw new Exception("No valid MAC address found.");
                }

                connection = GetConnection();

                using (var command = new MySqlCommand())
                {
                    connection.Open();
                    command.Connection = connection;

                    command.CommandText = "DELETE FROM autorizeddevices WHERE DeviceMac = @Mac";
                    command.Parameters.AddWithValue("@Mac", mac);

                    command.ExecuteNonQuery();
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"Database error: {ex.Message}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
            finally
            {
                if (connection != null && connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }


        public string GetRoleById(int roleId)
        {
            string roleTitle = null;
            MySqlConnection connection = null;

            try
            {
                connection = GetConnection();
                connection.Open();

                using (var command = new MySqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = "SELECT Title FROM roles WHERE Id = @RoleId";
                    command.Parameters.AddWithValue("RoleId", roleId);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            roleTitle = reader["Title"].ToString();
                        }
                        else
                        {
                            throw new Exception("Role not found.");
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"Database error: {ex.Message}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
            finally
            {
                if (connection != null && connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }

            return roleTitle;
        }


        public void Add(EmpData empData)
        {
            MySqlConnection connection = null;
            try
            {
                connection = GetConnection();
                connection.Open();

                using (var command = new MySqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = "INSERT INTO employees (RoleId, BranchId, Login, Password, Salt, Name, Surname, " +
                        "Patronomic, BirthDay, CreatedAt) "
                        + "VALUES(@role, @branch, @username, @password, @Salt, @name, @surname, " +
                        "@patronomic, @birthday, NOW())";

                    // Добавляем параметры
                    command.Parameters.AddWithValue("role", empData.Role + 1);
                    command.Parameters.AddWithValue("branch", empData.Branch + 1);
                    command.Parameters.AddWithValue("username", empData.Login);

                    // Генерируем соль и хешируем пароль
                    string salt = BCrypt.Net.BCrypt.GenerateSalt();
                    command.Parameters.AddWithValue("password", BCrypt.Net.BCrypt.HashPassword(empData.Password, salt));
                    command.Parameters.AddWithValue("Salt", salt);

                    command.Parameters.AddWithValue("name", empData.Name);
                    command.Parameters.AddWithValue("surname", empData.Surname);
                    command.Parameters.AddWithValue("patronomic", empData.Patronomic);
                    command.Parameters.AddWithValue("birthday", DateTime.Parse(empData.BirthDay).ToString("yyyy-MM-dd"));

                    // Выполнение команды
                    command.ExecuteNonQuery();
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"Ошибка базы данных: {ex.Message}");
                Console.WriteLine($"Подробности: {ex}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка: {ex.Message}");
                Console.WriteLine($"Подробности: {ex}");
            }
            finally
            {
                if (connection != null && connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }


        public bool IfUserExists(string Username)
        {
            MySqlConnection connection = null;
            try
            {
                connection = GetConnection();
                connection.Open();

                using (var command = new MySqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = "SELECT * FROM employees WHERE Login = @Username";
                    command.Parameters.AddWithValue("Username", Username);

                    using (var reader = command.ExecuteReader())
                    {
                        return reader.Read(); // Если нашелся хотя бы один результат, возвращаем true
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"Ошибка базы данных: {ex.Message}");
                return false; // Предполагаем, что при ошибке пользователь не найден
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка: {ex.Message}");
                return false;
            }
            finally
            {
                if (connection != null && connection.State == ConnectionState.Open)
                {
                    connection.Close(); // Гарантируем закрытие соединения
                }
            }
        }


        public List<string> GetRoles()
        {
            List<string> roles = new List<string>();
            MySqlConnection connection = null;

            try
            {
                connection = GetConnection();
                connection.Open();

                using (var command = new MySqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = "SELECT Title FROM roles";

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            roles.Add(reader["Title"].ToString());
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"Ошибка базы данных: {ex.Message}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка: {ex.Message}");
            }
            finally
            {
                if (connection != null && connection.State == ConnectionState.Open)
                {
                    connection.Close(); // Гарантируем закрытие соединения
                }
            }

            return roles; // Возвращаем список ролей (может быть пустым при ошибке)
        }


        public List<string> GetBranches()
        {
            List<string> branches = new List<string>();
            MySqlConnection connection = null;

            try
            {
                connection = GetConnection();
                connection.Open();

                using (var command = new MySqlCommand("SELECT address FROM branches", connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            branches.Add(reader["address"].ToString());
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"Ошибка при доступе к базе данных: {ex.Message}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла непредвиденная ошибка: {ex.Message}");
            }
            finally
            {
                if (connection != null && connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }

            return branches;
        }


        public string GetBranchById(int branchId)
        {
            string branchAddress = "";
            MySqlConnection connection = null;

            try
            {
                connection = GetConnection();
                connection.Open();

                using (var command = new MySqlCommand("SELECT address FROM branches WHERE Id = @BranchId", connection))
                {
                    command.Parameters.AddWithValue("@BranchId", branchId);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            branchAddress = reader["address"].ToString();
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"Ошибка при запросе базы данных: {ex.Message}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка: {ex.Message}");
            }
            finally
            {
                if (connection != null && connection.State == ConnectionState.Open)
                {
                    connection.Close(); // Гарантируем закрытие соединения
                }
            }

            return branchAddress; // Возвращаем пустую строку, если ничего не найдено
        }


        public void UpdateCurrentUserPicture(byte[] picture)
        {
            if (picture == null)
            {
                MessageBox.Show("Переданное изображение не может быть пустым.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            MySqlConnection connection = null;

            try
            {
                connection = GetConnection();
                connection.Open();

                using (var command = new MySqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = $"UPDATE employees SET ProfileImage = @value, UpdatedAt = NOW() WHERE Id = @UserId";
                    command.Parameters.AddWithValue("@value", picture);
                    command.Parameters.AddWithValue("@UserId", UserData.Id);

                    command.ExecuteNonQuery(); // Исполнение команды
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"Ошибка при обновлении изображения в базе данных: {ex.Message}", "Ошибка БД", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла непредвиденная ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                if (connection != null && connection.State == ConnectionState.Open)
                {
                    connection.Close(); // Закрытие соединения
                }
            }
        }


        public void UpdateEmployee(EmpData data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data), "Параметр данных сотрудника не может быть пустым.");
            }

            MySqlConnection connection = null;

            try
            {
                connection = GetConnection();
                connection.Open();

                using (var command = new MySqlCommand())
                {
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

                    // Проверяем данные перед добавлением параметров
                    if (string.IsNullOrWhiteSpace(data.Name) ||
                        string.IsNullOrWhiteSpace(data.Surname))
                    {
                        throw new ArgumentException("Имя и фамилия не могут быть пустыми.");
                    }

                    command.Parameters.AddWithValue("@Id", data.Id);
                    command.Parameters.AddWithValue("@Name", data.Name);
                    command.Parameters.AddWithValue("@Surname", data.Surname);
                    command.Parameters.AddWithValue("@Patronomic", data.Patronomic ?? ""); // Убедиться, что не `null`
                    command.Parameters.AddWithValue("@PhoneNumber", data.PhoneNumber ?? "");
                    command.Parameters.AddWithValue("@Email", data.Email ?? "");
                    command.Parameters.AddWithValue("@Address", data.Address ?? "");

                    command.ExecuteNonQuery();
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"Ошибка при обновлении данных сотрудника: {ex.Message}", "Ошибка БД", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла непредвиденная ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                if (connection != null && connection.State == ConnectionState.Open)
                {
                    connection.Close(); // Гарантированное закрытие соединения
                }
            }
        }
        public void AddBranch(string branchName)
        {
            MySqlConnection connection = null;

            try
            {
                connection = GetConnection();
                connection.Open();

                using (var command = new MySqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = @"INSERT INTO branches (Address, CreatedAt) VALUES (@Address, @CreatedAt)";
                    command.Parameters.AddWithValue("@Address", branchName);
                    command.Parameters.AddWithValue("@CreatedAt", DateTime.Now);

                    command.ExecuteNonQuery();
                }
            }
            catch (MySqlException ex)
            {
                throw new Exception($"Ошибка при добавлении филиала: {ex.Message}", ex);
            }
            finally
            {
                if (connection != null && connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }

    }
}

