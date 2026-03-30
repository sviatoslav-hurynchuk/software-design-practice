using System;

namespace Lab04
{
    public interface ISupportHandler
    {
        ISupportHandler SetNext(ISupportHandler handler);
        bool HandleRequest(string request);
    }

    public abstract class AbstractSupportHandler : ISupportHandler
    {
        private ISupportHandler _nextHandler;

        public ISupportHandler SetNext(ISupportHandler handler)
        {
            _nextHandler = handler;
            return handler;
        }

        public virtual bool HandleRequest(string request)
        {
            if (_nextHandler != null)
            {
                return _nextHandler.HandleRequest(request);
            }
            return false;
        }
    }


    public class BotHandler : AbstractSupportHandler
    {
        public override bool HandleRequest(string request)
        {
            if (request == "1")
            {
                Console.WriteLine("Бот: Ваш поточний тариф - 'Студентський'. Баланс: 50 грн.");
                return true;
            }
            return base.HandleRequest(request); 
        }
    }

    public class GeneralOperatorHandler : AbstractSupportHandler
    {
        public override bool HandleRequest(string request)
        {
            if (request == "2")
            {
                Console.WriteLine("Оператор: Щоб змінити тариф, перейдіть у додаток або надішліть SMS на номер 111.");
                return true;
            }
            return base.HandleRequest(request);
        }
    }

    public class TechSupportHandler : AbstractSupportHandler
    {
        public override bool HandleRequest(string request)
        {
            if (request == "3")
            {
                Console.WriteLine("Тех. підтримка: Ми бачимо проблеми з вишкою у вашому районі. Ремонт займе 2 години.");
                return true;
            }
            return base.HandleRequest(request);
        }
    }

    public class BillingHandler : AbstractSupportHandler
    {
        public override bool HandleRequest(string request)
        {
            if (request == "4")
            {
                Console.WriteLine("Фінансовий відділ: Ваш платіж знаходиться в обробці банком. Очікуйте зарахування протягом доби.");
                return true;
            }
            return base.HandleRequest(request);
        }
    }

    public static class Task1Demo
    {
        public static void Run()
        {
            var bot = new BotHandler();
            var operatorHandler = new GeneralOperatorHandler();
            var techSupport = new TechSupportHandler();
            var billing = new BillingHandler();

            bot.SetNext(operatorHandler).SetNext(techSupport).SetNext(billing);

            bool isResolved = false;

            while (!isResolved)
            {
                Console.WriteLine("\n=== Система підтримки користувачів ===");
                Console.WriteLine("Оберіть ваше питання:");
                Console.WriteLine("1 - Дізнатися баланс та тариф (Бот)");
                Console.WriteLine("2 - Як змінити тариф? (Оператор)");
                Console.WriteLine("3 - Немає інтернету (Тех. підтримка)");
                Console.WriteLine("4 - Де мої гроші після поповнення? (Фінансовий відділ)");
                Console.WriteLine("Будь-яка інша клавіша - Зв'язок з президентом компанії (Невідомий запит)");
                Console.Write("Ваш вибір: ");

                string request = Console.ReadLine();

                isResolved = bot.HandleRequest(request);

                if (!isResolved)
                {
                    Console.WriteLine("\n[!] На жаль, ми не змогли розпізнати ваш запит або знайти вільного спеціаліста.");
                    Console.WriteLine("Спробуйте обрати інший варіант з меню.");
                }
                else
                {
                    Console.WriteLine("\n[+] Ваше питання було успішно вирішено. Дякуємо за звернення!");
                }
            }
        }
    }
}