using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;

namespace ConsoleApp6
{
    class Program
    {
        private static readonly List<Shard> _shards = new()
        {
            new()
            {
                Id = 1,
                ConnectionString = @"Data Source=DESKTOP-NT000I8\SQLEXPRESS;Initial Catalog=MyDB_Shard1;Integrated Security=True"
            },
            new()
            {
                Id = 2,
                ConnectionString = @"Data Source=DESKTOP-NT000I8\SQLEXPRESS;Initial Catalog=MyDB_Shard2;Integrated Security=True"
            }
        };
        
        static async Task Main(string[] args)
        {
            await CreateSchema();

            var idGenerator = new IdGenerator("localhost");
            var customerShardingStrategy = new CustomerShardingStrategy(_shards);
            var repo = new CustomerRepository(customerShardingStrategy, idGenerator);

            var cust1 = new Customer { FirstName = "Alex", LastName = "Smith" };
            var cust2 = new Customer { FirstName = "John", LastName = "Snow" };
            var cust3 = new Customer { FirstName = "Jesus", LastName = "Riapolov" };
            
            try
            {
                ShowCustomers(new [] { cust1, cust2, cust3 });
                Console.ReadLine();

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Insert customer {cust1.FirstName} {cust1.LastName}");
                await repo.AddAsync(cust1);
                await ShowShardsDataAsync();
                Console.ReadLine();

                
                
                Console.WriteLine($"Insert customer {cust2.FirstName} {cust2.LastName}");
                await repo.AddAsync(cust2);
                await ShowShardsDataAsync();
                Console.ReadLine();

                
                
                Console.WriteLine($"Insert customer {cust3.FirstName} {cust3.LastName}");
                await repo.AddAsync(cust3);
                await ShowShardsDataAsync();
                Console.ReadLine();

                
                
                Console.WriteLine($"Update customer {cust2.Id} first name");
                cust2.FirstName = "newName";
                await repo.UpdateAsync(cust2);

                var cust = await repo.GetByIdAsync(cust2.Id);
                Console.WriteLine($"{cust.Id} | {cust.FirstName} {cust.LastName}");
                Console.ReadLine();

                
                
                var customers = await repo.GetAllAsync();
                ShowCustomers(customers);
                Console.ReadLine();

                
                
                Console.WriteLine($"Delete customer {cust.Id}");
                await repo.DeleteAsync(cust.Id);
                await ShowShardsDataAsync();
                Console.ReadLine();
            }
            finally
            {
                await ClearData();
            }
        }

        private static async Task ShowShardsDataAsync()
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("________________________________________________________");

            foreach (var shard in _shards)
            {
                Console.WriteLine($"\nShard {shard.Id} ({shard.ConnectionString})");
                
                await using var conn = shard.GetSqlConnection();
                var customers = await conn.QueryAsync<Customer>("SELECT * FROM Customers");
                
                ShowCustomers(customers);
            }
            
            Console.WriteLine("________________________________________________________\n");
            Console.ForegroundColor = ConsoleColor.Red;
        }

        private static void ShowCustomers(IEnumerable<Customer> customers)
        {
            foreach (var x in customers)
            {
                Console.WriteLine($"{x.Id} | {x.FirstName} {x.LastName}");
            }
        }

        private static async Task CreateSchema()
        {
            foreach (var shard in _shards)
            {
                await using var conn = shard.GetSqlConnection();

                await conn.ExecuteAsync(
                    @"IF (OBJECT_ID('[dbo].[Customers]', 'U') IS NULL)
                    CREATE TABLE [dbo].[Customers](
	                    [Id] int PRIMARY KEY,
	                    [FirstName] NVARCHAR(50) NULL,
	                    [LastName] NVARCHAR(50) NULL,
                    )");
            }
        }

        private static async Task ClearData()
        {
            foreach (var shard in _shards)
            {
                await using var conn = shard.GetSqlConnection();

                await conn.ExecuteAsync("DELETE FROM Customers");
            }
        }
    }
}