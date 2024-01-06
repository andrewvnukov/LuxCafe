using MySql.Data.MySqlClient;
using Notifications.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ToastNotifications.Display;

namespace LuxCafe
{
    /// <summary>
    /// Логика взаимодействия для Registration.xaml
    /// </summary>
    public partial class Registration : Window
    {
        public Registration()
        {
            InitializeComponent();
        }

        private void SignUp_Click(object sender, RoutedEventArgs e)
        {
            string login = Login_TextBox.Text;
            string name = Name_TextBox.Text;
            string surname = Surname_TextBox.Text;
            string patronymic = Patronymic_TextBox.Text;
            string role = Role_ComboBox.Text;
            string phoneNumber = PhoneNumber_TextBox.Text;
            string password = FirstPassword.Password;
            string secondpassword = SecondPassword.Password;

            string salt = BCrypt.Net.BCrypt.GenerateSalt();
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password, salt);

            //if (!IsRegistrationValid())
            //{
            //    return;
            //}

            try
            {
                string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString;

                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {

                    connection.Open();

                    // Создайте SQL-запрос для вставки нового пользователя
                    string insertQuery = "INSERT INTO employees (employeeusername, employeepassword, salt, " +
                        "employeerole, status, employeename, employeesurname, employeepatronomic," +
                        " employeephonenumber) VALUES (@login, @password," +
                        " @salt, @role, @status, @name, @surname, @patronymic, @phonenumber)";
                    MySqlCommand cmd = new MySqlCommand(insertQuery, connection);

                    // Задайте параметры для запроса
                    cmd.Parameters.AddWithValue("@login", login);
                    cmd.Parameters.AddWithValue("@password", hashedPassword);
                    cmd.Parameters.AddWithValue("@role", role);
                    cmd.Parameters.AddWithValue("@salt", salt);
                    cmd.Parameters.AddWithValue("@status", "Работает");
                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.Parameters.AddWithValue("@surname", surname);
                    cmd.Parameters.AddWithValue("@patronymic", patronymic);
                    cmd.Parameters.AddWithValue("@phonenumber", phoneNumber);

                    // Выполните запрос
                    int rowsAffected = cmd.ExecuteNonQuery();

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при регистрации пользователя: " + ex.Message);
            }

            var notificationManager = new NotificationManager();

            notificationManager.Show(
                new NotificationContent { Title = "Notification", Message = "Notification in window!" },
                areaName: "WindowArea");
        }
    }
}
