using LINQ;
using Microsoft.EntityFrameworkCore;

namespace LINQ
{
    internal class Program
    {
        static void Main(string[] args)
        {
            SQLitePCL.Batteries.Init();
            using var context = new MyDbContext();

            context.Database.EnsureCreated();

            SeedData(context);

            bool running = true;

            while (running)
            {
                Console.Clear();
                Console.WriteLine("==== LINQ Menu ====");
                Console.WriteLine("1. Show Electronic products sorted by price");
                Console.WriteLine("0. Exit");
                Console.Write("Choose an option: ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        Console.Clear();
                        ShowElectronics(context);
                        Pause();
                        break;

                    case "2":
                        Console.Clear();
                        ShowSuppliersWithLowStockProducts(context);
                        Pause();
                        break;

                    case "0":
                        running = false;
                        break;

                    default:
                        Console.WriteLine("Invalid choice.");
                        Pause();
                        break;
                }
            }
        }


        public static void ShowElectronics(MyDbContext context)
        {
            try
            {
                Console.WriteLine($"Categories count: {context.Categories.Count()}");
                Console.WriteLine($"Products count: {context.Products.Count()}");
                Console.WriteLine();

                var query = context.Products
                    .Include(p => p.Category)
                    .ToList();

                Console.WriteLine($"Loaded products: {query.Count}");
                Console.WriteLine();

                var electronics = query
                    .Where(p => p.Category != null && p.Category.Name == "Electronics")
                    .OrderByDescending(p => p.Price)
                    .ToList();

                if (!electronics.Any())
                {
                    Console.WriteLine("No electronics products found.");
                    return;
                }

                foreach (var product in electronics)
                {
                    var categoryName = product.Category?.Name ?? "No category";

                    Console.WriteLine(
                        $"Id: {product.Id}, " +
                        $"Name: {product.Name}, " +
                        $"Price: {product.Price}, " +
                        $"Category: {categoryName}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred:");
                Console.WriteLine(ex.Message);
            }
        }

        public static void SeedData(MyDbContext context)
        {
            if (context.Categories.Any() || context.Products.Any())
                return;

            var electronics = new Category
            {
                Name = "Electronics",
                Description = "Electronic devices"
            };

            var books = new Category
            {
                Name = "Books",
                Description = "Printed books and e-books"
            };

            context.Categories.AddRange(electronics, books);

            var supplier1 = new Supplier
            {
                Name = "Tech World",
                ContactPerson = "Alice Johnson",
                Email = "alice@techworld.com",
                Phone = "070-1234567"
            };

            var supplier2 = new Supplier
            {
                Name = "Book Haven",
                ContactPerson = "Bob Smith",
                Email = "bob@bookhaven.com",
                Phone = "070-9876543"
            };

            context.Suppliers.AddRange(supplier1, supplier2);

            context.Products.AddRange(
                new Product
                {
                    Name = "Laptop",
                    Description = "Gaming laptop",
                    Price = 15000m,
                    StockQuantity = 8,
                    Category = electronics,
                    Supplier = supplier1
                },
                new Product
                {
                    Name = "Smartphone",
                    Description = "Latest model smartphone",
                    Price = 599.99m,
                    StockQuantity = 15,
                    Category = electronics,
                    Supplier = supplier1
                },
                new Product
                {
                    Name = "Novel",
                    Description = "Fictional novel",
                    Price = 19.99m,
                    StockQuantity = 100,
                    Category = books,
                    Supplier = supplier2
                }
            );

            context.SaveChanges();
        }

        public static void Pause()
        {
            Console.WriteLine();
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        public static void ShowSuppliersWithLowStockProducts(MyDbContext context)
        {
            var suppliers = context.Suppliers
                .Include(s => s.Products)
                .Where(s => s.Products.Any(p => p.StockQuantity < 10))
                .ToList();

            if (!suppliers.Any())
            {
                Console.WriteLine("No suppliers found with products under 10 in stock.");
                return;
            }

            foreach (var supplier in suppliers)
            {
                Console.WriteLine($"Supplier: {supplier.Name}");

                var lowStockProducts = supplier.Products
                    .Where(p => p.StockQuantity < 10)
                    .ToList();

                foreach (var product in lowStockProducts)
                {
                    Console.WriteLine(
                        $"   Product: {product.Name}, Stock: {product.StockQuantity}");
                }

                Console.WriteLine();
            }
        }
    }
}