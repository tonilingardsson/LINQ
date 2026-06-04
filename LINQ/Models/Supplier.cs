using System.Collections.Generic;

namespace LINQ.Models
{
    public class Supplier
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!; 
        public string ContactPerson { get; set; } = null!;  
        public string Email { get; set; } = null!;
        public string Phone { get; set; } = null!;

        public List<Product> Products { get; set; } = new List<Product>();
    }
}