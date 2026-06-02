using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LINQ.Models
{
    internal class Products
    {
        public static void Electronics()
        {
            var customers = new List<Customer>
            {
                // Preparing for LINQ queries by creating a list of customers with their orders.
                // using (var context = new Products())
                // {
                // var customers = context.Customers.ToList();
                // LINQ queries will be performed on this list to demonstrate various operations.
                // }

                new Customer { Name = "Alice", Age = 30, Orders = new List<int> { 100, 200 } },
                new Customer { Name = "Bob", Age = 25, Orders = new List<int> { 150 } },
                new Customer { Name = "Charlie", Age = 35, Orders = new List<int> { 300, 400, 500 } }
            };
        }
    }

    public class Customer
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public List<int> Orders { get; set; }
    }
}
