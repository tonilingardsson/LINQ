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
                    .Where(p => p.Category != null && p.Category.Name == "Electronics")
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


        public static void Pause()
        {
            Console.WriteLine();
            Console.WriteLine("Tryck på valfri tangent för att fortsätta...");
            Console.ReadKey();
        }

        public static void ShowSuppliersWithLowStockProducts(MyDbContext context)
        {
            var suppliers = context.Suppliers
                .Include(s => s.Products) // not include just select, it takes less resources
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
            // just a select is enough, no need to include the whole order details, it takes less resources
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
                // I do not need to get the Product.Id to know how much of each product is sold,
                // I just need the name and the quantity, so I include only the name, it takes less resources
                // Like the method below
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
                    ProductCount = c.Products.Count
                })
                .ToList(); // List is not necessary

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
                .Include(o => o.Customer) // Not overuse Include, use Select instead, 
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
            // TODO: move methods to Classes, seedData to a separate class,
            // and use interfaces for better structure and maintainability

        }
    }
}