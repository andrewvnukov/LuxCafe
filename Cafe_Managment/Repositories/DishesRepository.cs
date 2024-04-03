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
                                      am.TransferedAt AS 'Дата добавления' 
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
        public void DeleteDish(DishData dish)
        {
            using (var connection = GetConnection())
            using (var command = new MySqlCommand())
            {
                connection.Open();

                // Удаление всех связанных строк в activemenu
                command.Connection = connection;
                command.CommandText = "DELETE FROM activemenu WHERE DishId = @DishId";
                command.Parameters.AddWithValue("@DishId", dish.Id);
                command.ExecuteNonQuery();

                // Удаление блюда из disharchive
                command.CommandText = "DELETE FROM disharchive WHERE Id = @Id";
                command.Parameters.Clear(); // Очистка параметров перед добавлением новых
                command.Parameters.AddWithValue("@Id", dish.Id);
                command.ExecuteNonQuery();

                connection.Close();
            }
        }

        public void MoveDishToActiveMenu(DishData dish)
        {
            using (var connection = GetConnection())
            using (var command = new MySqlCommand())
            {
                connection.Open();

                command.Connection = connection;
                command.CommandText = @"INSERT INTO activemenu (CategoryId, Title, Description, Composition, CreatedAt, UpdatedAt, BranchId, Price) 
                                SELECT CategoryId, Title, Description, Composition, CreatedAt, UpdatedAt, BranchId, Price
                                FROM disharchive 
                                WHERE Id = @Id";

                command.Parameters.AddWithValue("@Id", dish.Id);    

                command.ExecuteNonQuery();

                connection.Close();
            }
        }


    }
}
