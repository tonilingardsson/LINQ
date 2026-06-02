using LINQ;
using LINQ.Models;
using Microsoft.EntityFrameworkCore;

namespace LINQ
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            SQLitePCL.Batteries.Init();
            using var context = new MyDbContext();
            context.Database.Migrate();
            SeedData(context);

            //bool running = true;

            while (true)
            {
                Console.Clear();
                Console.WriteLine("1. Show electronics");
                Console.WriteLine("2. Show suppliers with low stock");
                Console.WriteLine("3. Show total order value for the last month");
                Console.WriteLine("4. Show top 3 best-selling products");
                Console.WriteLine("5. Show categories and product counts");
                Console.WriteLine("0. Exit");
                Console.Write("Choose an option: ");

                string choice = (Console.ReadLine() ?? "").Trim();

                Console.WriteLine($"You entered: {choice}");

                switch (choice)
                {
                    case "1":
                        ShowElectronics(context);
                        Pause();
                        break;

                    case "2":
                        ShowSuppliersWithLowStockProducts(context);
                        Pause();
                        break;

                    case "3":
                        ShowTotalOrderValueLastMonth(context);
                        Pause();
                        break;

                    case "4":
                        ShowTop3MostSoldProducts(context);
                        Pause();
                        break;

                        case "5":
                            ShowCategoryProductCounts(context);
                            Pause();
                            break;

                    case "0":
                        return;

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
            if (context.Categories.Any() || context.Products.Any() || context.Orders.Any())
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
                    Id = 1,
                    Name = "Laptop",
                    Description = "Gaming laptop",
                    Price = 15000d,
                    StockQuantity = 8,
                    Category = electronics,
                    Supplier = supplier1
                },
                new Product
                {
                    Id = 2,
                    Name = "Smartphone",
                    Description = "Latest model smartphone",
                    Price = 600d,
                    StockQuantity = 15,
                    Category = electronics,
                    Supplier = supplier1
                },
                new Product
                {
                    Id = 3,
                    Name = "Novel",
                    Description = "Fictional novel",
                    Price = 20d,
                    StockQuantity = 100,
                    Category = books,
                    Supplier = supplier2
                },
                new Product
                {
                    Id = 4,
                    Name = "E-book Reader",
                    Description = "Portable e-book reader",
                    Price = 5000d,
                    StockQuantity = 5,
                    Category = electronics,
                    Supplier = supplier1
                }
            );

            context.Orders.AddRange(
                new Order
                {
                Id =1,
                OrderDate = new DateTime(2026, 5, 1),
                TotalAmount = 60000d,
                Status = "Paid"},
                new Order
                {
                Id =2,
                OrderDate = new DateTime(2026, 5, 15),
                TotalAmount = 20000d,
                Status = "Paid"},
                new Order
                {
                    Id = 3,
                    OrderDate = new DateTime(2026, 5, 9),
                    TotalAmount = 200d,
                    Status = "Paid"
                },
                new Order
                {
                    Id = 4,
                    OrderDate = DateTime.Now.AddDays(-5),
                    TotalAmount = 45000d,
                    Status = "Paid"
                }

            );

            context.OrderDetails.AddRange(
    new OrderDetail
    {
        Id = 1,
        OrderId = 1,
        ProductId = 1,
        Quantity = 4,
        UnitPrice = 15000d
    },
    new OrderDetail
    {
        Id = 2,
        OrderId = 2,
        ProductId = 2,
        Quantity = 2,
        UnitPrice = 1200d
    },
    new OrderDetail
    {
        Id = 3,
        OrderId = 3,
        ProductId = 3,
        Quantity = 1,
        UnitPrice = 20d
    },
    new OrderDetail
    {
        Id = 4,
        OrderId = 4,
        ProductId = 1,
        Quantity = 3,
        UnitPrice = 15000d
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

        public static void ShowTotalOrderValueLastMonth(MyDbContext context)
        {
            var fromDate = DateTime.Now.AddDays(-30);

            var ordersLastMonth = context.Orders
                .Where(o => o.OrderDate >= fromDate)
                .ToList();


            double totalOrderValue = ordersLastMonth.Sum(o => o.TotalAmount); 

            Console.WriteLine(
                $"Total order value for the last 30 days: {totalOrderValue} kr");

            var allOrders = context.Orders.ToList();

            foreach (var order in allOrders)
            {
                Console.WriteLine($"{order.Id} - {order.OrderDate} - {order.TotalAmount}");
            }

        }

        public static void ShowTop3MostSoldProducts(MyDbContext context)
        {
            var topProducts = context.OrderDetails
                .Include(od => od.Product)
                .GroupBy(od => new { od.ProductId, od.Product.Name })
                .Select(g => new
                {
                    ProductId = g.Key.ProductId,
                    ProductName = g.Key.Name,
                    TotalQuantity = g.Sum(od => od.Quantity)
                })
                .OrderByDescending(x => x.TotalQuantity)
                .Take(3)
                .ToList();

            if (!topProducts.Any())
            {
                Console.WriteLine("No order details found.");
                return;
            }

            Console.WriteLine("Top 3 most sold products (by quantity):");
            foreach (var item in topProducts)
            {
                Console.WriteLine(
                    $"Product: {item.ProductName}, Total sold: {item.TotalQuantity}");
            }
        }

        public static void ShowCategoryProductCounts(MyDbContext context)
        {
            var categoryCounts = context.Categories
                .Select(c => new
                {
                    CategoryName = c.Name,
                    ProductCount = c.Products.Count()
                })
                .ToList();

            if (!categoryCounts.Any())
            {
                Console.WriteLine("No categories found.");
                return;
            }

            Console.WriteLine("Categories and product counts:");
            foreach (var item in categoryCounts)
            {
                Console.WriteLine(
                    $"Category: {item.CategoryName}, Products: {item.ProductCount}");
            }
        }
    }
}