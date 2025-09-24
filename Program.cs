using System;
using System.Collections.Generic;
using System.Linq;

namespace ShopInventory
{
    public enum ProductCategory
    {
        Electronics,
        Clothing,
        Food,
        Books,
        Sports
    }

    public class Product
    {
        public string Code { get; private set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public bool InStock => Quantity > 0;
        public ProductCategory Category { get; set; }

        public Product(string name, decimal price, int quantity, ProductCategory category, string code)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Название не может быть пустым");
            if (price < 0) throw new ArgumentException("Цена не может быть отрицательной");
            if (quantity < 0) throw new ArgumentException("Количество не может быть отрицательным");

            Name = name;
            Price = price;
            Quantity = quantity;
            Category = category;
            Code = code;
        }

        public override string ToString()
        {
            return $"Код: {Code}, Название: {Name}, Цена: {Price:C}, " +
                   $"Количество: {Quantity}, В наличии: {(InStock ? "Да" : "Нет")}, " +
                   $"Категория: {Category}";
        }
    }

    public class SaleRecord
    {
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime SaleDate { get; set; }

        public SaleRecord(string productCode, string productName, int quantity, decimal totalPrice)
        {
            ProductCode = productCode;
            ProductName = productName;
            Quantity = quantity;
            TotalPrice = totalPrice;
            SaleDate = DateTime.Now;
        }

        public override string ToString()
        {
            return $"{SaleDate:yyyy-MM-dd HH:mm} - {ProductName} ({ProductCode}), " +
                   $"Количество: {Quantity}, Сумма: {TotalPrice:C}";
        }
    }

    public class Shop
    {
        private List<Product> products;
        private Stack<SaleRecord> salesHistory;
        private List<SaleRecord> salesReport;
        private int productCounter;

        public Shop()
        {
            products = new List<Product>();
            salesHistory = new Stack<SaleRecord>();
            salesReport = new List<SaleRecord>();
            productCounter = 1;
            InitializeTestData();
        }

        private void InitializeTestData()
        {
            AddProduct("Ноутбук", 50000, 10, ProductCategory.Electronics);
            AddProduct("Футболка", 1500, 50, ProductCategory.Clothing);
            AddProduct("Хлеб", 50, 100, ProductCategory.Food);
            AddProduct("Книга по C#", 1200, 20, ProductCategory.Books);
            AddProduct("Мяч", 2500, 15, ProductCategory.Sports);
        }

        private string GenerateProductCode() => $"1{productCounter++:D6}";

        public void AddProduct(string name, decimal price, int quantity, ProductCategory category)
        {
            try
            {
                var product = new Product(name, price, quantity, category, GenerateProductCode());
                products.Add(product);
                Console.WriteLine($" Товар добавлен: {product}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($" Ошибка: {ex.Message}");
            }
        }

        public void RemoveProduct(string code)
        {
            var product = products.FirstOrDefault(p => p.Code == code);
            if (product != null)
            {
                products.Remove(product);
                Console.WriteLine($" Товар с кодом {code} удален");
            }
            else
            {
                Console.WriteLine($" Товар с кодом {code} не найден");
            }
        }
        public void OrderSupply(string code, int quantity)
        {
            if (quantity <= 0)
            {
                Console.WriteLine(" Количество должно быть положительным");
                return;
            }

            var product = products.FirstOrDefault(p => p.Code == code);
            if (product != null)
            {
                product.Quantity += quantity;
                Console.WriteLine($" Поставка заказана. Новое количество: {product.Quantity}");
            }
            else
            {
                Console.WriteLine($" Товар с кодом {code} не найден");
            }
        }

        public void SellProduct(string code, int quantity)
        {
            if (quantity <= 0)
            {
                Console.WriteLine(" Количество должно быть положительным");
                return;
            }

            var product = products.FirstOrDefault(p => p.Code == code);
            if (product == null)
            {
                Console.WriteLine($" Товар с кодом {code} не найден");
                return;
            }

            if (product.Quantity < quantity)
            {
                Console.WriteLine($" Недостаточно товара. Доступно: {product.Quantity}");
                return;
            }

            product.Quantity -= quantity;
            decimal totalPrice = product.Price * quantity;

            var saleRecord = new SaleRecord(product.Code, product.Name, quantity, totalPrice);
            salesHistory.Push(saleRecord);
            salesReport.Add(saleRecord);

            Console.WriteLine($" Продажа совершена:");
            Console.WriteLine($"   Товар: {product.Name}");
            Console.WriteLine($"   Количество: {quantity} шт.");
            Console.WriteLine($"   Сумма: {totalPrice:C}");
            Console.WriteLine($"   Остаток: {product.Quantity} шт.");
        }

        public void SearchProducts(string searchTerm)
        {
            var results = products.Where(p =>
                p.Code.Contains(searchTerm) ||
                p.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                p.Category.ToString().Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
            ).ToList();

            if (results.Any())
            {
                Console.WriteLine($" Найдено товаров: {results.Count}");
                foreach (var product in results)
                {
                    Console.WriteLine($"   {product}");
                }
            }
            else
            {
                Console.WriteLine(" Товары не найдены");
            }
        }

        public void ShowSalesHistory()
        {
            if (!salesHistory.Any())
            {
                Console.WriteLine(" История продаж пуста");
                return;
            }

            Console.WriteLine(" История продаж:");
            foreach (var sale in salesHistory.Reverse())
            {
                Console.WriteLine($"   {sale}");
            }
        }

        public void UndoLastSale()
        {
            if (!salesHistory.Any())
            {
                Console.WriteLine(" Нет продаж для отмены");
                return;
            }

            var lastSale = salesHistory.Pop();
            var product = products.FirstOrDefault(p => p.Code == lastSale.ProductCode);

            if (product != null)
            {
                product.Quantity += lastSale.Quantity;
                salesReport.Remove(lastSale);
                Console.WriteLine($" Продажа отменена: {lastSale.ProductName}");
                Console.WriteLine($"   Возвращено: {lastSale.Quantity} шт.");
            }
            else
            {
                Console.WriteLine(" Товар из продажи не найден в каталоге");
            }
        }

        public void GenerateSalesReport()
        {
            if (!salesReport.Any())
            {
                Console.WriteLine("Нет данных о продажах");
                return;
            }
            Console.WriteLine("ОТЧЕТ О ПРОДАЖАХ:");
            Console.WriteLine("====================");

            decimal totalRevenue = 0;
            int totalItemsSold = 0;

            foreach (var sale in salesReport)
            {
                Console.WriteLine($"   {sale}");
                totalRevenue += sale.TotalPrice;
                totalItemsSold += sale.Quantity;
            }

            Console.WriteLine("====================");
            Console.WriteLine($"   Всего продаж: {salesReport.Count}");
            Console.WriteLine($"   Товаров продано: {totalItemsSold} шт.");
            Console.WriteLine($"   Общая выручка: {totalRevenue:C}");
        }

        public void ShowAllProducts()
        {
            if (!products.Any())
            {
                Console.WriteLine(" Товаров нет");
                return;
            }

            Console.WriteLine(" ВСЕ ТОВАРЫ:");
            foreach (var product in products)
            {
                Console.WriteLine($"   {product}");
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("🛒 СИСТЕМА УЧЕТА ТОВАРОВ");
            Shop shop = new Shop();

            while (true)
            {
                Console.WriteLine("\n=== МЕНЮ ===");
                Console.WriteLine("1. Показать все товары");
                Console.WriteLine("2. Добавить товар");
                Console.WriteLine("3. Удалить товар");
                Console.WriteLine("4. Заказать поставку");
                Console.WriteLine("5. Продать товар");
                Console.WriteLine("6. Поиск товаров");
                Console.WriteLine("7. История продаж");
                Console.WriteLine("8. Отменить последнюю продажу");
                Console.WriteLine("9. Отчет о продажах");
                Console.WriteLine("0. Выход");
                Console.Write("Выберите: ");

                switch (Console.ReadLine())
                {
                    case "1": shop.ShowAllProducts(); break;
                    case "2": AddProductMenu(shop); break;
                    case "3": RemoveProductMenu(shop); break;
                    case "4": OrderSupplyMenu(shop); break;
                    case "5": SellProductMenu(shop); break;
                    case "6": SearchProductsMenu(shop); break;
                    case "7": shop.ShowSalesHistory(); break;
                    case "8": shop.UndoLastSale(); break;
                    case "9": shop.GenerateSalesReport(); break;
                    case "0": Console.WriteLine(" До свидания!"); return;
                    default: Console.WriteLine("Неверный выбор"); break;
                }
            }
        }

        static void AddProductMenu(Shop shop)
        {
            Console.Write("Название: ");
            string name = Console.ReadLine();

            Console.Write("Цена: ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal price) || price < 0)
            {
                Console.WriteLine(" Неверная цена"); return;
            }

            Console.Write("Количество: ");
            if (!int.TryParse(Console.ReadLine(), out int quantity) || quantity < 0)
            {
                Console.WriteLine(" Неверное количество"); return;
            }

            Console.WriteLine("Категории: 0-Electronics, 1-Clothing, 2-Food, 3-Books, 4-Sports");
            Console.Write("Категория (0-4): ");
            if (!Enum.TryParse(Console.ReadLine(), out ProductCategory category) || !Enum.IsDefined(typeof(ProductCategory), category))
            {
                Console.WriteLine(" Неверная категория"); return;
            }

            shop.AddProduct(name, price, quantity, category);
        }

        static void RemoveProductMenu(Shop shop)
        {
            Console.Write("Код товара: ");
            shop.RemoveProduct(Console.ReadLine());
        }
        static void OrderSupplyMenu(Shop shop)
        {
            Console.Write("Код товара: ");
            string code = Console.ReadLine();

            Console.Write("Количество: ");
            if (!int.TryParse(Console.ReadLine(), out int quantity) || quantity <= 0)
            {
                Console.WriteLine(" Неверное количество"); return;
            }

            shop.OrderSupply(code, quantity);
        }

        static void SellProductMenu(Shop shop)
        {
            Console.Write("Код товара: ");
            string code = Console.ReadLine();

            Console.Write("Количество: ");
            if (!int.TryParse(Console.ReadLine(), out int quantity) || quantity <= 0)
            {
                Console.WriteLine(" Неверное количество"); return;
            }

            shop.SellProduct(code, quantity);
        }

        static void SearchProductsMenu(Shop shop)
        {
            Console.Write("Поиск (код/название/категория): ");
            string term = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(term))
            {
                Console.WriteLine(" Пустой запрос"); return;
            }
            shop.SearchProducts(term);
        }
    }
}