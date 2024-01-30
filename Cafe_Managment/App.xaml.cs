using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net.Mail;
using System.Net.NetworkInformation;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Windows;
using Cafe_Managment.Repositories;
using Cafe_Managment.View;
using MySql.Data.MySqlClient;

namespace Cafe_Managment
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        //protected void ApplicationStart(object sender, StartupEventArgs e)
        //{
        //    const string connectionString = "server=sql11.freemysqlhosting.net;user=sql11680178;password=2RsC6gIPXP;database=sql11680178;";
        //    bool IsAutorized = true;
        //    MainMenu mainMenu;

        //    using (var connection = new MySqlConnection(connectionString))
        //    using (var command = new MySqlCommand())
        //    {

        //        connection.Open();

        //        var MacAddress = (from nic in NetworkInterface.GetAllNetworkInterfaces()
        //                          where nic.OperationalStatus == OperationalStatus.Up
        //                          select nic.GetPhysicalAddress().ToString()).FirstOrDefault();

        //        command.Connection = connection;
        //        command.CommandText = "SELECT ProfileId FROM autorizeddevices WHERE DeviceMac=@Mac";
        //        command.Parameters.AddWithValue("Mac", MacAddress);
                
        //        using(var reader =  command.ExecuteReader())
        //        {
        //            if (reader.Read())
        //            {
        //                Thread.CurrentPrincipal = new GenericPrincipal(new GenericIdentity(reader[0].ToString()), null);
        //            }
        //            else IsAutorized=false;
        //            connection.Close();
        //        }

                
        //    }
        //    if (IsAutorized)
        //    {
        //        mainMenu = new MainMenu();
        //        mainMenu.Show();
        //        mainMenu.IsVisibleChanged += (s, ev) =>
        //        {
        //            if (!mainMenu.IsVisible && mainMenu.IsLoaded && mainMenu.IsEnabled)
        //            {

        //                using (var connection = new MySqlConnection(connectionString))
        //                using (var command = new MySqlCommand())
        //                {

        //                    connection.Open();

        //                    var MacAddress = (from nic in NetworkInterface.GetAllNetworkInterfaces()
        //                                      where nic.OperationalStatus == OperationalStatus.Up
        //                                      select nic.GetPhysicalAddress().ToString()).FirstOrDefault();

        //                    command.Connection = connection;
        //                    command.CommandText = "DELETE FROM autorizeddevices WHERE DeviceMac=@Mac";
        //                    command.Parameters.AddWithValue("Mac", MacAddress);
        //                    command.ExecuteNonQuery();
        //                    connection.Close();
        //                }
        //                System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
        //                Application.Current.Shutdown();
        //            }
        //        };
        //    }
        //    if (!IsAutorized)
        //    {
        //        var loginView = new Autorization();
        //        loginView.Show();

        //        loginView.IsVisibleChanged += (s, ev) =>
        //        {
        //            if (!loginView.IsVisible && loginView.IsLoaded && loginView.IsEnabled)
        //            {
        //                mainMenu = new MainMenu();
        //                mainMenu.Show();
        //                loginView.Close();

        //            }
        //        };
        //    }
        //}
    }
}
