using System.Data.SqlClient;

namespace ConsoleApp6
{
    public class Shard
    {
        public int Id { get; set; }
        public string ConnectionString { get; set; }

        public SqlConnection GetSqlConnection()
        {
            return new SqlConnection(ConnectionString);
        }
    }
}