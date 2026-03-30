using System;
using System.Collections.Generic;
using System.Linq;

namespace Lab04
{
    public interface ICommandCentre
    {
        void RequestLanding(Aircraft aircraft);
        void RequestTakeOff(Aircraft aircraft);
    }

    public class Runway
    {
        public readonly Guid Id = Guid.NewGuid();
        public bool IsBusy { get; set; }

        public void HighLightRed()
        {
            Console.WriteLine($"Runway {this.Id} is busy! (Red Light)");
        }

        public void HighLightGreen()
        {
            Console.WriteLine($"Runway {this.Id} is free! (Green Light)");
        }
    }

    public class Aircraft
    {
        public string Name { get; }
        public Guid? CurrentRunwayId { get; set; }
        public bool IsTakingOff { get; set; }

        private readonly ICommandCentre _commandCentre;

        public Aircraft(string name, ICommandCentre commandCentre)
        {
            Name = name;
            _commandCentre = commandCentre;
        }

        public void RequestLanding()
        {
            _commandCentre.RequestLanding(this);
        }

        public void RequestTakeOff()
        {
            IsTakingOff = true;
            _commandCentre.RequestTakeOff(this);
        }

        public void Land(Guid runwayId)
        {
            Console.WriteLine($"Aircraft {this.Name} is landing.");
            CurrentRunwayId = runwayId;
            IsTakingOff = false;
            Console.WriteLine($"Aircraft {this.Name} has landed.");
        }
    }

    public class CommandCentre : ICommandCentre
    {
        private readonly List<Runway> _runways = new List<Runway>();
        private readonly List<Aircraft> _aircrafts = new List<Aircraft>();

        public CommandCentre(Runway[] runways, Aircraft[] aircrafts)
        {
            _runways.AddRange(runways);
            _aircrafts.AddRange(aircrafts);
        }

        public void RequestLanding(Aircraft aircraft)
        {
            Console.WriteLine($"Command Centre: Checking runways for {aircraft.Name}...");

            var freeRunway = _runways.FirstOrDefault(r => !r.IsBusy);

            if (freeRunway != null)
            {
                aircraft.Land(freeRunway.Id);

                freeRunway.IsBusy = true;
                freeRunway.HighLightRed();
            }
            else
            {
                Console.WriteLine($"Could not land, all runways are busy.");
            }
        }

        public void RequestTakeOff(Aircraft aircraft)
        {
            if (aircraft.CurrentRunwayId.HasValue)
            {
                Console.WriteLine($"Aircraft {aircraft.Name} is taking off.");

                var runway = _runways.FirstOrDefault(r => r.Id == aircraft.CurrentRunwayId.Value);

                if (runway != null)
                {
                    runway.IsBusy = false;
                    aircraft.CurrentRunwayId = null;
                    runway.HighLightGreen();
                    Console.WriteLine($"Aircraft {aircraft.Name} has took off.");
                }
            }
        }
    }

    public static class Task2Demo
    {
        public static void Run()
        {
            Console.WriteLine("\n=== Завдання 2: Посередник ===");

            var runways = new Runway[] { new Runway(), new Runway() };

            
            var commandCentre = new CommandCentre(runways, new Aircraft[0]);

            var flight1 = new Aircraft("Boeing 737", commandCentre);
            var flight2 = new Aircraft("Airbus A320", commandCentre);
            var flight3 = new Aircraft("Cessna 172", commandCentre);

            Console.WriteLine("--- Запити на посадку ---");
            flight1.RequestLanding();
            Console.WriteLine();
            flight2.RequestLanding();
            Console.WriteLine();
            flight3.RequestLanding(); 

            Console.WriteLine("\n--- Запит на зліт ---");
            flight1.RequestTakeOff();

            Console.WriteLine("\n--- Повторний запит на посадку ---");
            flight3.RequestLanding(); 
        }
    }
}