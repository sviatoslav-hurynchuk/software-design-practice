using System;
using System.IO;

namespace Lab3.Task1
{
    public interface ILogger
    {
        void Log(string message);
        void Error(string message);
        void Warn(string message);
    }

    public class Logger : ILogger
    {
        public void Log(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[LOG] {message}");
            Console.ResetColor();
        }

        public void Error(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[ERROR] {message}");
            Console.ResetColor();
        }

        public void Warn(string message)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine($"[WARN] {message}");
            Console.ResetColor();
        }
    }

    public class FileWriter
    {
        private readonly string _filePath;

        public FileWriter(string filePath)
        {
            _filePath = filePath;
        }

        public void Write(string text)
        {
            File.AppendAllText(_filePath, text);
        }

        public void WriteLine(string text)
        {
            File.AppendAllText(_filePath, text + Environment.NewLine);
        }
    }

    public class FileLoggerAdapter : ILogger
    {
        private readonly FileWriter _fileWriter;

        public FileLoggerAdapter(FileWriter fileWriter)
        {
            _fileWriter = fileWriter;
        }

        public void Log(string message)
        {
            _fileWriter.WriteLine($"[LOG - {DateTime.Now}]: {message}");
        }

        public void Error(string message)
        {
            _fileWriter.WriteLine($"[ERROR - {DateTime.Now}]: {message}");
        }

        public void Warn(string message)
        {
            _fileWriter.WriteLine($"[WARN - {DateTime.Now}]: {message}");
        }
    }

    public static class Task1Demo
    {
        public static void Run()
        {
            Console.WriteLine("=== Завдання 1: Адаптер ===");

            // напряму
            ILogger consoleLogger = new Logger();
            consoleLogger.Log("Систему успішно запущено.");
            consoleLogger.Warn("Пам'ять заповнена на 80%.");
            consoleLogger.Error("Збій підключення до бази даних.");

            Console.WriteLine("\n--- Переключення на файловий логер ---");

            // через адаптер
            string path = "log.txt";
            FileWriter writer = new FileWriter(path);
            ILogger fileLogger = new FileLoggerAdapter(writer);

            fileLogger.Log("Запис у файл: Систему успішно запущено.");
            fileLogger.Warn("Запис у файл: Пам'ять заповнена на 80%.");
            fileLogger.Error("Запис у файл: Збій підключення до бази даних.");

            Console.WriteLine($"Логи успішно записані у файл: {Path.GetFullPath(path)}\n");
        }
    }
}