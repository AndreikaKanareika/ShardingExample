using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;

namespace ConsoleApp6
{
    public static class DapperMultiSqlExtension
    {
        public static async Task<IEnumerable<T>> QueryAsync<T>(this MultiSqlConnection multiSqlConnection,
            string sql,
            object param = null,
            int? commandTimeout = null,
            CommandType? commandType = null)
        {
            var tasks = multiSqlConnection.SqlConnections
                .Select(c => c.QueryAsync<T>(sql, param, null, commandTimeout, commandType));

            var values = await Task.WhenAll(tasks);
            
            return values.SelectMany(x => x);
        }
    }
}