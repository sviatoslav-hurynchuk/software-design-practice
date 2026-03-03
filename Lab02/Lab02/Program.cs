using Lab2.Task1;
using Lab2.Task2;
using Lab2.Task3;
using Lab2.Task4;
using Lab2.Task5;
using System;
using System.Reflection.Emit;

namespace Lab2
{
    class Program
    {
        static void Main(string[] args)
        {
            DemonstrateTask1();
            DemonstrateTask2();
            DemonstrateTask3();
            DemonstrateTask4();
            DemonstrateTask5();

            Console.ReadLine();
        }

        static void DemonstrateTask1()
        {
            Console.WriteLine("=== TASK 1: Factory Method ===");
            SubscriptionCreator web = new WebSite();
            SubscriptionCreator app = new MobileApp();
            SubscriptionCreator call = new ManagerCall();

            web.PurchaseSubscription();
            app.PurchaseSubscription();
            call.PurchaseSubscription();
            Console.WriteLine();
        }

        static void DemonstrateTask2()
        {
            Console.WriteLine("=== TASK 2: Abstract Factory ===");
            ITechFactory iproneFactory = new IProneFactory();
            var myLaptop = iproneFactory.CreateLaptop();
            myLaptop.Work();

            ITechFactory kiaomiFactory = new KiaomiFactory();
            var myPhone = kiaomiFactory.CreateSmartphone();
            myPhone.Call();

            ITechFactory balaxyFactory = new BalaxyFactory();
            var myEbook = balaxyFactory.CreateEBook();
            myEbook.Read();
            Console.WriteLine();
        }

        static void DemonstrateTask3()
        {
            Console.WriteLine("=== TASK 3: Singleton ===");
            Authenticator auth1 = Authenticator.Instance;
            Authenticator auth2 = Authenticator.Instance;

            auth1.AuthenticateUser("Sviatoslav");

            Console.WriteLine($"Are both authenticators the same instance? {ReferenceEquals(auth1, auth2)}");
            Console.WriteLine();
        }

        static void DemonstrateTask4()
        {
            Console.WriteLine("=== TASK 4: Prototype ===");

            // 1 покоління
            Virus grandParent = new Virus(1.5, 10, "Covid-19", "Coronavirus");

            // 2 покоління
            Virus parent1 = new Virus(1.2, 5, "Covid-19 Alpha", "Coronavirus");
            Virus parent2 = new Virus(1.3, 4, "Covid-19 Beta", "Coronavirus");

            // 3 покоління
            Virus child1 = new Virus(0.8, 1, "Covid-19 Delta", "Coronavirus");

            parent1.AddChild(child1);
            grandParent.AddChild(parent1);
            grandParent.AddChild(parent2);

            Console.WriteLine("Original Virus Family:");
            grandParent.PrintVirus();

            Console.WriteLine("\nCloned Virus Family:");
            Virus clonedGrandParent = (Virus)grandParent.Clone();

            // Змінюємо ім'я клону, щоб показати, що це новий об'єкт
            clonedGrandParent.Name = "Covid-20 (Clone)";
            clonedGrandParent.Children[0].Name = "Mutated Alpha (Clone)";

            clonedGrandParent.PrintVirus();
            Console.WriteLine();
        }
        
        static void DemonstrateTask5()
        {
            Console.WriteLine("=== TASK 5: Builder ===");
            Director director = new Director();

            HeroBuilder heroBuilder = new HeroBuilder();
            Character myHero = director.ConstructDreamHero(heroBuilder);
            myHero.ShowInfo();

            EnemyBuilder enemyBuilder = new EnemyBuilder();
            Character myEnemy = director.ConstructWorstEnemy(enemyBuilder);
            myEnemy.ShowInfo();
            Console.WriteLine();
        }
    }
}