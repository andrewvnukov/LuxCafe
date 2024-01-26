using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Windows;
using Cafe_Managment.View;

namespace Cafe_Managment
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected void ApplicationStart(object sender, StartupEventArgs e)
        {
            var loginView = new Autorization();
            loginView.Show();

            loginView.IsVisibleChanged += (s, ev) =>
            {
                if (!loginView.IsVisible && loginView.IsLoaded && loginView.IsEnabled)
                {
                    var mainMenu = new MainMenu();
                    mainMenu.Show();
                    loginView.Close();
                    
                }
            };
        }
    }
}
