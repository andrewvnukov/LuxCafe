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
            MySqlConnection connection = null;

            try
            {
                connection = GetConnection();
                connection.Open();

                using (var command = new MySqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = @"
                SELECT dish_name, SUM(quantity) as total_quantity
                FROM orderdetails
                GROUP BY dish_name
                ORDER BY total_quantity DESC
                LIMIT 3";

                    using (var reader = command.ExecuteReader())
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
            catch (MySqlException ex)
            {
                MessageBox.Show($"Ошибка при извлечении данных из базы: {ex.Message}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла неожиданная ошибка: {ex.Message}");
            }
            finally
            {
                if (connection != null && connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }

            return dishData;
        }

        public Dictionary<DateTime, double> GetProfitForTimePeriod(DateTime startDate, DateTime endDate)
        {
            var profits = new Dictionary<DateTime, double>();
            MySqlConnection connection = null;

            try
            {
                connection = GetConnection();
                connection.Open();

                using (var command = new MySqlCommand())
                {
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

                            // Проверка, существует ли уже ключ в словаре
                            if (profits.ContainsKey(date))
                            {
                                MessageBox.Show($"Предупреждение: дублированный ключ для даты {date}. Пропускаем.");
                            }
                            else
                            {
                                profits[date] = profit;
                            }
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"Ошибка MySQL: {ex.Message}");
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

            return profits;
        }

        public Dictionary<string, double> GetPopularDishes()
        {
            var popularDishes = new Dictionary<string, double>();
            MySqlConnection connection = null;

            try
            {
                connection = GetConnection();
                connection.Open();

                using (var command = new MySqlCommand())
                {
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
                LIMIT 5"; // Возвращаем 5 самых популярных блюд

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string dishName = reader.GetString("dish_name");
                            double totalQuantity = reader.GetDouble("total_quantity");

                            // Проверка на дубликаты в словаре
                            if (popularDishes.ContainsKey(dishName))
                            {
                                MessageBox.Show($"Предупреждение: дублированное имя блюда '{dishName}'.");
                            }
                            else
                            {
                                popularDishes[dishName] = totalQuantity;
                            }
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"Ошибка MySQL: {ex.Message}");
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

            return popularDishes;
        }

        public Dictionary<string, double> GetUnpopularDishes()
        {
            var unpopularDishes = new Dictionary<string, double>();
            MySqlConnection connection = null;

            try
            {
                connection = GetConnection();
                connection.Open();

                using (var command = new MySqlCommand())
                {
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
                LIMIT 5"; // Возвращаем 5 самых непопулярных блюд

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string dishName = reader.GetString("dish_name");
                            double totalQuantity = reader.GetDouble("total_quantity");

                            if (unpopularDishes.ContainsKey(dishName))
                            {
                                MessageBox.Show($"Предупреждение: дублированное имя блюда '{dishName}'.");
                            }
                            else
                            {
                                unpopularDishes[dishName] = totalQuantity;
                            }
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"Ошибка MySQL: {ex.Message}");
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

            return unpopularDishes;
        }
        public List<DishData> GetDishPopularityTrend(int dishId, DateTime startDate, DateTime endDate)
        {
            List<DishData> trendData = new List<DishData>();
            MySqlConnection connection = null;

            try
            {
                connection = GetConnection();
                connection.Open();

                using (var command = new MySqlCommand())
                {
                    command.Connection = connection;

                    // SQL-запрос для получения суммарного количества порций блюда по дням в заданном периоде
                    command.CommandText = @"
                SELECT DATE(orders.CreatedAt) AS OrderDate, SUM(orderdetails.Quantity) AS TotalQuantity
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
                                Count = reader.GetInt32("TotalQuantity")
                            };

                            trendData.Add(dishData);
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"Ошибка MySQL: {ex.Message}");
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

            return trendData;
        }

        public IEnumerable<DishData> GetAllDishes()
        {
            List<DishData> dishes = new List<DishData>();
            MySqlConnection connection = null;

            try
            {
                connection = GetConnection();
                connection.Open();

                using (var command = new MySqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = @"SELECT
                                      am.Id,
                                      da.Title AS Title,
                                      da.Description,
                                      da.Composition,
                                      am.Price,                                     
                                      am.TransferedAt AS CreatedAt
                                    FROM activemenu am
                                    INNER JOIN disharchive da ON am.DishId = da.Id
                                    INNER JOIN categories c ON da.CategoryId = c.Id";

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var dish = new DishData
                            {
                                Id = reader.GetInt32("Id"),
                                Title = reader.IsDBNull(reader.GetOrdinal("Title")) ? "Untitled" : reader.GetString("Title"),
                                Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? string.Empty : reader.GetString("Description"),
                                Composition = reader.IsDBNull(reader.GetOrdinal("Composition")) ? string.Empty : reader.GetString("Composition"),
                                CreatedAt = reader.GetDateTime("CreatedAt"),
                                Price = reader.IsDBNull(reader.GetOrdinal("Price")) ? "0.00" : reader.GetDecimal("Price").ToString("F2")
                            };

                            dishes.Add(dish);
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"Ошибка базы данных MySQL: {ex.Message}");
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

            return dishes;
        }
    }
}