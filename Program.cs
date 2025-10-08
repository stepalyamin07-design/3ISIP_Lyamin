using System;
using System.Collections.Generic;

class TextStatistics
{
    public string Text { get; set; }
    public int WordCount { get; set; }
    public string ShortestWord { get; set; }
    public string LongestWord { get; set; }
    public int SentenceCount { get; set; }
    public int VowelCount { get; set; }
    public int ConsonantCount { get; set; }
    public Dictionary<char, int> LetterFrequency { get; set; }

    public TextStatistics()
    {
        LetterFrequency = new Dictionary<char, int>();
    }
}

class Program
{
    static void Main()
    {
        List<TextStatistics> previousStats = new List<TextStatistics>();

        while (true)
        {
            Console.WriteLine("Введите текст (минимум 100 символов):");
            string input = Console.ReadLine();

            // Проверяем длину текста, если меньше 100 - просим ввести заново
            if (input.Length < 100)
            {
                Console.WriteLine("Текст слишком короткий, попробуйте еще раз.");
                continue;
            }

            TextStatistics stats = AnalyzeText(input);
            previousStats.Add(stats);

            // Выводим статистику для текущего текста
            PrintStatistics(stats);

            Console.WriteLine("Хотите обработать еще один текст? (да/нет)");
            string answer = Console.ReadLine();
            if (answer.ToLower() != "да") break;
        }

        Console.WriteLine("\nСтатистика по всем введенным текстам:");
        for (int i = 0; i < previousStats.Count; i++)
        {
            Console.WriteLine($"\nТекст #{i + 1}:");
            PrintStatistics(previousStats[i]);
        }
    }

    static TextStatistics AnalyzeText(string text)
    {
        TextStatistics stats = new TextStatistics();
        stats.Text = text;

        // Разбиваем текст на слова по пробелам и знакам препинания
        char[] separators = new char[] { ' ', '\n', '\r', '\t', ',', '.', '!', '?', ':', ';', '-', '(', ')', '"', '\'' };
        string[] words = text.Split(separators, StringSplitOptions.RemoveEmptyEntries);

        stats.WordCount = words.Length;

        // Поиск самого короткого и самого длинного слова
        string shortest = null;
        string longest = null;

        foreach (var word in words)
        {
            if (shortest == null || word.Length < shortest.Length)
                shortest = word;
            if (longest == null || word.Length > longest.Length)
                longest = word;
        }

        stats.ShortestWord = shortest;
        stats.LongestWord = longest;

        // Подсчёт предложений — считаем по знакам окончания '.', '!', '?' 
        int sentenceCount = 0;
        foreach (char c in text)
        {
            if (c == '.' || c == '!' || c == '?')
                sentenceCount++;
        }
        stats.SentenceCount = sentenceCount;

        // Подсчёт гласных и согласных
        string vowels = "аеёиоуыэюяАЕЁИОУЫЭЮЯaeiouAEIOU"; // русские и английские гласные
        int vowelCount = 0;
        int consonantCount = 0;

        // Создаем словарь частоты букв
        Dictionary<char, int> freq = new Dictionary<char, int>();

        foreach (char c in text)
        {
            // Определяем букву ли это (русские и английские алфавиты)
            if ((c >= 'А' && c <= 'я') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == 'ё' || c == 'Ё')
            {
                // Подсчёт гласных и согласных
                if (vowels.IndexOf(c) >= 0)
                    vowelCount++;
                else
                    consonantCount++;

                // Нормализуем букву к нижнему регистру для статистики
                char lowerChar = Char.ToLower(c);

                if (freq.ContainsKey(lowerChar))
                    freq[lowerChar]++;
                else
                    freq[lowerChar] = 1;
            }
        }

        stats.VowelCount = vowelCount;
        stats.ConsonantCount = consonantCount;
        stats.LetterFrequency = freq;

        return stats;
    }

    static void PrintStatistics(TextStatistics stats)
    {
        Console.WriteLine("\nСтатистика текста:");
        Console.WriteLine($"Количество слов: {stats.WordCount}");
        Console.WriteLine($"Самое короткое слово: {stats.ShortestWord}");
        Console.WriteLine($"Самое длинное слово: {stats.LongestWord}");
        Console.WriteLine($"Количество предложений: {stats.SentenceCount}");
        Console.WriteLine($"Количество гласных: {stats.VowelCount}");
        Console.WriteLine($"Количество согласных: {stats.ConsonantCount}");
        Console.WriteLine("Частота букв:");

        foreach (var pair in stats.LetterFrequency)
        {
            Console.WriteLine($"  {pair.Key}: {pair.Value}");
        }
    }
}