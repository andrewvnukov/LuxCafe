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
            //_connectionString = "server=localhost;user=root;password=123456Aa;database=cafe_managment;charset=utf8;";
            //_connectionString = "server=sql7.freemysqlhosting.net;user=sql7711327;password=6JalC7anb6;database=sql7711327;charset=utf8;";//freemysqlhosting.net
            //_connectionString = "server=w19.h.filess.io;user=Staffee_insideknew;password=833ab79fee6354074845f4460adc69318e823d69;database=Staffee_insideknew;charset=utf8mb4;";//рабочий
            //_connectionString = "server=3sr.h.filess.io;user=Staffee2_mynewknown;password=c3bfafcaf92801f754debc59720fb628ff6d09e5;database=Staffee2_mynewknown;charset=utf8;";
            _connectionString = "server=sql7.freesqldatabase.com;user=sql7714281;password=YfLxfGaaLa;database=sql7714281;charset=utf8;";//Freesqldatabase.com
        }

        protected MySqlConnection GetConnection()
        {
            return new MySqlConnection(_connectionString);

        }
    }
}
