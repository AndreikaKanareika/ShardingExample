using System;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace ConsoleApp6
{
    public class IdGenerator: IDisposable
    {
        private readonly ConnectionMultiplexer _multiplexer;
        private readonly IDatabase _database;
        
        public IdGenerator(string connection)
        {
            _multiplexer = ConnectionMultiplexer.Connect(connection);
            _database = _multiplexer.GetDatabase();
        }
        
        public async Task<int> GetIdAsync(string key)
        {
            var id = await _database.StringIncrementAsync(key);
            return (int)id;
        }
        
        public async Task ResetId(string key)
        {
            await _database.StringSetAsync(key, 0);
        }

        public void Dispose()
        {
            _multiplexer.Dispose();
        }
    }
}