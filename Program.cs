
using System;
using System.Collections.Generic;
using System.Linq;

namespace LibraryManagement
{
    public enum Genre { Fantasy, ScienceFiction, Mystery, Romance, Horror, Thriller }

    public class Book
    {
        private static int _nextId = 1;

        public int Id { get; }
        public string Title { get; }
        public string Author { get; }
        public Genre Genre { get; }
        public int Year { get; }
        public decimal Price { get; }

        public Book(string title, string author, Genre genre, int year, decimal price)
        {
            if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Название обязательно");
            if (string.IsNullOrWhiteSpace(author)) throw new ArgumentException("Автор обязателен");
            if (year < 1000 || year > DateTime.Now.Year) throw new ArgumentException("Некорректный год");
            if (price < 0) throw new ArgumentException("Цена не может быть отрицательной");

            Id = _nextId++;
            Title = title;
            Author = author;
            Genre = genre;
            Year = year;
            Price = price;
        }

        public override string ToString() =>
            $"ID: {Id}, {Title} - {Author} ({Genre}, {Year}), {Price:C}";
    }

    public class Library
    {
        private List<Book> _books = new List<Book>();

        public void AddBook(Book book) => _books.Add(book);

        public bool RemoveBook(int id)
        {
            var book = _books.FirstOrDefault(b => b.Id == id);
            return book != null && _books.Remove(book);
        }

        public IEnumerable<Book> FindBooks(Func<Book, bool> predicate) => _books.Where(predicate);

        public IEnumerable<Book> SortBy(Func<Book, object> keySelector) => _books.OrderBy(keySelector);

        public (Book min, Book max) GetPriceExtremes() =>
            (_books.OrderBy(b => b.Price).FirstOrDefault(), _books.OrderByDescending(b => b.Price).FirstOrDefault());

        public IEnumerable<IGrouping<string, Book>> GroupByAuthor() => _books.GroupBy(b => b.Author);

        public void AddTestData()
        {
            _books.AddRange(new[] {
                new Book("Властелин Колец", "Дж. Р. Р. Толкин", Genre.Fantasy, 1954, 1500),
                new Book("1984", "Джордж Оруэлл", Genre.ScienceFiction, 1949, 800),
                new Book("Убийство в Восточном экспрессе", "Агата Кристи", Genre.Mystery, 1934, 700),
                new Book("Гордость и предубеждение", "Джейн Остин", Genre.Romance, 1813, 600),
                new Book("Дракула", "Брэм Стокер", Genre.Horror, 1897, 900)
            });
        }

        public IEnumerable<Book> GetAllBooks() => _books;
    }

    class Program
    {
        static Library library = new Library();

        static void Main(string[] args)
        {
            library.AddTestData();

            while (true)
            {
                Console.WriteLine("\n=== Библиотека ===");
                Console.WriteLine("1. Добавить книгу\n2. Удалить книгу\n3. Найти книги\n4. Сортировать\n5. Цены мин/макс\n6. Группировка по авторам\n7. Все книги\n8. Выход");
                Console.Write("Выбор: ");

                try
                {
                    switch (Console.ReadLine())
                    {
                        case "1": AddBook(); break;
                        case "2": RemoveBook(); break;
                        case "3": FindBooks(); break;
                        case "4": SortBooks(); break;
                        case "5": ShowPriceExtremes(); break;
                        case "6": ShowAuthorsGroup(); break;
                        case "7": ShowAllBooks(); break;
                        case "8": return;
                        default: Console.WriteLine("Неверный ввод!"); break;
                    }
                }
                catch (Exception ex) { Console.WriteLine($"Ошибка: {ex.Message}"); }
            }
        }

        static void AddBook()
        {
            Console.Write("Название: ");
            var title = Console.ReadLine();
            Console.Write("Автор: ");
            var author = Console.ReadLine();

            Console.WriteLine("Жанры: " + string.Join(", ", Enum.GetNames(typeof(Genre))));
            Console.Write("Жанр: ");
            if (!Enum.TryParse<Genre>(Console.ReadLine(), true, out var genre))
                throw new ArgumentException("Неверный жанр");

            Console.Write("Год: ");
            if (!int.TryParse(Console.ReadLine(), out int year)) throw new ArgumentException("Неверный год");

            Console.Write("Цена: ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal price)) throw new ArgumentException("Неверная цена");

            library.AddBook(new Book(title, author, genre, year, price));
            Console.WriteLine("Книга добавлена!");
        }

        static void RemoveBook()
        {
            Console.Write("ID для удаления: ");
            if (int.TryParse(Console.ReadLine(), out int id))
                Console.WriteLine(library.RemoveBook(id) ? "Удалено!" : "Не найдено!");
            else
                Console.WriteLine("Неверный ID!");
        }

        static void FindBooks()
        {
            Console.Write("Поиск по (1-название, 2-автор, 3-жанр): ");
            var choice = Console.ReadLine();
            var results = choice switch
            {
                "1" => library.FindBooks(b => b.Title.Contains(Console.ReadLine() ?? "", StringComparison.OrdinalIgnoreCase)),
                "2" => library.FindBooks(b => b.Author.Contains(Console.ReadLine() ?? "", StringComparison.OrdinalIgnoreCase)),
                "3" => Enum.TryParse<Genre>(Console.ReadLine(), true, out var genre)
                      ? library.FindBooks(b => b.Genre == genre) : Enumerable.Empty<Book>(),
                _ => Enumerable.Empty<Book>()
            };

            var books = results.ToList();
            Console.WriteLine(books.Any() ? string.Join("\n", books) : "Не найдено!");
        }

        static void SortBooks()
        {
            Console.Write("Сортировать по (1-название, 2-год): ");
            var sorted = Console.ReadLine() switch
            {
                "1" => library.SortBy(b => b.Title),
                "2" => library.SortBy(b => b.Year),
                _ => Enumerable.Empty<Book>()
            };

            Console.WriteLine(string.Join("\n", sorted));
        }

        static void ShowPriceExtremes()
        {
            var (min, max) = library.GetPriceExtremes();
            Console.WriteLine($"Самая дешевая: {min}\nСамая дорогая: {max}");
        }

        static void ShowAuthorsGroup()
        {
            foreach (var group in library.GroupByAuthor())
                Console.WriteLine($"{group.Key}: {group.Count()} книг(и)");
        }

        static void ShowAllBooks()
        {
            Console.WriteLine(string.Join("\n", library.GetAllBooks()));
        }
    }
}