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

namespace LuxCafe
{
    /// <summary>
    /// Логика взаимодействия для MainMenu.xaml
    /// </summary>
    public partial class MainMenu : Window
    {
        public MainMenu()
        {
            InitializeComponent();
            this.HorizontalAlignment = HorizontalAlignment.Center;
            this.VerticalAlignment = VerticalAlignment.Center;
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
    }
}
