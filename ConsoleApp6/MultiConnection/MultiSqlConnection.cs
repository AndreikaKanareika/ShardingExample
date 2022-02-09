using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace ConsoleApp6
{
    public class MultiSqlConnection : IDisposable
    {
        public IEnumerable<SqlConnection> SqlConnections { get; set; }
        
        public MultiSqlConnection(IEnumerable<string> connectionStrings)
        {
            SqlConnections = connectionStrings.Select(x => new SqlConnection(x));
        }
        
        public void Dispose()
        {
            foreach (var conn in SqlConnections)
            {
                conn.Dispose();
            }
        }
    }
}