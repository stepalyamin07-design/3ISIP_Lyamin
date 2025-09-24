using System;
using System.Collections.Generic;
using System.Text;

class Program
{
   
    class TextStatistics
    {
        public int WordCount { get; set; }
        public string ShortestWord { get; set; }
        public int SentenceCount { get; set; }
        public int VowelCount { get; set; }
        public int ConsonantCount { get; set; }
        public string LongestWord { get; set; }
        public Dictionary<char, int> LetterFrequency { get; set; }
        public string OriginalText { get; set; }
    }

    static void Main()
    {
        List<TextStatistics> allStatistics = new List<TextStatistics>();
        bool continueWorking = true;

        while (continueWorking)
        {
            Console.WriteLine("Введите текст (не менее 100 символов):");
            string text = Console.ReadLine();


            if (text.Length < 100)
            {
                Console.WriteLine("Текст должен содержать не менее 100 символов!");
                continue;
            }

            
            TextStatistics stats = ProcessText(text);
            allStatistics.Add(stats);

           
            PrintStatistics(stats);

          
            Console.WriteLine("\nХотите ввести новый текст? (да/нет)");
            string response = Console.ReadLine().ToLower();
            continueWorking = response == "да";
        }

      
        PrintAllStatistics(allStatistics);
    }

    static TextStatistics ProcessText(string text)
    {
        TextStatistics stats = new TextStatistics
        {
            OriginalText = text,
            LetterFrequency = new Dictionary<char, int>()
        };

        char[] wordSeparators = GetWordSeparators();
        char[] sentenceSeparators = { '.', '!', '?', ';' };

    
        string[] words = SplitWords(text, wordSeparators);
        stats.WordCount = words.Length;
        FindShortestAndLongestWord(words, stats);

 
        stats.SentenceCount = CountSentences(text, sentenceSeparators);

    
        CountVowelsAndConsonants(text, stats);

 
        BuildLetterFrequency(text, stats);

        return stats;
    }


    static char[] GetWordSeparators()
    {
        List<char> separators = new List<char>();
        for (char c = char.MinValue; c < char.MaxValue; c++)
        {
            if (char.IsPunctuation(c) || char.IsSeparator(c) || char.IsSymbol(c))
                separators.Add(c);
        }
        return separators.ToArray();
    }

  
    static string[] SplitWords(string text, char[] separators)
    {
        List<string> words = new List<string>();
        StringBuilder currentWord = new StringBuilder();

        foreach (char c in text)
        {
            if (Array.IndexOf(separators, c) >= 0)
            {
                if (currentWord.Length > 0)
                {
                    words.Add(currentWord.ToString());
                    currentWord.Clear();
                }
            }
            else
            {
                currentWord.Append(c);
            }
        }


        if (currentWord.Length > 0)
            words.Add(currentWord.ToString());

        return words.ToArray();
    }


    static void FindShortestAndLongestWord(string[] words, TextStatistics stats)
    {
        if (words.Length == 0) return;

        stats.ShortestWord = words[0];
        stats.LongestWord = words[0];

        for (int i = 1; i < words.Length; i++)
        {
            if (words[i].Length < stats.ShortestWord.Length)
                stats.ShortestWord = words[i];

            if (words[i].Length > stats.LongestWord.Length)
                stats.LongestWord = words[i];
        }
    }

    static int CountSentences(string text, char[] separators)
    {
        int count = 0;
        bool inSentence = false;

        foreach (char c in text)
        {
            if (Array.IndexOf(separators, c) >= 0)
            {
                if (inSentence)
                {
                    count++;
                    inSentence = false;
                }
            }
            else if (!char.IsWhiteSpace(c))
            {
                inSentence = true;
            }
        }

        if (inSentence) count++;
        return count;
    }


    static void CountVowelsAndConsonants(string text, TextStatistics stats)
    {
        string vowels = "aeiouyаеёиоуыэюяAEIOUYАЕЁИОУЫЭЮЯ";
        string consonants = "bcdfghjklmnpqrstvwxzбвгджзйклмнпрстфхцчшщBCDFGHJKLMNPQRSTVWXZБВГДЖЗЙКЛМНПРСТФХЦЧШЩ";

        foreach (char c in text)
        {
            if (vowels.IndexOf(c) >= 0)
                stats.VowelCount++;
            else if (consonants.IndexOf(c) >= 0)
                stats.ConsonantCount++;
        }
    }


    static void BuildLetterFrequency(string text, TextStatistics stats)
    {
        foreach (char c in text)
        {
            if (char.IsLetter(c))
            {
                char lowerChar = char.ToLower(c);
                if (stats.LetterFrequency.ContainsKey(lowerChar))
                    stats.LetterFrequency[lowerChar]++;
                else
                    stats.LetterFrequency[lowerChar] = 1;
            }
        }
    }

    static void PrintStatistics(TextStatistics stats)
    {
        Console.WriteLine("\n=== СТАТИСТИКА ТЕКСТА ===");
        Console.WriteLine($"Количество слов: {stats.WordCount}");
        Console.WriteLine($"Самое короткое слово: {stats.ShortestWord}");
        Console.WriteLine($"Количество предложений: {stats.SentenceCount}");
        Console.WriteLine($"Гласные буквы: {stats.VowelCount}");
        Console.WriteLine($"Согласные буквы: {stats.ConsonantCount}");
        Console.WriteLine($"Самое длинное слово: {stats.LongestWord}");

        Console.WriteLine("Частота букв:");
        foreach (var entry in stats.LetterFrequency)
        {
            Console.WriteLine($"{entry.Key}: {entry.Value}");
        }
    }

   
    static void PrintAllStatistics(List<TextStatistics> allStatistics)
    {
        Console.WriteLine("\n=== СТАТИСТИКА ПО ВСЕМ ТЕКСТАМ ===");
        for (int i = 0; i < allStatistics.Count; i++)
        {
            Console.WriteLine($"\n--- Текст #{i + 1} ---");
            Console.WriteLine($"Первые 50 символов: {allStatistics[i].OriginalText.Substring(0, Math.Min(50, allStatistics[i].OriginalText.Length))}...");
            PrintStatistics(allStatistics[i]);
        }
    }
}
