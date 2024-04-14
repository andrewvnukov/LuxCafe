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
    internal class StatisticsRepository : RepositoryBase, IStatisticsRepository
    {

        public Dictionary<string, double> GetDishDataFromDatabase()
        {
            Dictionary<string, double> dishData = new Dictionary<string, double>();

            using (var connection = GetConnection())
            using (var command = new MySqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = @"
                    SELECT dish_name, SUM(quantity) as total_quantity
                    FROM order_details
                    GROUP BY dish_name
                    ORDER BY total_quantity DESC
                    LIMIT 3";

                MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string dishName = reader.GetString("dish_name");
                            double totalQuantity = reader.GetDouble("total_quantity");
                            dishData.Add(dishName, totalQuantity);
                        }
                    }
                }
            }

            return dishData;
        }

        public double GetProfitForTimePeriod(DateTime startDate, DateTime endDate)
        {
            double totalProfit = 0.0;

            {
                using (var connection = GetConnection())
                using (var command = new MySqlCommand())
                {
                    connection.Open();
                    command.Connection = connection;
                    command.CommandText = @"
                    SELECT SUM(TotalPrice)
                    FROM orders
                    WHERE CreatedAt >= @startDate AND CreatedAt <= @endDate";

                    MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                    {
                        command.Parameters.AddWithValue("@startDate", startDate);
                        command.Parameters.AddWithValue("@endDate", endDate);

                        object result = command.ExecuteScalar();
                        if (result != DBNull.Value)
                        {
                            totalProfit = Convert.ToDouble(result);
                        }
                    }
                }

                return totalProfit;
            }
        }
    }
}