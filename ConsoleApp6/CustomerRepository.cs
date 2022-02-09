using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;

namespace ConsoleApp6
{
    public class CustomerRepository
    {
        private readonly CustomerShardingStrategy _shardingStrategy;
        private readonly IdGenerator _idGenerator;
        
        public CustomerRepository(CustomerShardingStrategy shardingStrategy, IdGenerator idGenerator)
        {
            _shardingStrategy = shardingStrategy;
            _idGenerator = idGenerator;
        }
        
        public async Task AddAsync(Customer customer)
        {
            customer.Id = await _idGenerator.GetIdAsync("CustomerId");
            
            await using var conn = _shardingStrategy.GetSqlConnectionByKey(customer.Id);
            
            await conn.ExecuteAsync(
                "INSERT INTO Customers (Id, FirstName, LastName) VALUES (@Id, @FirstName, @LastName)", 
                new {customer.Id, customer.FirstName, customer.LastName});
        }

        public async Task<Customer> GetByIdAsync(int id)
        {
            await using var conn = _shardingStrategy.GetSqlConnectionByKey(id);

            return await conn.QueryFirstOrDefaultAsync<Customer>("SELECT * FROM Customers WHERE Id = @id", new {id});
        }

        public async Task<IEnumerable<Customer>> GetAllAsync()
        {
            using var multiSqlConnection = _shardingStrategy.GetAllSqlConnections();

            return await multiSqlConnection.QueryAsync<Customer>("SELECT * FROM Customers");
        }

        public async Task UpdateAsync(Customer customer)
        {
            await using var conn = _shardingStrategy.GetSqlConnectionByKey(customer.Id);

            await conn.ExecuteAsync(
                "UPDATE Customers SET FirstName = @FirstName, LastName = @LastName WHERE Id = @Id",
                new {customer.Id, customer.FirstName, customer.LastName});
        }
        
        public async Task DeleteAsync(int id)
        {
            await using var conn = _shardingStrategy.GetSqlConnectionByKey(id);

            await conn.ExecuteAsync("DELETE FROM Customers WHERE Id = @id", new { id });
        }
    }
}