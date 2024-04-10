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
    internal class DeletedDataRepository : RepositoryBase, IDeletedDataRepository
    {

        public DataTable GetDismissedEmployees()
        {
            DataTable dataTable = new DataTable();

            using (var connection = GetConnection())
            using (var command = new MySqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = @"SELECT ROW_NUMBER() OVER() AS '№',
                                        
                                        c.Address AS 'Филиал', r.Title AS 'Роль', 
                                        e.Name AS 'Имя', e.Surname AS 'Фамилия', 
                                        e.Patronomic AS 'Отчество', e.PhoneNumber AS 'Номер телефона', 
                                        e.Email AS 'Почта', DATE_FORMAT(e.BirthDay, '%d-%m-%Y') AS 'Дата рождения', 
                                        e.Address AS 'Адрес' 
                                FROM dismissed_employees e 
                                INNER JOIN Roles r ON e.RoleId = r.Id
                                INNER JOIN Branches c ON e.BranchId = c.Id";

                MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                adapter.Fill(dataTable);

                connection.Close();
            }

            return dataTable;
        }

        public DataTable GetAllDeletedDishes()
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
                                      d.UpdatedAt AS 'Дата последнего обновления',
                                      d.DeletedAt AS 'Дата удаления' 
                                FROM deleted_dishes d
                                INNER JOIN categories c ON d.CategoryId = c.Id";

                MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                adapter.Fill(dataTable);

                connection.Close();
            }

            return dataTable;
        }

        public DataTable RestoreDeletedDish(DishData dish)
        {
            DataTable restoredData = new DataTable(); // Создаем новый DataTable для возвращаемых данных

            using (var connection = GetConnection())
            using (var command = new MySqlCommand())
            {
                connection.Open();

                // Перемещение информации о блюде из deleted_dishes в disharchive
                command.Connection = connection;
                command.CommandText = @"INSERT INTO disharchive (Id, CategoryId, Title, Description, Composition, CreatedAt, UpdatedAt)
                                SELECT Id, CategoryId, Title, Description, Composition, CreatedAt, UpdatedAt
                                FROM deleted_dishes
                                WHERE Id = @Id";
                command.Parameters.AddWithValue("@Id", dish.Id);
                command.ExecuteNonQuery();

                // Удаление блюда из deleted_dishes
                command.CommandText = "DELETE FROM deleted_dishes WHERE Id = @Id";
                command.ExecuteNonQuery();

                // Получение данных из disharchive
                command.CommandText = "SELECT * FROM disharchive WHERE Id = @Id";
                using (var adapter = new MySqlDataAdapter(command))
                {
                    adapter.Fill(restoredData);
                }

                connection.Close();
            }

            return restoredData; // Возвращаем восстановленные данные
        }



    }

}
