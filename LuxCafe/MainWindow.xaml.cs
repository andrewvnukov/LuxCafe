using MySql.Data.MySqlClient;
using Org.BouncyCastle.Crypto.Generators;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Messages;
using ToastNotifications.Position;

namespace LuxCafe
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public int UserId { get; private set; }
  

        public static string CurrentUserId;
        public MainWindow()
        {
            InitializeComponent();

        }

        ~MainWindow()
        {
            
        }
        


        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
        private void Autorization_Click(object sender, RoutedEventArgs e)
        {
            
            try
            {
                
                string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString;

                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    string username = LoginTextBox.Text;
                    string enteredPassword = PasswordTextBox.Password;

                    // Извлекаем хэшированный пароль, соль и статус из базы данных
                    string query = "SELECT employeepassword, employeerole, id, salt, status FROM employees WHERE employeeusername = @Username";
                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@Username", username);
                        connection.Open();

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows && reader.Read())
                            {
                                string storedPassword = reader["employeepassword"].ToString();
                                string userStatus = reader["status"].ToString();

                                if (userStatus.Equals("Работает", StringComparison.OrdinalIgnoreCase))
                                {
                                    // Проверяем введенный пароль
                                    if (BCrypt.Net.BCrypt.Verify(enteredPassword, storedPassword))
                                    {
                                        // Учетные данные верны, пользователь успешно вошел
                                        UserId = Convert.ToInt32(reader["id"]);
                                        string inputData = LoginTextBox.Text;

                                        MainMenu window = new MainMenu(UserId);
                                        window.Show();
                                        connection.Close();
                                        this.Close();
                                        
                                    }
                                    else
                                    {

                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка: " + ex.Message);
            }
        }

    }
}
