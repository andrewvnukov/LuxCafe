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
using System.Diagnostics;
using Cafe_Managment.Controls;
using System.Diagnostics.Metrics;

namespace Cafe_Managment.Repositories
{
    internal class DishesRepository : RepositoryBase, IDishesRepository
    {
        public DataTable GetAllDishesFromArchive()
        {
            DataTable dataTable = new DataTable();

            using (var connection = GetConnection())
            using (var command = new MySqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = @"SELECT ROW_NUMBER() OVER() AS '№',
                                      d.Id,
                                      c.Title AS 'Раздел',
                                      d.Title AS 'Название',
                                      d.Description AS 'Описание', 
                                      d.Composition AS 'Состав',
                                      d.CreatedAt AS 'Дата добавления', 
                                      d.UpdatedAt AS 'Дата последнего обновления' 
                                FROM disharchive d
                                INNER JOIN categories c ON d.CategoryId = c.Id";

                MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                adapter.Fill(dataTable);

                connection.Close();
            }

            return dataTable;
        }

        public DataTable GetAllDishesFromMenu()
        {
            DataTable dataTable = new DataTable();

            using (var connection = GetConnection())
            using (var command = new MySqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = @"SELECT ROW_NUMBER() OVER() AS '№',
                                      am.Id,
                                      c.Title AS 'Раздел', 
                                      da.Title AS 'Название',
                                      da.Description AS 'Описание', 
                                      da.Composition AS 'Состав',
                                      am.Price AS 'Стоимость',                                     
                                      am.TransferedAt AS 'Дата добавления',
                                      am.UpdatedAt AS 'Дата обновления цены'
                                FROM activemenu am
                                INNER JOIN disharchive da ON am.DishId = da.Id
                                INNER JOIN categories c ON da.CategoryId = c.Id";

                MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                adapter.Fill(dataTable);

                connection.Close();
            }

            return dataTable;
        }

        public void UpdateDish(DishData dish)
        {
            using (var connection = GetConnection()) 
            using (var command = new MySqlCommand())
            {
                connection.Open();
                try
                {
                    command.Connection = connection;
                    command.CommandText = @"UPDATE disharchive 
                                SET 
                                    Title = @Title,
                                    Description = @Description,
                                    Composition = @Composition,
                                    UpdatedAt = NOW() 
                                WHERE 
                                    Id = @Id";
                    command.Parameters.AddWithValue("@Id", dish.Id);
                    command.Parameters.AddWithValue("@Title", dish.Title);
                    command.Parameters.AddWithValue("@Description", dish.Description);
                    command.Parameters.AddWithValue("@Composition", dish.Composition);
                    

                    command.ExecuteNonQuery();
                    connection.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                
            }
        }

        public void UpdateDishPrice(DishData dish)
        {
            using (var connection = GetConnection())
            using (var command = new MySqlCommand())
            {
                connection.Open();
                try
                {
                    command.Connection = connection;
                    command.CommandText = @"UPDATE activemenu 
                                SET 
                                    Price = @Price,                                   
                                    UpdatedAt = NOW() 
                                WHERE 
                                    Id = @Id";
                    command.Parameters.AddWithValue("@Id", dish.Id);
                    command.Parameters.AddWithValue("@Price", dish.Price);
                    
                    command.ExecuteNonQuery();
                    connection.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

            }
        }

        public void DeleteDish(DishData dish)
        {
            using (var connection = GetConnection())
            using (var command = new MySqlCommand())
            {
                connection.Open();

                // Временно отключаем проверку ограничения внешнего ключа в таблице deleted_dishes
                command.Connection = connection;
                command.CommandText = "SET foreign_key_checks = 0";
                command.ExecuteNonQuery();

                // Перемещение информации о блюде в таблицу deleted_dishes
                command.CommandText = @"INSERT INTO deleted_dishes (Id, CategoryId, Title, Description, Composition, CreatedAt, UpdatedAt, DeletedAt) 
                            SELECT Id, CategoryId, Title, Description, Composition, CreatedAt, UpdatedAt, NOW() as DeletedAt
                            FROM disharchive 
                            WHERE Id = @Id";
                command.Parameters.AddWithValue("@Id", dish.Id);
                command.ExecuteNonQuery();

                // Удаление блюда из disharchive
                command.CommandText = "DELETE FROM disharchive WHERE Id = @Id";
                command.ExecuteNonQuery();

                // Включаем проверку ограничения внешнего ключа обратно
                command.CommandText = "SET foreign_key_checks = 1";
                command.ExecuteNonQuery();

                connection.Close();
            }
        }

        public void DeleteDishMenu(DishData dish)
        {
            using (var connection = GetConnection())
            using (var command = new MySqlCommand())
            {
                connection.Open();

                // Временно отключаем проверку ограничения внешнего ключа в таблице deleted_dishes
                command.Connection = connection;
                command.CommandText = "SET foreign_key_checks = 0";
                command.ExecuteNonQuery();
              
                // Удаление блюда из disharchive
                command.CommandText = "DELETE FROM activemenu WHERE Id = @Id";
                command.Parameters.AddWithValue("@Id", dish.Id);
                command.ExecuteNonQuery();

                // Включаем проверку ограничения внешнего ключа обратно
                command.CommandText = "SET foreign_key_checks = 1";
                command.ExecuteNonQuery();

                connection.Close();
            }
        }
        public void TransferDishToActiveMenu(DishData dish)
        {
            using (var connection = GetConnection())
            using (var command = new MySqlCommand())
            {
                connection.Open();
                command.Connection = connection;

                command.CommandText = "SET foreign_key_checks = 0";
                command.ExecuteNonQuery();

                
                command.CommandText = @"INSERT INTO activemenu (DishId, BranchId, Price, TransferedAt)
                                VALUES (@Id, @branchId, 100, NOW())";

                command.Parameters.AddWithValue("@Id", dish.Id); 
                command.Parameters.AddWithValue("@branchId", UserData.BranchId); 

                command.ExecuteNonQuery();

                command.CommandText = "SET foreign_key_checks = 1";
                command.ExecuteNonQuery();

                connection.Close();
            }
        }

        public List<DishData> GetDishListByCategory(int CatId)
        {
            List<DishData> result = new List<DishData>();

            using(var connection = GetConnection())
            using (var command = new MySqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = $@"SELECT
                                        a.Title,
                                        a.Description,
                                        a.Composition,
                                        am.Price,
                                        am.Id
                                    FROM activemenu am
                                    INNER JOIN disharchive a ON am.DishId = a.Id
                                    WHERE am.BranchId = {UserData.BranchId}
                                        AND a.CategoryId = {CatId}";
                try
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add(new DishData
                            {
                                Title = reader[0].ToString(),
                                Description = reader[1].ToString(),
                                Composition = reader[2].ToString(),
                                Price = reader[3].ToString(),
                                Id = reader.GetInt32(4)
                            }) ;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
                connection.Close();
            }

            return result;
        }

        public int CreateNewOrder(List<DishData> dishList, int spot, int guestCount, float totalPrice)
        {
            long insertedId;
            using (var connection = GetConnection())
            using (var command = new MySqlCommand())
            { 
                connection.Open();
                command.Connection = connection;
                command.CommandText = $@"INSERT orders (EmployeeId, BranchId, TotalPrice, GuestCount, Spot, CreatedAt)
                                            VALUES
                                            ({UserData.Id}, {UserData.BranchId},
                                            {totalPrice}, {guestCount}, {spot}, NOW())";
                try
                {
                    command.ExecuteNonQuery();
                    insertedId = command.LastInsertedId;
                }
                catch (Exception)
                {
                    return 1;
                }
                foreach(DishData dish in dishList)
                {
                    command.CommandText = $@"INSERT orderdetails (DishId, OrderId, Quantity)
                                            VALUES
                                            ({dish.Id}, {insertedId}, {dish.Count})";
                    command.ExecuteNonQuery();
                }
                connection.Close();
                return 0;
            }
        }

        public List<ChequeModel> GetActiveOrders()
        {
            List<ChequeModel> result = new List<ChequeModel>();
            List<DishData> dishTemp = new List<DishData>();
            using (var connection = GetConnection())
            using (var command = new MySqlCommand())
            {

                connection.Open();
                command.Connection = connection;
                command.CommandText = $@"SELECT Id, TotalPrice, GuestCount, Spot, CreatedAt
                                        FROM orders WHERE BranchId={UserData.BranchId} AND IsReady=0";
                using(var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        dishTemp = new List<DishData>();
                        using (var connection2 = GetConnection())
                        using (var command2 = new MySqlCommand())
                        {
                            connection2.Open();
                            command2.Connection = connection2;
                            command2.CommandText = $@"SELECT 
                                                    a.Title,
                                                    od.Quantity,
                                                    od.Status,
                                                    od.Id
                                                    FROM orderdetails od
                                                    INNER JOIN activemenu am ON am.Id = od.DishId
                                                    INNER JOIN disharchive a ON a.Id = am.DishId
                                                    WHERE od.OrderId = {reader[0].ToString()}";
                            using (var reader2 = command2.ExecuteReader())
                            { 
                                while (reader2.Read())
                                {
                                    dishTemp.Add(new DishData
                                    {
                                        Title = reader2[0].ToString(),
                                        Count = reader2.GetInt32(1),
                                        Status = reader2.GetInt32(2),
                                        Id = reader2.GetInt32(3)
                                    });
                                }
                            }
                        }
                        result.Add(new ChequeModel
                        {
                            Id = reader.GetInt32(0),
                            TotalPrice = reader.GetFloat(1),
                            GuestNumber = reader.GetInt32(2),
                            SpotNumber = reader.GetInt32(3),
                            CreatedAt = reader.GetDateTime(4),
                            dishes = dishTemp
                        });
                    }
                }
            }
            return result;
        }

        public void UpdateStatus(DishData dish)
        {
            using (var connection = GetConnection())
            using (var command = new MySqlCommand())
            {
                connection.Open();
                try
                {
                    command.Connection = connection;
                    command.CommandText = $@"UPDATE orderdetails
                                SET 
                                    Status = {dish.Status}
                                WHERE
                                    Id = {dish.Id}";
                    command.ExecuteNonQuery();
                    connection.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

            }
        }

        public void DoOrderReady(ChequeModel cheque)
        {
            using (var connection = GetConnection())
            using (var command = new MySqlCommand())
            {
                connection.Open();
                try
                {
                    command.Connection = connection;
                    command.CommandText = $@"UPDATE orders
                                SET 
                                    IsReady = true
                                WHERE
                                    Id = {cheque.Id}";
                    command.ExecuteNonQuery();
                    connection.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

            }
        }
    }
}
