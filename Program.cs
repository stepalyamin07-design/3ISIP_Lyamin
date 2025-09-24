using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace ExpenseTracker
{
    class Expense
    {
        public string Name { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string Category { get; set; }

        public Expense(string name, decimal amount, DateTime date, string category = "Разное")
        {
            Name = name;
            Amount = amount;
            Date = date;
            Category = category;
        }

        public override string ToString()
        {
            return $"{Date:dd.MM.yyyy} - {Name} - {Amount} руб. [{Category}]";
        }
    }

    class ExpenseManager
    {
        private List<Expense> expenses = new List<Expense>();

        public void AddExpense(Expense expense) => expenses.Add(expense);
        public int ExpensesCount => expenses.Count;
        public bool HasExpenses => expenses.Count > 0;

        public void ShowAllExpenses()
        {
            if (!HasExpenses)
            {
                Console.WriteLine("Нет данных о тратах.");
                return;
            }

            Console.WriteLine("\n=== ВСЕ ТРАТЫ ===");
            for (int i = 0; i < expenses.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {expenses[i]}");
            }
        }

        public void ShowStatistics()
        {
            if (!HasExpenses)
            {
                Console.WriteLine("Нет данных для статистики.");
                return;
            }

            var stats = new
            {
                Total = expenses.Sum(e => e.Amount),
                Average = expenses.Average(e => e.Amount),
                Max = expenses.Max(e => e.Amount),
                Min = expenses.Min(e => e.Amount),
                Count = expenses.Count
            };

            Console.WriteLine("\n=== СТАТИСТИКА ===");
            Console.WriteLine($"Общая сумма: {stats.Total:F2} руб.");
            Console.WriteLine($"Средняя трата: {stats.Average:F2} руб.");
            Console.WriteLine($"Максимальная трата: {stats.Max:F2} руб.");
            Console.WriteLine($"Минимальная трата: {stats.Min:F2} руб.");
            Console.WriteLine($"Количество операций: {stats.Count}");

            // Дополнительная статистика по категориям
            var categoryStats = expenses
                .GroupBy(e => e.Category)
                .Select(g => new { Category = g.Key, Total = g.Sum(e => e.Amount) });

            Console.WriteLine("\nПо категориям:");
            foreach (var cat in categoryStats)
            {
                Console.WriteLine($"  {cat.Category}: {cat.Total:F2} руб.");
            }
        }

        public void BubbleSortByPrice(bool ascending = true)
        {
            if (!HasExpenses)
            {
                Console.WriteLine("Нет данных для сортировки.");
                return;
            }

            var sortedList = new List<Expense>(expenses);
            int n = sortedList.Count;

            for (int i = 0; i < n - 1; i++)
            {
                for (int j = 0; j < n - i - 1; j++)
                {
                    bool shouldSwap = ascending ?
                        sortedList[j].Amount > sortedList[j + 1].Amount :
                        sortedList[j].Amount < sortedList[j + 1].Amount;

                    if (shouldSwap)
                    {
                        (sortedList[j], sortedList[j + 1]) = (sortedList[j + 1], sortedList[j]);
                    }
                }
            }

            Console.WriteLine($"\n=== СОРТИРОВКА ПО ЦЕНЕ ({(ascending ? "ПО ВОЗРАСТАНИЮ" : "ПО УБЫВАНИЮ")}) ===");
            for (int i = 0; i < sortedList.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {sortedList[i]}");
            }

            Console.Write("\nПрименить сортировку к основному списку? (y/n): ");
            if (Console.ReadLine().ToLower() == "y")
            {
                expenses = sortedList;
                Console.WriteLine("Сортировка применена!");
            }
        }

        public void CurrencyConversion()
        {
            if (!HasExpenses)
            {
                Console.WriteLine("Нет данных для конвертации.");
                return;
            }

            var currencies = new Dictionary<string, (string name, string symbol)>
            {
                {"1", ("Доллар США", "USD")},
                {"2", ("Евро", "EUR")},
                {"3", ("Фунт стерлингов", "GBP")},
                {"4", ("Йена", "JPY")},
                {"5", ("Произвольный курс", "CUSTOM")}
            };

            Console.WriteLine("\n=== КОНВЕРТАЦИЯ ВАЛЮТЫ ===");
            foreach (var currency in currencies)
            {
                Console.WriteLine($"{currency.Key}. {currency.Value.name}");
            }

            Console.Write("Выберите валюту или введите 0 для отмены: ");
            string choice = Console.ReadLine();

            if (choice == "0") return;

            if (!currencies.ContainsKey(choice))
            {
                Console.WriteLine("Неверный выбор!");
                return;
            }

            decimal rate = GetExchangeRate(currencies[choice].name);
            if (rate <= 0) return;

            string symbol = currencies[choice].symbol == "CUSTOM" ? "у.е." : currencies[choice].symbol;

            Console.WriteLine($"\n=== ТРАТЫ В {symbol} (курс: {rate:F2} руб.) ===");
            foreach (var expense in expenses)
            {
                decimal convertedAmount = expense.Amount / rate;
                Console.WriteLine($"{expense.Date:dd.MM} - {expense.Name} - {convertedAmount:F2} {symbol}");
            }
        }

        private decimal GetExchangeRate(string currencyName)
        {
            while (true)
            {
                Console.Write($"Введите курс рубля к {currencyName}: ");
                string input = Console.ReadLine();

                if (input == "0") return 0;

                if (decimal.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal rate) && rate > 0)
                {
                    return rate;
                }
                Console.WriteLine("Неверный формат! Введите положительное число или 0 для отмены:");
            }
        }

        public void SearchByName()
        {
            if (!HasExpenses)
            {
                Console.WriteLine("Нет данных для поиска.");
                return;
            }

            Console.Write("\nВведите название для поиска: ");
            string searchTerm = Console.ReadLine()?.ToLower().Trim();

            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                Console.WriteLine("Пустой поисковый запрос!");
                return;
            }

            var results = expenses
                .Where(e => e.Name.ToLower().Contains(searchTerm))
                .ToList();

            Console.WriteLine($"\n=== РЕЗУЛЬТАТЫ ПОИСКА: '{searchTerm}' ===");

            if (results.Count == 0)
            {
                Console.WriteLine("Ничего не найдено.");
                return;
            }

            foreach (var (expense, index) in results.Select((e, i) => (e, i)))
            {
                Console.WriteLine($"{index + 1}. {expense}");
            }
            Console.WriteLine($"Найдено: {results.Count} операций");
        }

        public void AddSampleData()
        {
            expenses.AddRange(new[]
            {
                new Expense("Продукты в Пятерочке", 1567.50m, DateTime.Today.AddDays(-2), "Продукты"),
                new Expense("Бензин АИ-95", 2500m, DateTime.Today.AddDays(-1), "Транспорт"),
                new Expense("Кофе с собой", 350m, DateTime.Today, "Кафе"),
                new Expense("Кино", 800m, DateTime.Today.AddDays(-5), "Развлечения")
            });
        }
    }

    class Program
    {
        private static ExpenseManager manager = new ExpenseManager();

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.InputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("=== УЧЕТ РАСХОДОВ v2.0 ===");

            InitializeExpenses();
            ShowMainMenu();
        }

        static void InitializeExpenses()
        {
            Console.WriteLine("Хотите добавить тестовые данные? (y/n): ");
            if (Console.ReadLine().ToLower() == "y")
            {
                manager.AddSampleData();
                Console.WriteLine("Добавлено 4 тестовые операции!");
                return;
            }

            int operationsCount = GetOperationsCount();
            InputExpenses(operationsCount);
        }

        static int GetOperationsCount()
        {
            while (true)
            {
                Console.Write("Введите количество операций (2-40): ");
                if (int.TryParse(Console.ReadLine(), out int count) && count >= 2 && count <= 40)
                {
                    return count;
                }
                Console.WriteLine("Ошибка! Введите число от 2 до 40.");
            }
        }

        static void InputExpenses(int count)
        {
            Console.WriteLine("\nВведите траты в формате: Название; Сумма; [Категория]");
            Console.WriteLine("Пример 1: Влажные салфетки; 235");
            Console.WriteLine("Пример 2: Бензин; 1500; Транспорт");

            for (int i = 0; i < count; i++)
            {
                while (true)
                {
                    Console.Write($"Операция {i + 1}: ");
                    string input = Console.ReadLine();

                    if (TryParseExpense(input, out Expense expense))
                    {
                        manager.AddExpense(expense);
                        break;
                    }
                    Console.WriteLine("Ошибка формата! Используйте: Название; Сумма; [Категория]");
                }
            }
            Console.WriteLine("\nВсе операции успешно добавлены!");
        }

        static bool TryParseExpense(string input, out Expense expense)
        {
            expense = null;
            if (string.IsNullOrWhiteSpace(input)) return false;

            string[] parts = input.Split(';');
            if (parts.Length < 2) return false;

            string name = parts[0].Trim();
            if (string.IsNullOrEmpty(name)) return false;

            if (!decimal.TryParse(parts[1].Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal amount) || amount <= 0)
                return false;

            string category = parts.Length > 2 ? parts[2].Trim() : "Разное";
            DateTime date = parts.Length > 3 && DateTime.TryParse(parts[3].Trim(), out DateTime d) ? d : DateTime.Today;

            expense = new Expense(name, amount, date, category);
            return true;
        }

        static void ShowMainMenu()
        {
            while (true)
            {
                Console.WriteLine("\n=== ГЛАВНОЕ МЕНЮ ===");
                Console.WriteLine("1. Вывод всех трат");
                Console.WriteLine("2. Статистика");
                Console.WriteLine("3. Сортировка по цене (возрастание)");
                Console.WriteLine("4. Сортировка по цене (убывание)");
                Console.WriteLine("5. Конвертация валюты");
                Console.WriteLine("6. Поиск по названию");
                Console.WriteLine("7. О программе");
                Console.WriteLine("0. Выход");

                Console.Write("Выберите пункт меню: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1": manager.ShowAllExpenses(); break;
                    case "2": manager.ShowStatistics(); break;
                    case "3": manager.BubbleSortByPrice(true); break;
                    case "4": manager.BubbleSortByPrice(false); break;
                    case "5": manager.CurrencyConversion(); break;
                    case "6": manager.SearchByName(); break;
                    case "7": ShowAbout(); break;
                    case "0":
                        Console.WriteLine("До свидания!");
                        return;
                    default:
                        Console.WriteLine("Неверный выбор!");
                        break;
                }
            }
        }

        static void ShowAbout()
        {
            Console.WriteLine("\n=== О ПРОГРАММЕ ===");
            Console.WriteLine("Учет расходов v2.0");
            Console.WriteLine($"Количество записей: {manager.ExpensesCount}");
            Console.WriteLine("Доступные функции:");
            Console.WriteLine("- Ввод и хранение расходов");
            Console.WriteLine("- Статистика и анализ");
            Console.WriteLine("- Сортировка и поиск");
            Console.WriteLine("- Конвертация валют");
        }
    }
}