using Microsoft.EntityFrameworkCore;

namespace LINQ
{
    public class MyDbContext : DbContext
    {
        // Expose DbSet<Product>, DbSet<Category>, DbSet<Customer>, DbSet<Order>, DbSet<OrderDetail>
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        //public DbSet<Customer> Customers { get; set; }
        //public DbSet<Order> Orders { get; set; }
        //public DbSet<OrderDetail> OrderDetails { get; set; }

        // Configure SQLite connection in OnConfiguring, for example with a local .db file
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=mydatabase.db");
        }

    }
}
