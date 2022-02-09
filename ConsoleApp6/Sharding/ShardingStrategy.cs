using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace ConsoleApp6
{
    public class CustomerShardingStrategy : ShardingStrategy<int>
    {
        public CustomerShardingStrategy(List<Shard> shards) : base(shards)
        {
        }
    }
    
    
    public abstract class ShardingStrategy<T>
    {
        private List<Shard> _shards;
        
        protected ShardingStrategy(List<Shard> shards)
        {
            _shards = shards;
        }

        public SqlConnection GetSqlConnectionByKey(T key)
        {
            var shardId = GetShardId(key);
            var shard = _shards.Single(x => x.Id == shardId);
            return shard.GetSqlConnection();
        }
        
        public MultiSqlConnection GetAllSqlConnections()
        {
            return new MultiSqlConnection(_shards.Select(x => x.ConnectionString));
        }

        public virtual int GetShardId(T key)
        {
            return ((key.GetHashCode() & 0xfffffff) % _shards.Count) + 1;
        }
    }
}