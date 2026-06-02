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
                Console.WriteLine("1. Visa electronik");
                Console.WriteLine("2. Visa leverantörer med lågt lager");
                Console.WriteLine("3. Visa total ordervärde för den senaste månaden");
                Console.WriteLine("4. Visa topp 3 bästsäljande produkter");
                Console.WriteLine("5. Visa kategorier och produktantal");
                Console.WriteLine("6. Visa beställningar > 1000 kr ");
                Console.WriteLine("0. Avsluta");
                Console.Write("Välj ett alternativ: ");

                string choice = (Console.ReadLine() ?? "").Trim();

                Console.WriteLine($"Du skrev: {choice}");

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

                    case "6":
                        ShowHighValueOrders(context);
                        Pause();
                        break;

                    case "0":
                        return;

                    default:
                        Console.WriteLine("Ogilitigt val.");
                        Pause();
                        break;
                }
            }
        }
        


        public static void ShowElectronics(MyDbContext context)
        {
            try
            {
                Console.WriteLine($"Antal kategorier : {context.Categories.Count()}");
                Console.WriteLine($"Antal produkter: {context.Products.Count()}");
                Console.WriteLine();

                var query = context.Products
                    .Include(p => p.Category)
                    .ToList();

                Console.WriteLine($"Produkter i lager: {query.Count}");
                Console.WriteLine();

                var electronics = query
                    .Where(p => p.Category != null && p.Category.Name == "Electronik")
                    .OrderByDescending(p => p.Price)
                    .ToList();

                if (!electronics.Any())
                {
                    Console.WriteLine("Inga elektronikprodukter hittades.");
                    return;
                }

                foreach (var product in electronics)
                {
                    var categoryName = product.Category?.Name ?? "Ingen kategori";

                    Console.WriteLine(
                        $"Id: {product.Id}, " +
                        $"Namn: {product.Name}, " +
                        $"Pris: {product.Price}, " +
                        $"Kategori: {categoryName}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ett fel uppstod:");
                Console.WriteLine(ex.Message);
            }
        }

        public static void SeedData(MyDbContext context)
        {
            if (context.Categories.Any() ||
                context.Suppliers.Any() ||
                context.Products.Any() ||
                context.Customers.Any() ||
                context.Orders.Any() ||
                context.OrderDetails.Any())
                return;

            context.Categories.AddRange(
                new Category { Id = 1, Name = "Electronik", Description = "Elektronik och tekniska produkter" },
                new Category { Id = 2, Name = "Hem & Kök", Description = "Produkter för hemmet och köket" },
                new Category { Id = 3, Name = "Kläder", Description = "Kläder och accessoarer" },
                new Category { Id = 4, Name = "Sport", Description = "Sportutrustning och träningsprodukter" },
                new Category { Id = 5, Name = "Böcker", Description = "Böcker och litteratur" }
            );

            context.Suppliers.AddRange(
                new Supplier { Id = 1, Name = "TechVision AB", ContactPerson = "Anna Lindberg", Email = "anna@techvision.se", Phone = "070-123-4567" },
                new Supplier { Id = 2, Name = "HomeStyle", ContactPerson = "Johan Bergman", Email = "johan@homestyle.se", Phone = "073-234-5678" },
                new Supplier { Id = 3, Name = "Fashion First", ContactPerson = "Maria Ek", Email = "maria@fashionfirst.se", Phone = "076-345-6789" },
                new Supplier { Id = 4, Name = "SportMax", ContactPerson = "Erik Strand", Email = "erik@sportmax.se", Phone = "072-456-7890" },
                new Supplier { Id = 5, Name = "Nordic Electronics", ContactPerson = "Karl Holm", Email = "karl@nordicelec.se", Phone = "070-567-8901" },
                new Supplier { Id = 6, Name = "Global Gadgets", ContactPerson = "Lisa Björk", Email = "lisa@globalgadgets.se", Phone = "073-678-9012" }
            );

            context.Products.AddRange(
                new Product { Id = 1, Name = "iPhone 13 Pro", Description = "Smartphone med 128GB lagring", Price = 11999, StockQuantity = 15, CategoryId = 1, SupplierId = 1 },
                new Product { Id = 2, Name = "Samsung TV 55\"", Description = "4K Smart TV med HDR", Price = 8999, StockQuantity = 8, CategoryId = 1, SupplierId = 5 },
                new Product { Id = 3, Name = "Sony WH-1000XM4", Description = "Trådlösa hörlurar med brusreducering", Price = 3499, StockQuantity = 7, CategoryId = 1, SupplierId = 5 },
                new Product { Id = 4, Name = "MacBook Air", Description = "Laptop med M1-chip och 8GB RAM", Price = 12499, StockQuantity = 12, CategoryId = 1, SupplierId = 1 },
                new Product { Id = 5, Name = "Espressomaskin", Description = "Automatisk espressomaskin", Price = 4995, StockQuantity = 6, CategoryId = 2, SupplierId = 2 },
                new Product { Id = 6, Name = "Matberedare", Description = "Multifunktionell köksmaskin", Price = 1299, StockQuantity = 20, CategoryId = 2, SupplierId = 2 },
                new Product { Id = 7, Name = "Vinterjacka", Description = "Varm jacka för vinterbruk", Price = 1999, StockQuantity = 25, CategoryId = 3, SupplierId = 3 },
                new Product { Id = 8, Name = "Löparskor", Description = "Skor för långdistanslöpning", Price = 1499, StockQuantity = 18, CategoryId = 4, SupplierId = 4 },
                new Product { Id = 9, Name = "Yogamatta", Description = "Halkfri yogamatta", Price = 349, StockQuantity = 30, CategoryId = 4, SupplierId = 4 },
                new Product { Id = 10, Name = "Bestsellerroman", Description = "Populär skönlitterär roman", Price = 249, StockQuantity = 40, CategoryId = 5, SupplierId = 2 },
                new Product { Id = 11, Name = "Gaming PC", Description = "Högpresterande dator för gaming", Price = 18999, StockQuantity = 5, CategoryId = 1, SupplierId = 6 },
                new Product { Id = 12, Name = "Tablet", Description = "10\" surfplatta med WiFi", Price = 4299, StockQuantity = 9, CategoryId = 1, SupplierId = 5 },
                new Product { Id = 13, Name = "Bluetooth-högtalare", Description = "Portabel högtalare med 20h batteritid", Price = 899, StockQuantity = 22, CategoryId = 1, SupplierId = 6 },
                new Product { Id = 14, Name = "Kaffebryggare", Description = "Programmerbar kaffebryggare", Price = 799, StockQuantity = 14, CategoryId = 2, SupplierId = 2 },
                new Product { Id = 15, Name = "Träningströja", Description = "Funktionströja för träning", Price = 499, StockQuantity = 35, CategoryId = 3, SupplierId = 3 }
            );

            context.Customers.AddRange(
                new Customer { Id = 1, Name = "Anders Svensson", Age = 34, Email = "anders@example.com", Phone = "070-111-2233", Address = "Storgatan 1, Stockholm" },
                new Customer { Id = 2, Name = "Emma Johansson", Age = 29, Email = "emma@example.com", Phone = "073-222-3344", Address = "Kungsgatan 15, Göteborg" },
                new Customer { Id = 3, Name = "Lars Nilsson", Age = 41, Email = "lars@example.com", Phone = "076-333-4455", Address = "Drottninggatan 8, Malmö" },
                new Customer { Id = 4, Name = "Sofia Lindgren", Age = 31, Email = "sofia@example.com", Phone = "072-444-5566", Address = "Sveavägen 22, Uppsala" },
                new Customer { Id = 5, Name = "Peter Karlsson", Age = 38, Email = "peter@example.com", Phone = "070-555-6677", Address = "Järntorget 5, Göteborg" }
            );

            context.Orders.AddRange(
                new Order { Id = 1, OrderDate = new DateTime(2026, 3, 1), CustomerId = 1, TotalAmount = 11999, Status = "Levererad" },
                new Order { Id = 2, OrderDate = new DateTime(2026, 3, 5), CustomerId = 2, TotalAmount = 9695, Status = "Levererad" },
                new Order { Id = 3, OrderDate = new DateTime(2026, 3, 10), CustomerId = 3, TotalAmount = 18999, Status = "Behandlas" },
                new Order { Id = 4, OrderDate = new DateTime(2026, 3, 12), CustomerId = 4, TotalAmount = 3499, Status = "Levererad" },
                new Order { Id = 5, OrderDate = new DateTime(2026, 3, 15), CustomerId = 5, TotalAmount = 17494, Status = "Skickad" },
                new Order { Id = 6, OrderDate = new DateTime(2026, 2, 20), CustomerId = 1, TotalAmount = 899, Status = "Levererad" },
                new Order { Id = 7, OrderDate = new DateTime(2026, 2, 25), CustomerId = 3, TotalAmount = 2546, Status = "Levererad" },
                new Order { Id = 8, OrderDate = new DateTime(2026, 3, 18), CustomerId = 2, TotalAmount = 3496, Status = "Skickad" },
                new Order { Id = 9, OrderDate = new DateTime(2026, 3, 20), CustomerId = 4, TotalAmount = 11896, Status = "Behandlas" },
                new Order { Id = 10, OrderDate = new DateTime(2026, 3, 22), CustomerId = 5, TotalAmount = 1299, Status = "Behandlas" }
            );

            context.OrderDetails.AddRange(
                new OrderDetail { Id = 1, OrderId = 1, ProductId = 1, Quantity = 1, UnitPrice = 11999 },
                new OrderDetail { Id = 2, OrderId = 2, ProductId = 3, Quantity = 2, UnitPrice = 3499 },
                new OrderDetail { Id = 3, OrderId = 2, ProductId = 13, Quantity = 3, UnitPrice = 899 },
                new OrderDetail { Id = 4, OrderId = 3, ProductId = 11, Quantity = 1, UnitPrice = 18999 },
                new OrderDetail { Id = 5, OrderId = 4, ProductId = 3, Quantity = 1, UnitPrice = 3499 },
                new OrderDetail { Id = 6, OrderId = 5, ProductId = 4, Quantity = 1, UnitPrice = 12499 },
                new OrderDetail { Id = 7, OrderId = 5, ProductId = 5, Quantity = 1, UnitPrice = 4995 },
                new OrderDetail { Id = 8, OrderId = 6, ProductId = 13, Quantity = 1, UnitPrice = 899 },
                new OrderDetail { Id = 9, OrderId = 7, ProductId = 8, Quantity = 1, UnitPrice = 1499 },
                new OrderDetail { Id = 10, OrderId = 7, ProductId = 9, Quantity = 3, UnitPrice = 349 },
                new OrderDetail { Id = 11, OrderId = 8, ProductId = 7, Quantity = 1, UnitPrice = 1999 },
                new OrderDetail { Id = 12, OrderId = 8, ProductId = 15, Quantity = 3, UnitPrice = 499 },
                new OrderDetail { Id = 13, OrderId = 9, ProductId = 2, Quantity = 1, UnitPrice = 8999 },
                new OrderDetail { Id = 14, OrderId = 9, ProductId = 6, Quantity = 1, UnitPrice = 1299 },
                new OrderDetail { Id = 15, OrderId = 9, ProductId = 14, Quantity = 2, UnitPrice = 799 },
                new OrderDetail { Id = 16, OrderId = 10, ProductId = 6, Quantity = 1, UnitPrice = 1299 }
            );

            context.SaveChanges();
        }

        public static void Pause()
        {
            Console.WriteLine();
            Console.WriteLine("Tryck på valfri tangent för att fortsätta...");
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
                Console.WriteLine("Inga leverantörer hittades med produkter under 10 i lager.");
                return;
            }

            foreach (var supplier in suppliers)
            {
                Console.WriteLine($"Leverantör: {supplier.Name}");

                var lowStockProducts = supplier.Products
                    .Where(p => p.StockQuantity < 10)
                    .ToList();

                foreach (var product in lowStockProducts)
                {
                    Console.WriteLine(
                        $"   Produkt: {product.Name}, Lager st: {product.StockQuantity}");
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
                $"Totalt ordervärde för de senaste 30 dagarna: {totalOrderValue} kr");

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
                Console.WriteLine("Inga orderuppgifter hittades.");
                return;
            }

            Console.WriteLine("Topp 3 mest sålda produkter (efter kvantitet):");
            foreach (var item in topProducts)
            {
                Console.WriteLine(
                    $"Produkt: {item.ProductName}, Totalt sålt: {item.TotalQuantity}");
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
                Console.WriteLine("Inga kategorier hittades.");
                return;
            }

            Console.WriteLine("Kategorier och produktantal:");
            foreach (var item in categoryCounts)
            {
                Console.WriteLine(
                    $"Kategori: {item.CategoryName}, Produkter: {item.ProductCount}");
            }
        }

        public static void ShowHighValueOrders(MyDbContext context)
        {
            var orders = context.Orders
                .Where(o => o.TotalAmount > 1000)
                .Include(o => o.Customer)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .ToList();

            if (!orders.Any())
            {
                Console.WriteLine("Inga beställningar över 1000 kr hittades.");
                return;
            }

            foreach (var order in orders)
            {
                Console.WriteLine(
                    $"Order ID: {order.Id}, Datum: {order.OrderDate:yyyy-MM-dd}, " +
                    $"Kund: {order.Customer.Name}, Totalt: {order.TotalAmount} kr, Status: {order.Status}");

                foreach (var detail in order.OrderDetails)
                {
                    Console.WriteLine(
                        $"   Produkt: {detail.Product.Name}, Mängd: {detail.Quantity}, A-pris: {detail.UnitPrice} kr");
                }

                Console.WriteLine();
            }
        }
    }
}