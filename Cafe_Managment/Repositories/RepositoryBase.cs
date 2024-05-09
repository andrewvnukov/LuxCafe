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
            //_connectionString = "server=sql11.freemysqlhosting.net;user=sql11688980;password=r5x3eqwMhE;database=sql11688980;charset=utf8;";
            _connectionString = "server=localhost;user=root;password=123456Aa;database=cafe_managment;charset=utf8;";
        }

        protected MySqlConnection GetConnection()
        {
            return new MySqlConnection(_connectionString);

        }
    }
}
