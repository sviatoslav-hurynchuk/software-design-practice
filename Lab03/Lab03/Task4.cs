using Lab3.Task4;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Lab3.Task4
{
    public interface ITextReader
    {
        char[][] ReadText(string filePath);
    }

    public class SmartTextReader : ITextReader
    {
        public char[][] ReadText(string filePath)
        {
            string[] lines = File.ReadAllLines(filePath);

            char[][] result = new char[lines.Length][];
            for (int i = 0; i < lines.Length; i++)
            {
                result[i] = lines[i].ToCharArray();
            }

            return result;
        }
    }

    public class SmartTextChecker : ITextReader
    {
        private readonly ITextReader _realReader;

        public SmartTextChecker(ITextReader realReader)
        {
            _realReader = realReader;
        }

        public char[][] ReadText(string filePath)
        {
            Console.WriteLine($"[Log] Відкриття файлу: {filePath}...");

            char[][] result = _realReader.ReadText(filePath);

            int totalLines = result.Length;
            int totalChars = result.Sum(line => line.Length);

            Console.WriteLine($"[Log] Файл успішно прочитано. Загальна кількість рядків: {totalLines}, символів: {totalChars}.");
            Console.WriteLine($"[Log] Закриття файлу: {filePath}...");

            return result;
        }
    }

    public class SmartTextReaderLocker : ITextReader
    {
        private readonly ITextReader _realReader;
        private readonly Regex _restrictedPattern;

        public SmartTextReaderLocker(ITextReader realReader, string regexPattern)
        {
            _realReader = realReader;
            _restrictedPattern = new Regex(regexPattern);
        }

        public char[][] ReadText(string filePath)
        {
            if (_restrictedPattern.IsMatch(filePath))
            {
                Console.WriteLine("Access denied!");
                return null;
            }

            return _realReader.ReadText(filePath);
        }
    }

    public static class Task4Demo
    {
        public static void Run()
        {
            Console.WriteLine("=== Завдання 4: Проксі ===");

            string safeFile = "safe_document.txt";
            string secretFile = "secret_passwords.txt";
            File.WriteAllText(safeFile, "Hello World\nThis is a test file.");
            File.WriteAllText(secretFile, "admin:12345");

            ITextReader baseReader = new SmartTextReader();

            ITextReader loggingReader = new SmartTextChecker(baseReader);


            ITextReader securedReader = new SmartTextReaderLocker(loggingReader, @"secret.*\.txt$");

            Console.WriteLine("-> Спроба прочитати безпечний файл:");
            char[][] safeResult = securedReader.ReadText(safeFile);

            Console.WriteLine("\n-> Спроба прочитати секретний файл:");
            char[][] secretResult = securedReader.ReadText(secretFile);

            if (File.Exists(safeFile)) File.Delete(safeFile);
            if (File.Exists(secretFile)) File.Delete(secretFile);

            Console.WriteLine();
        }
    }
}