using System;
using System.Collections.Generic;

namespace Lab2.Task1
{
    // Інтерфейс продукту
    public interface ISubscription
    {
        decimal MonthlyFee { get; }
        int MinPeriodMonths { get; }
        List<string> Channels { get; }
        void DisplayInfo();
    }

    // Конкретні продукти
    public class DomesticSubscription : ISubscription
    {
        public decimal MonthlyFee => 10.99m;
        public int MinPeriodMonths => 1;
        public List<string> Channels => new List<string> { "News", "Sports", "Movies" };

        public void DisplayInfo() => Console.WriteLine($"Domestic: {MonthlyFee}$, Min: {MinPeriodMonths} months.");
    }

    public class EducationalSubscription : ISubscription
    {
        public decimal MonthlyFee => 5.99m;
        public int MinPeriodMonths => 6;
        public List<string> Channels => new List<string> { "Discovery", "History", "Science" };

        public void DisplayInfo() => Console.WriteLine($"Educational: {MonthlyFee}$, Min: {MinPeriodMonths} months.");
    }

    public class PremiumSubscription : ISubscription
    {
        public decimal MonthlyFee => 25.99m;
        public int MinPeriodMonths => 12;
        public List<string> Channels => new List<string> { "All Channels", "4K Ultra HD", "Ad-free" };

        public void DisplayInfo() => Console.WriteLine($"Premium: {MonthlyFee}$, Min: {MinPeriodMonths} months.");
    }

    // Абстрактний творець
    public abstract class SubscriptionCreator
    {
        // Фабричний метод
        public abstract ISubscription CreateSubscription();

        public void PurchaseSubscription()
        {
            var subscription = CreateSubscription();
            Console.Write($"{this.GetType().Name} created -> ");
            subscription.DisplayInfo();
        }
    }

    // Конкретні творці
    public class WebSite : SubscriptionCreator
    {
        public override ISubscription CreateSubscription()
        {
            return new EducationalSubscription();
        }
    }

    public class MobileApp : SubscriptionCreator
    {
        public override ISubscription CreateSubscription()
        {
            return new PremiumSubscription();
        }
    }

    public class ManagerCall : SubscriptionCreator
    {
        public override ISubscription CreateSubscription()
        {
            return new DomesticSubscription();
        }
    }
}