using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace TodoWebApi.Data
{
    public static class DbHelper
    {
        private static readonly string _connStr = ConfigurationManager.ConnectionStrings["TodoDB"].ConnectionString;

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(_connStr);
        }
    }
}