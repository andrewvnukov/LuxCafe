using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;

namespace Cafe_Managment.Repositories
{
    public abstract class RepositoryBase
    {
        private readonly string _connectionString;

        public RepositoryBase()
        {
            _connectionString = "server=localhost;user=root;password=1234;database=cafemanagement;";
        }

        protected MySqlConnection GetConnection()
        {
            return new MySqlConnection(_connectionString);
        }
    }
}
