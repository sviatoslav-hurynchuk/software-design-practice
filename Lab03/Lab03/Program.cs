using System;
using Lab3.Task1;
using Lab3.Task2;
using Lab3.Task3;
using Lab3.Task4;
using Lab3.Task5_6;

namespace Lab3
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            //Task1Demo.Run();

            //Task2Demo.Run();

            //Task3Demo.Run();
            //Task4Demo.Run();
            Task5_6Demo.Run();

            Console.WriteLine("Натисніть будь-яку клавішу для завершення...");
            Console.ReadKey();
        }
    }
}