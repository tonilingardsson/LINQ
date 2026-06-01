using System.Collections.Generic;

namespace LINQ
{
    public class Supplier
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ContactPerson { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

        // One supplier can have many products
        public List<Product> Products { get; set; } = new List<Product>();
    }
}