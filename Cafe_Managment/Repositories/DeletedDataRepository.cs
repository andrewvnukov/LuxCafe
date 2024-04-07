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
                                        e.Id,
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

    }
 
}
