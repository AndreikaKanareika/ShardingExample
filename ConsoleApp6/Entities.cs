using System;

namespace ConsoleApp6
{
    public class Customer
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class Order
    {
        public int CustomerId { get; set; }
        public int ProductId { get; set; }
    }
}