using System;

namespace Lab3.Task2
{
    public abstract class Hero
    {
        public string Name { get; protected set; }

        public abstract int GetPower();
        public abstract string GetDescription();
    }

    public class Warrior : Hero
    {
        public Warrior(string name) { Name = name; }
        public override int GetPower() => 100;
        public override string GetDescription() => $"Warrior {Name}";
    }

    public class Mage : Hero
    {
        public Mage(string name) { Name = name; }
        public override int GetPower() => 80;
        public override string GetDescription() => $"Mage {Name}";
    }

    public class Palladin : Hero
    {
        public Palladin(string name) { Name = name; }
        public override int GetPower() => 90;
        public override string GetDescription() => $"Palladin {Name}";
    }

    public abstract class InventoryDecorator : Hero
    {
        protected Hero _hero;

        public InventoryDecorator(Hero hero)
        {
            _hero = hero;
            Name = hero.Name; 
        }
    }

    // інвентар
    public class Clothing : InventoryDecorator
    {
        public Clothing(Hero hero) : base(hero) { }

        public override int GetPower() => _hero.GetPower() + 10;
        public override string GetDescription() => _hero.GetDescription() + " + Steel Armor";
    }

    public class Weapon : InventoryDecorator
    {
        public Weapon(Hero hero) : base(hero) { }

        public override int GetPower() => _hero.GetPower() + 50;
        public override string GetDescription() => _hero.GetDescription() + " + Magic Sword";
    }

    public class Artifact : InventoryDecorator
    {
        public Artifact(Hero hero) : base(hero) { }

        public override int GetPower() => _hero.GetPower() + 100;
        public override string GetDescription() => _hero.GetDescription() + " + Amulet of Fire";
    }

    public static class Task2Demo
    {
        public static void Run()
        {
            Console.WriteLine("=== Завдання 2: Декоратор ===");

            Hero myHero = new Warrior("Arthur");
            Console.WriteLine($"{myHero.GetDescription()} | Загальна сила: {myHero.GetPower()}");

            myHero = new Clothing(myHero);
            Console.WriteLine($"{myHero.GetDescription()} | Загальна сила: {myHero.GetPower()}");

            myHero = new Weapon(myHero);
            Console.WriteLine($"{myHero.GetDescription()} | Загальна сила: {myHero.GetPower()}");

            myHero = new Artifact(myHero);
            Console.WriteLine($"{myHero.GetDescription()} | Загальна сила: {myHero.GetPower()}");

            Console.WriteLine();

            Hero mage = new Artifact(new Clothing(new Mage("Gandalf")));
            Console.WriteLine($"{mage.GetDescription()} | Загальна сила: {mage.GetPower()}\n");
        }
    }
}