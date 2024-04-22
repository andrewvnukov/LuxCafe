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


        


        public Dictionary<DateTime, double> GetProfitForTimePeriod(DateTime startDate, DateTime endDate)
        {
            Dictionary<DateTime, double> profits = new Dictionary<DateTime, double>();

            using (var connection = GetConnection())
            using (var command = new MySqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = @"
            SELECT DATE(CreatedAt) as Date, SUM(TotalPrice) as Profit
            FROM orders
            WHERE CreatedAt >= @startDate AND CreatedAt <= @endDate
            GROUP BY DATE(CreatedAt)";

                command.Parameters.AddWithValue("@startDate", startDate);
                command.Parameters.AddWithValue("@endDate", endDate);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        DateTime date = reader.GetDateTime("Date");
                        double profit = reader.GetDouble("Profit");
                        profits[date] = profit;
                    }
                }
            }

            return profits;
        }

        public Dictionary<string, double> GetPopularDishes()
        {
            var popularDishes = new Dictionary<string, double>();

            using (var connection = GetConnection())
            using (var command = new MySqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = @"
            SELECT 
                da.Title AS dish_name,
                SUM(od.Quantity) AS total_quantity
            FROM 
                orderdetails od
                JOIN activemenu am ON od.DishId = am.Id
                JOIN disharchive da ON am.DishId = da.Id
            GROUP BY 
                da.Title
            ORDER BY 
                total_quantity DESC
            LIMIT 5"; // Возвращаем 10 самых популярных блюд

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string dishName = reader.GetString("dish_name");
                        double totalQuantity = reader.GetDouble("total_quantity");
                        popularDishes.Add(dishName, totalQuantity);
                    }
                }
            }

            return popularDishes;
        }

        public Dictionary<string, double> GetUnpopularDishes()
        {
            var unpopularDishes = new Dictionary<string, double>();

            using (var connection = GetConnection())
            using (var command = new MySqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = @"
            SELECT 
                da.Title AS dish_name,
                SUM(od.Quantity) AS total_quantity
            FROM 
                orderdetails od
                JOIN activemenu am ON od.DishId = am.Id
                JOIN disharchive da ON am.DishId = da.Id
            GROUP BY 
                da.Title
            ORDER BY 
                total_quantity ASC
            LIMIT 5"; // Возвращаем 10 самых непопулярных блюд

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string dishName = reader.GetString("dish_name");
                        double totalQuantity = reader.GetDouble("total_quantity");
                        unpopularDishes.Add(dishName, totalQuantity);
                    }
                }
            }

            return unpopularDishes;
        }


        public List<DishData> GetDishPopularityTrend(int dishId, DateTime startDate, DateTime endDate)
        {
            List<DishData> trendData = new List<DishData>();

            using (var connection = GetConnection())
            using (var command = new MySqlCommand())
            {
                connection.Open();
                command.Connection = connection;

                // SQL-запрос для получения количества заказов блюда по дням в заданном периоде
                command.CommandText = @"
                SELECT DATE(orders.CreatedAt) AS OrderDate, COUNT(*) AS OrderCount
                FROM orderdetails
                JOIN orders ON orderdetails.OrderId = orders.Id
                WHERE orderdetails.DishId = @DishId
                  AND orders.CreatedAt BETWEEN @StartDate AND @EndDate
                GROUP BY OrderDate
                ORDER BY OrderDate";

                command.Parameters.AddWithValue("@DishId", dishId);
                command.Parameters.AddWithValue("@StartDate", startDate);
                command.Parameters.AddWithValue("@EndDate", endDate);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        DishData dishData = new DishData
                        {
                            Id = dishId,
                            CreatedAt = reader.GetDateTime("OrderDate"),
                            Count = reader.GetInt32("OrderCount")
                        };

                        trendData.Add(dishData);
                    }
                }
            }

            return trendData;
        }

        public IEnumerable<DishData> GetAllDishes()
        {
            List<DishData> dishes = new List<DishData>();

            using (var connection = GetConnection())
            using (var command = new MySqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = @"SELECT 
                                  am.Id,
                                  c.Title AS Category, 
                                  da.Title AS DishName,
                                  da.Description AS Description, 
                                  da.Composition AS Composition,
                                  am.Price AS Price,                                     
                                  am.TransferedAt AS CreatedAt,
                                  am.UpdatedAt AS UpdatedAt
                                FROM activemenu am
                                INNER JOIN disharchive da ON am.DishId = da.Id
                                INNER JOIN categories c ON da.CategoryId = c.Id";

                MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);

                // Преобразование DataTable в список DishData
                foreach (DataRow row in dataTable.Rows)
                {
                    DishData dish = new DishData
                    {
                        Id = Convert.ToInt32(row["Id"]),
                        CategoryId = Convert.ToInt32(row["Category"]), // или используйте строковое значение
                        Title = row["DishName"].ToString(),
                        Description = row["Description"].ToString(),
                        Composition = row["Composition"].ToString(),
                        
                        Price = row["Price"].ToString() // Преобразование в строку
                    };

                    dishes.Add(dish);
                }

                connection.Close();
            }

            return dishes;
        }

    }
}