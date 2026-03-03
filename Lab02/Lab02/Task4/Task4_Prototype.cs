using System;
using System.Collections.Generic;

namespace Lab2.Task4
{
    public class Virus : ICloneable
    {
        public double Weight { get; set; }
        public int Age { get; set; }
        public string Name { get; set; }
        public string Species { get; set; }
        public List<Virus> Children { get; set; }

        public Virus(double weight, int age, string name, string species)
        {
            Weight = weight;
            Age = age;
            Name = name;
            Species = species;
            Children = new List<Virus>();
        }

        public void AddChild(Virus child)
        {
            Children.Add(child);
        }

        // Глибоке клонування
        public object Clone()
        {
            // Копіюємо базові типи (значення)
            Virus clone = (Virus)this.MemberwiseClone();

            // Створюємо новий список для дітей і рекурсивно клонуємо кожного
            clone.Children = new List<Virus>();
            foreach (var child in this.Children)
            {
                clone.Children.Add((Virus)child.Clone());
            }

            return clone;
        }

        public void PrintVirus(int level = 0)
        {
            string indent = new string('-', level * 2);
            Console.WriteLine($"{indent}> Virus: {Name}, Age: {Age}, Species: {Species}, Children: {Children.Count}");
            foreach (var child in Children)
            {
                child.PrintVirus(level + 1);
            }
        }
    }
}