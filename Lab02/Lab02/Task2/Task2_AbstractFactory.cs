using System;

namespace Lab2.Task2
{
    // Інтерфейси продуктів
    public interface ILaptop { void Work(); }
    public interface INetbook { void SurfInternet(); }
    public interface IEBook { void Read(); }
    public interface ISmartphone { void Call(); }

    // Конкретні продукти для IProne
    public class IProneLaptop : ILaptop { public void Work() => Console.WriteLine("Working on IProne Laptop"); }
    public class IProneNetbook : INetbook { public void SurfInternet() => Console.WriteLine("Surfing on IProne Netbook"); }
    public class IProneEBook : IEBook { public void Read() => Console.WriteLine("Reading on IProne EBook"); }
    public class IProneSmartphone : ISmartphone { public void Call() => Console.WriteLine("Calling from IProne Smartphone"); }

    // Конкретні продукти для Kiaomi
    public class KiaomiLaptop : ILaptop { public void Work() => Console.WriteLine("Working on Kiaomi Laptop"); }
    public class KiaomiNetbook : INetbook { public void SurfInternet() => Console.WriteLine("Surfing on Kiaomi Netbook"); }
    public class KiaomiEBook : IEBook { public void Read() => Console.WriteLine("Reading on Kiaomi EBook"); }
    public class KiaomiSmartphone : ISmartphone { public void Call() => Console.WriteLine("Calling from Kiaomi Smartphone"); }

    // Конкретні продукти для Balaxy
    public class BalaxyLaptop : ILaptop { public void Work() => Console.WriteLine("Working on Balaxy Laptop"); }
    public class BalaxyNetbook : INetbook { public void SurfInternet() => Console.WriteLine("Surfing on Balaxy Netbook"); }
    public class BalaxyEBook : IEBook { public void Read() => Console.WriteLine("Reading on Balaxy EBook"); }
    public class BalaxySmartphone : ISmartphone { public void Call() => Console.WriteLine("Calling from Balaxy Smartphone"); }

    // Інтерфейс Абстрактної фабрики
    public interface ITechFactory
    {
        ILaptop CreateLaptop();
        INetbook CreateNetbook();
        IEBook CreateEBook();
        ISmartphone CreateSmartphone();
    }

    // Конкретні фабрики
    public class IProneFactory : ITechFactory
    {
        public ILaptop CreateLaptop() => new IProneLaptop();
        public INetbook CreateNetbook() => new IProneNetbook();
        public IEBook CreateEBook() => new IProneEBook();
        public ISmartphone CreateSmartphone() => new IProneSmartphone();
    }

    public class KiaomiFactory : ITechFactory
    {
        public ILaptop CreateLaptop() => new KiaomiLaptop();
        public INetbook CreateNetbook() => new KiaomiNetbook();
        public IEBook CreateEBook() => new KiaomiEBook();
        public ISmartphone CreateSmartphone() => new KiaomiSmartphone();
    }

    public class BalaxyFactory : ITechFactory
    {
        public ILaptop CreateLaptop() => new BalaxyLaptop();
        public INetbook CreateNetbook() => new BalaxyNetbook();
        public IEBook CreateEBook() => new BalaxyEBook();
        public ISmartphone CreateSmartphone() => new BalaxySmartphone();
    }
}