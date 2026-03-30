using System;
using Lab04;
// using Lab4.Task5;

namespace Lab4
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Task1Demo.Run();

            Task2Demo.Run();

            Task5Demo.Run();

            Console.WriteLine("\nНатисніть будь-яку клавішу для завершення...");
            Console.ReadKey();
        }
    }
}