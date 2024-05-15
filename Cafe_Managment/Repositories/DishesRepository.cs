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
            MySqlConnection connection = null;

            try
            {
                connection = GetConnection(); // Получение соединения с базой данных
                using (var command = new MySqlCommand())
                {
                    connection.Open(); // Открытие соединения
                    command.Connection = connection;

                    command.CommandText = @"
                SELECT ROW_NUMBER() OVER() AS '№',
                      d.Id,
                      c.Title AS 'Раздел',
                      d.Title AS 'Название',
                      d.Description AS 'Описание', 
                      d.Composition AS 'Состав',
                      d.CreatedAt AS 'ДатаДобавления', 
                      d.UpdatedAt AS 'ДатаПоследнегоОбновления' 
                FROM disharchive d
                INNER JOIN categories c ON d.CategoryId = c.Id";

                    MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                    adapter.Fill(dataTable); // Заполнение DataTable
                }
            }
            catch (MySqlException sqlEx)
            {
                // Логирование или вывод сообщений при ошибках MySQL
                Console.WriteLine($"MySQL Error: {sqlEx.Message}");
                MessageBox.Show($"Ошибка базы данных: {sqlEx.Message}");
            }
            catch (Exception ex)
            {
                // Ловим все другие возможные исключения
                Console.WriteLine($"General Error: {ex.Message}");
                MessageBox.Show($"Произошла ошибка: {ex.Message}");
            }
            finally
            {
                // Закрываем соединение, даже если произошло исключение
                connection?.Close();
            }

            return dataTable;
        }

        public DataTable GetAllDeletedDishes()
        {
            DataTable dataTable = new DataTable();
            MySqlConnection connection = null;

            try
            {
                connection = GetConnection(); // Получение соединения
                using (var command = new MySqlCommand())
                {
                    connection.Open(); // Открытие соединения
                    command.Connection = connection;

                    command.CommandText = @"
                SELECT ROW_NUMBER() OVER() AS '№',
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
                    adapter.Fill(dataTable); // Заполнение DataTable
                }
            }
            catch (MySqlException sqlEx)
            {
                Console.WriteLine($"MySQL Error: {sqlEx.Message}");
                MessageBox.Show($"Ошибка базы данных: {sqlEx.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General Error: {ex.Message}");
                MessageBox.Show($"Произошла ошибка: {ex.Message}");
            }
            finally
            {
                // Закрываем соединение, если оно было открыто
                if (connection != null && connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }

            return dataTable;
        }

        public DataTable RestoreDeletedDish(DishData dish)
        {
            DataTable restoredData = new DataTable(); // Создаем новый DataTable для возвращаемых данных
            MySqlConnection connection = null;

            try
            {
                connection = GetConnection(); // Инициализируем соединение
                using (var command = new MySqlCommand())
                {
                    connection.Open(); // Открываем соединение

                    // Перемещение информации о блюде из deleted_dishes в disharchive
                    command.Connection = connection;
                    command.CommandText = @"
                INSERT INTO disharchive (Id, CategoryId, Title, Description, Composition, CreatedAt, UpdatedAt)
                SELECT Id, CategoryId, Title, Description, Composition, CreatedAt, UpdatedAt
                FROM deleted_dishes
                WHERE Id = @Id";
                    command.Parameters.AddWithValue("@Id", dish.Id);
                    command.ExecuteNonQuery(); // Выполняем вставку

                    // Удаление блюда из deleted_dishes
                    command.CommandText = "DELETE FROM deleted_dishes WHERE Id = @Id";
                    command.ExecuteNonQuery(); // Выполняем удаление

                    // Получение данных из disharchive после восстановления
                    command.CommandText = "SELECT * FROM disharchive WHERE Id = @Id";
                    using (var adapter = new MySqlDataAdapter(command))
                    {
                        adapter.Fill(restoredData); // Заполняем DataTable
                    }
                }
            }
            catch (MySqlException sqlEx)
            {
                Console.WriteLine($"MySQL Error: {sqlEx.Message}");
                MessageBox.Show($"Ошибка базы данных: {sqlEx.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General Error: {ex.Message}");
                MessageBox.Show($"Произошла ошибка: {ex.Message}");
            }
            finally
            {
                if (connection != null && connection.State == ConnectionState.Open)
                {
                    connection.Close(); // Гарантируем закрытие соединения
                }
            }

            return restoredData; // Возвращаем восстановленные данные
        }


        public DataTable GetAllDishesFromMenu()
        {
            DataTable dataTable = new DataTable();
            MySqlConnection connection = null;

            try
            {
                connection = GetConnection(); // Инициализация соединения
                using (var command = new MySqlCommand())
                {
                    connection.Open(); // Открытие соединения
                    command.Connection = connection;

                    // Команда SQL для получения данных
                    command.CommandText = $@"
                SELECT ROW_NUMBER() OVER() AS '№',
                am.Id,
                c.Title AS 'Раздел', 
                da.Title AS 'Название',
                da.Description AS 'Описание', 
                da.Composition AS 'Состав',
                am.Price AS 'Стоимость',                                     
                am.TransferedAt AS 'ДатаДобавления',
                am.UpdatedAt AS 'ДатаОбновленияЦены'
                FROM activemenu am
                INNER JOIN disharchive da ON am.DishId = da.Id
                INNER JOIN categories c ON da.CategoryId = c.Id
                WHERE am.BranchId = {UserData.BranchId}";

                    MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                    adapter.Fill(dataTable); // Заполнение DataTable
                }
            }
            catch (MySqlException sqlEx)
            {
                // Обработка ошибок MySQL
                Console.WriteLine($"MySQL Error: {sqlEx.Message}");
                MessageBox.Show($"Ошибка базы данных: {sqlEx.Message}");
            }
            catch (Exception ex)
            {
                // Обработка общих ошибок
                Console.WriteLine($"General Error: {ex.Message}");
                MessageBox.Show($"Произошла ошибка: {ex.Message}");
            }
            finally
            {
                if (connection != null && connection.State == ConnectionState.Open)
                {
                    connection.Close(); // Гарантируем закрытие соединения
                }
            }

            return dataTable; // Возвращаем заполненный DataTable
        }

        public void UpdateDish(DishData dish)
        {
            MySqlConnection connection = null;

            try
            {
                connection = GetConnection(); // Инициализация соединения
                using (var command = new MySqlCommand())
                {
                    connection.Open(); // Открытие соединения
                    command.Connection = connection;

                    // Команда SQL для обновления данных
                    command.CommandText = @"
                UPDATE disharchive 
                SET 
                    Title = @Title,
                    Description = @Description,
                    Composition = @Composition,
                    UpdatedAt = NOW() 
                WHERE 
                    Id = @Id";

                    // Заполнение параметров
                    command.Parameters.AddWithValue("@Id", dish.Id);
                    command.Parameters.AddWithValue("@Title", dish.Title);
                    command.Parameters.AddWithValue("@Description", dish.Description ?? string.Empty); // Обработка null
                    command.Parameters.AddWithValue("@Composition", dish.Composition ?? string.Empty); // Обработка null

                    command.ExecuteNonQuery(); // Выполнение команды SQL
                }
            }
            catch (MySqlException sqlEx)
            {
                // Обработка ошибок MySQL
                MessageBox.Show($"Ошибка базы данных: {sqlEx.Message}");
            }
            catch (Exception ex)
            {
                // Общая обработка исключений
                MessageBox.Show($"Произошла ошибка: {ex.Message}");
            }
            finally
            {
                // Гарантируем закрытие соединения
                if (connection != null && connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }


        public void UpdateDishPrice(DishData dish)
        {
            MySqlConnection connection = null;

            try
            {
                connection = GetConnection();
                connection.Open();

                using (var command = new MySqlCommand())
                {
                    command.Connection = connection;

                    command.CommandText = @"
                UPDATE activemenu 
                SET 
                    Price = @Price,
                    UpdatedAt = NOW() 
                WHERE 
                    Id = @Id";

                    // Добавление параметров в команду
                    command.Parameters.AddWithValue("@Id", dish.Id);

                    if (decimal.TryParse(dish.Price, out decimal price))
                    {
                        command.Parameters.AddWithValue("@Price", price);
                    }
                    else
                    {
                        throw new InvalidOperationException("Цена должна быть числом.");
                    }

                    command.ExecuteNonQuery(); // Выполнение команды
                }
            }
            catch (MySqlException sqlEx)
            {
                // Обработка специфичных ошибок MySQL
                MessageBox.Show($"Ошибка базы данных: {sqlEx.Message}");
            }
            catch (InvalidOperationException ioEx)
            {
                // Обработка ошибки преобразования
                MessageBox.Show($"Ошибка преобразования цены: {ioEx.Message}");
            }
            catch (Exception ex)
            {
                // Общая обработка ошибок
                MessageBox.Show($"Произошла ошибка: {ex.Message}");
            }
            finally
            {
                // Гарантируем закрытие соединения
                if (connection != null && connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }


        public void DeleteDish(DishData dish)
        {
            MySqlConnection connection = null;

            try
            {
                connection = GetConnection();
                connection.Open();

                using (var command = new MySqlCommand())
                {
                    command.Connection = connection;

                    // Отключение проверки ограничений внешнего ключа
                    command.CommandText = "SET foreign_key_checks = 0";
                    command.ExecuteNonQuery();

                    // Перемещение блюда в таблицу удаленных блюд
                    command.CommandText = @"
                INSERT INTO deleted_dishes 
                    (Id, CategoryId, Title, Description, Composition, CreatedAt, UpdatedAt, DeletedAt)
                SELECT 
                    Id, CategoryId, Title, Description, Composition, CreatedAt, UpdatedAt, NOW() 
                FROM disharchive 
                WHERE Id = @Id";

                    command.Parameters.AddWithValue("@Id", dish.Id);
                    command.ExecuteNonQuery();

                    // Удаление блюда из основной таблицы
                    command.CommandText = "DELETE FROM disharchive WHERE Id = @Id";
                    command.ExecuteNonQuery();

                    // Включение проверки ограничений внешнего ключа
                    command.CommandText = "SET foreign_key_checks = 1";
                    command.ExecuteNonQuery();
                }
            }
            catch (MySqlException sqlEx)
            {
                // Обработка ошибок MySQL
                MessageBox.Show($"Ошибка базы данных: {sqlEx.Message}");
            }
            catch (Exception ex)
            {
                // Обработка всех остальных ошибок
                MessageBox.Show($"Произошла ошибка: {ex.Message}");
            }
            finally
            {
                // Гарантированное закрытие соединения
                if (connection != null && connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }


        public void DeleteDishMenu(DishData dish)
        {
            MySqlConnection connection = null;

            try
            {
                connection = GetConnection();
                connection.Open();

                using (var command = new MySqlCommand())
                {
                    command.Connection = connection;

                    // Отключение проверки ограничений внешнего ключа
                    command.CommandText = "SET foreign_key_checks = 0";
                    command.ExecuteNonQuery();

                    // Удаление блюда из активного меню
                    command.CommandText = "DELETE FROM activemenu WHERE Id = @Id";
                    command.Parameters.AddWithValue("@Id", dish.Id);
                    command.ExecuteNonQuery();

                    // Включение проверки ограничений внешнего ключа
                    command.CommandText = "SET foreign_key_checks = 1";
                    command.ExecuteNonQuery();
                }
            }
            catch (MySqlException sqlEx)
            {
                // Обработка ошибок MySQL
                MessageBox.Show($"Ошибка базы данных: {sqlEx.Message}");
            }
            catch (Exception ex)
            {
                // Обработка всех остальных ошибок
                MessageBox.Show($"Произошла ошибка: {ex.Message}");
            }
            finally
            {
                // Гарантированное закрытие соединения
                if (connection != null && connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }

        public void TransferDishToActiveMenu(DishData dish)
        {
            MySqlConnection connection = null;

            try
            {
                connection = GetConnection();
                connection.Open();

                using (var command = new MySqlCommand())
                {
                    command.Connection = connection;

                    // Проверка, не существует ли уже блюдо в активном меню
                    command.CommandText = @"SELECT Id 
                                    FROM activemenu
                                    WHERE BranchId = @BranchId AND DishId = @DishId";

                    command.Parameters.AddWithValue("@DishId", dish.Id);
                    command.Parameters.AddWithValue("@BranchId", UserData.BranchId);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            // Если блюдо уже в меню, предупреждаем пользователя
                            MessageBox.Show("Это блюдо уже есть в меню.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Information);
                            return; // Прерываем выполнение метода
                        }
                    }

                    // Отключаем проверку внешних ключей
                    command.CommandText = "SET foreign_key_checks = 0";
                    command.ExecuteNonQuery();

                    // Добавляем блюдо в активное меню
                    command.CommandText = @"INSERT INTO activemenu (DishId, BranchId, Price, TransferedAt)
                                    VALUES (@DishId, @BranchId, @Price, NOW())";
                    command.Parameters.AddWithValue("@Price", dish.Price);

                    command.ExecuteNonQuery();

                    // Включаем проверку внешних ключей
                    command.CommandText = "SET foreign_key_checks = 1";
                    command.ExecuteNonQuery();
                }
            }
            catch (MySqlException sqlEx)
            {
                MessageBox.Show($"Ошибка базы данных: {sqlEx.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                if (connection != null && connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }

        public bool CheckDishInMenu(int dishId, int branchId)
        {
            MySqlConnection connection = null;

            try
            {
                connection = GetConnection();
                connection.Open();

                using (var command = new MySqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = @"SELECT Id FROM activemenu WHERE BranchId = @BranchId AND DishId = @DishId";
                    command.Parameters.AddWithValue("@BranchId", branchId);
                    command.Parameters.AddWithValue("@DishId", dishId);

                    using (var reader = command.ExecuteReader())
                    {
                        return reader.HasRows; // Если есть строки, блюдо уже в меню
                    }
                }
            }
            catch (MySqlException sqlEx)
            {
                // Обработка исключений MySQL
                MessageBox.Show($"Ошибка базы данных: {sqlEx.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                // Общая обработка исключений
                MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                if (connection != null && connection.State == ConnectionState.Open)
                {
                    connection.Close(); // Гарантированное закрытие соединения
                }
            }

            return false; // Если что-то пошло не так, предполагаем, что блюда в меню нет
        }

        public List<DishData> GetDishListByCategory(int CatId)
        {
            List<DishData> result = new List<DishData>();

            MySqlConnection connection = null;
            try
            {
                connection = GetConnection();
                connection.Open();

                using (var command = new MySqlCommand())
                {
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
                                        AND a.CategoryId = @CatId";

                    command.Parameters.AddWithValue("@CatId", CatId);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add(new DishData
                            {
                                Title = reader.GetString(0),
                                Description = reader.GetString(1),
                                Composition = reader.GetString(2),
                                Price = reader.GetDecimal(3).ToString("F2"), // Чтобы обеспечить форматирование цены
                                Id = reader.GetInt32(4)
                            });
                        }
                    }
                }
            }
            catch (MySqlException sqlEx)
            {
                // Обработка ошибок MySQL
                Debug.WriteLine($"Ошибка базы данных: {sqlEx.Message}");
                MessageBox.Show($"Произошла ошибка в базе данных: {sqlEx.Message}");
            }
            catch (Exception ex)
            {
                // Общая обработка ошибок
                Debug.WriteLine($"Произошла ошибка: {ex.Message}");
                MessageBox.Show($"Произошла ошибка: {ex.Message}");
            }
            finally
            {
                // Гарантированное закрытие соединения
                if (connection != null && connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }

            return result;
        }

        public int CreateNewOrder(List<DishData> dishList, int spot, int guestCount, float totalPrice)
        {
            long insertedId = -1; // Инициализация переменной для хранения ID нового заказа
            MySqlConnection connection = null;

            try
            {
                connection = GetConnection();
                connection.Open();

                using (var command = new MySqlCommand())
                {
                    command.Connection = connection;

                    // Использование параметров для защиты от SQL-инъекций
                    command.CommandText = @"INSERT INTO orders (EmployeeId, BranchId, TotalPrice, GuestCount, Spot, CreatedAt)
                                    VALUES (@EmployeeId, @BranchId, @TotalPrice, @GuestCount, @Spot, NOW())";

                    command.Parameters.AddWithValue("@EmployeeId", UserData.Id);
                    command.Parameters.AddWithValue("@BranchId", UserData.BranchId);
                    command.Parameters.AddWithValue("@TotalPrice", totalPrice);
                    command.Parameters.AddWithValue("@GuestCount", guestCount);
                    command.Parameters.AddWithValue("@Spot", spot);

                    command.ExecuteNonQuery();
                    insertedId = command.LastInsertedId; // Получение ID нового заказа
                }

                // Вставка деталей заказа
                using (var command = new MySqlCommand())
                {
                    command.Connection = connection;

                    foreach (DishData dish in dishList)
                    {
                        command.CommandText = @"INSERT INTO orderdetails (DishId, OrderId, Quantity)
                                       VALUES (@DishId, @OrderId, @Quantity)";

                        command.Parameters.Clear(); // Удаление предыдущих параметров
                        command.Parameters.AddWithValue("@DishId", dish.Id);
                        command.Parameters.AddWithValue("@OrderId", insertedId);
                        command.Parameters.AddWithValue("@Quantity", dish.Count);

                        command.ExecuteNonQuery();
                    }
                }

                return 0; // Возвращение кода успеха
            }
            catch (MySqlException ex)
            {
                // Обработка ошибок MySQL
                MessageBox.Show($"Произошла ошибка при выполнении SQL-запроса: {ex.Message}");
                return -1; // Возвращение кода ошибки
            }
            catch (Exception ex)
            {
                // Общая обработка ошибок
                MessageBox.Show($"Произошла непредвиденная ошибка: {ex.Message}");
                return -1;
            }
            finally
            {
                if (connection != null && connection.State == ConnectionState.Open)
                {
                    connection.Close(); // Гарантированное закрытие соединения
                }
            }
        }
        public List<ChequeModel> GetActiveOrders()
        {
            List<ChequeModel> result = new List<ChequeModel>();

            MySqlConnection connection1 = null;
            MySqlConnection connection2 = null;

            try
            {
                connection1 = GetConnection();
                connection1.Open();

                using (var command1 = new MySqlCommand())
                {
                    command1.Connection = connection1;
                    command1.CommandText = @"SELECT Id, TotalPrice, GuestCount, Spot, CreatedAt
                                    FROM orders 
                                    WHERE BranchId = @BranchId AND IsReady = 0";
                    command1.Parameters.AddWithValue("@BranchId", UserData.BranchId);

                    using (var reader1 = command1.ExecuteReader())
                    {
                        while (reader1.Read())
                        {
                            var orderId = reader1.GetInt32(0);

                            List<DishData> dishTemp = new List<DishData>();

                            try
                            {
                                connection2 = GetConnection();
                                connection2.Open();

                                using (var command2 = new MySqlCommand())
                                {
                                    command2.Connection = connection2;
                                    command2.CommandText = @"SELECT 
                                                        a.Title,
                                                        od.Quantity,
                                                        od.Status,
                                                        od.Id
                                                      FROM orderdetails od
                                                      INNER JOIN activemenu am ON am.Id = od.DishId
                                                      INNER JOIN disharchive a ON a.Id = am.DishId
                                                      WHERE od.OrderId = @OrderId";

                                    command2.Parameters.AddWithValue("@OrderId", orderId);

                                    using (var reader2 = command2.ExecuteReader())
                                    {
                                        while (reader2.Read())
                                        {
                                            dishTemp.Add(new DishData
                                            {
                                                Title = reader2.GetString(0),
                                                Count = reader2.GetInt32(1),
                                                Status = reader2.GetInt32(2),
                                                Id = reader2.GetInt32(3)
                                            });
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show($"Ошибка при получении деталей заказа: {ex.Message}");
                            }
                            finally
                            {
                                if (connection2 != null && connection2.State == ConnectionState.Open)
                                {
                                    connection2.Close();
                                }
                            }

                            result.Add(new ChequeModel
                            {
                                Id = orderId,
                                TotalPrice = reader1.GetFloat(1),
                                GuestNumber = reader1.GetInt32(2),
                                SpotNumber = reader1.GetInt32(3),
                                CreatedAt = reader1.GetDateTime(4),
                                dishes = dishTemp
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при получении активных заказов: {ex.Message}");
            }
            finally
            {
                if (connection1 != null && connection1.State == ConnectionState.Open)
                {
                    connection1.Close();
                }
            }

            return result;
        }

        public void UpdateStatus(DishData dish)
        {
            MySqlConnection connection = null;

            try
            {
                connection = GetConnection();
                connection.Open();

                using (var command = new MySqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = @"UPDATE orderdetails
                                    SET 
                                        Status = @Status
                                    WHERE
                                        Id = @Id";

                    command.Parameters.AddWithValue("@Status", dish.Status);
                    command.Parameters.AddWithValue("@Id", dish.Id);

                    command.ExecuteNonQuery();
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"Ошибка при обновлении статуса заказа: {ex.Message}");
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
        }

        public void IsOrderReady(ChequeModel cheque)
        {
            MySqlConnection connection = null;

            try
            {
                connection = GetConnection();
                connection.Open();

                using (var command = new MySqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = @"UPDATE orders
                                   SET 
                                       IsReady = true
                                   WHERE
                                       Id = @Id";

                    command.Parameters.AddWithValue("@Id", cheque.Id);

                    command.ExecuteNonQuery();
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"Ошибка при обновлении статуса заказа: {ex.Message}");
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
        }

        public List<string> GetAllCategories()
        {
            List<string> categories = new List<string>();
            MySqlConnection connection = GetConnection();
            connection.Open();
            using(var command = new MySqlCommand())
            {
                command.Connection = connection;
                command.CommandText = "SELECT Title FROM categories";
                using(var reader = command.ExecuteReader())
                {
                    if(reader.Read())
                    {
                        foreach(var item in reader)
                        {
                            categories.Add(item.ToString());
                        }
                    }
                }
            }
            return categories;
        }
    }
}
