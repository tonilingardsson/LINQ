using System.Collections.Generic;

namespace LINQ
{
    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int Age { get; set; }
        public string Email { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string Address { get; set; } = null!;

        public List<Order> Orders { get; set; } = new();
    }
}
