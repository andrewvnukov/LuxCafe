using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
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
using MySql.Data.MySqlClient;
using System.IO;
using System.Drawing;


namespace LuxCafe
{
    /// <summary>
    /// Логика взаимодействия для MainMenu.xaml
    /// </summary>
    public partial class MainMenu : Window
    {
        int UserId;
        string connectionString;


        public MainMenu(int UserID)
        {   

            InitializeComponent();
            this.HorizontalAlignment = HorizontalAlignment.Center;
            this.VerticalAlignment = VerticalAlignment.Center;

            this.UserId= UserID;

            try
            {
                string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString;

                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    // Замените на фактический ID пользователя

                    string query = "SELECT employeename, employeesurname, employeerole FROM employees WHERE id = @UserId";
                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@UserId", UserId);
                        connection.Open();

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows && reader.Read())
                            {
                                string firstName = reader["employeename"].ToString();
                                string lastName = reader["employeesurname"].ToString();
                                string position = reader["employeerole"].ToString();

                                // Теперь у вас есть имя, фамилия и должность пользователя по его ID
                                UserName.Text = firstName + " " + lastName;
                                UserRole.Text = position;

                            }
                            else
                            {
                                Console.WriteLine("User not found");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Обработка ошибок
                Console.WriteLine("Error: " + ex.Message);
            }
            loadImage();

        }

        void loadImage()
        {
            try
            {
                string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString;

                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                     // Замените на фактический ID пользователя

                    string query = "SELECT photodata FROM userphotos WHERE userid = @UserId";
                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@UserId", UserId);
                        connection.Open();

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows && reader.Read())
                            {
                                // Получаем бинарные данные изображения
                                byte[] imageData = (byte[])reader["photodata"];

                                // Отображаем изображение в элементе Image в WPF
                                BitmapImage bitmapImage = new BitmapImage();
                                bitmapImage.BeginInit();
                                bitmapImage.StreamSource = new MemoryStream(imageData);
                                bitmapImage.EndInit();

                                // Здесь 'YourImageControl' - это ваш элемент управления Image в XAML
                                UserImage.Source = bitmapImage;
                            }
                            else
                            {
                                Console.WriteLine("User not found");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Обработка ошибок
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var tappedWindow = e.OriginalSource as Button;

            while (ActiveWindow.NavigationService.CanGoBack)
            {
                ActiveWindow.NavigationService.RemoveBackEntry();
            }

            switch (tappedWindow.Name)
                {
                default:
                    
                    ActiveWindow.Source = new Uri("pack://application:,,,/Profile.xaml"); break;
                case "EmployeesButton":
                    ActiveWindow.Source = new Uri("pack://application:,,,/Employees.xaml"); break;
                case "StatisticButton":
                    ActiveWindow.Source = new Uri("pack://application:,,,/Statistic.xaml"); break;
                case "MenuButton":
                    ActiveWindow.Source = new Uri("pack://application:,,,/Menu.xaml"); break;
                case "DishesButton":
                    ActiveWindow.Source = new Uri("pack://application:,,,/Dishes.xaml"); break;
                case "OrderButton":
                    ActiveWindow.Source = new Uri("pack://application:,,,/Order.xaml"); break;
            }



        }

        private void EmployeesButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
