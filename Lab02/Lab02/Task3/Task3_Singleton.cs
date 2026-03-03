using System;

namespace Lab2.Task3
{
    public sealed class Authenticator
    {
        // Використовуємо Lazy для лінивої та потокобезпечної ініціалізації
        private static readonly Lazy<Authenticator> _instance =
            new Lazy<Authenticator>(() => new Authenticator());

        public static Authenticator Instance => _instance.Value;

        // Приватний конструктор забороняє створення через new
        private Authenticator()
        {
            Console.WriteLine("Authenticator instance created.");
        }

        public void AuthenticateUser(string username)
        {
            Console.WriteLine($"User {username} authenticated successfully.");
        }
    }
}