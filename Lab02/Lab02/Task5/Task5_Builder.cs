using System;
using System.Collections.Generic;

namespace Lab2.Task5
{
    public class Character
    {
        public string Name { get; set; }
        public string Height { get; set; }
        public string Build { get; set; }
        public string HairColor { get; set; }
        public string EyeColor { get; set; }
        public string Clothing { get; set; }
        public List<string> Inventory { get; set; } = new List<string>();
        public List<string> Deeds { get; set; } = new List<string>();

        public void ShowInfo()
        {
            Console.WriteLine($"\nCharacter: {Name}");
            Console.WriteLine($"Appearance: {Height}, {Build}, Hair: {HairColor}, Eyes: {EyeColor}");
            Console.WriteLine($"Clothing: {Clothing}");
            Console.WriteLine($"Inventory: {string.Join(", ", Inventory)}");
            Console.WriteLine($"Deeds: {string.Join(", ", Deeds)}");
        }
    }

    // Загальний інтерфейс будівельника (Fluent)
    public interface ICharacterBuilder
    {
        ICharacterBuilder SetName(string name);
        ICharacterBuilder SetHeight(string height);
        ICharacterBuilder SetBuild(string build);
        ICharacterBuilder SetAppearance(string hairColor, string eyeColor);
        ICharacterBuilder SetClothing(string clothing);
        ICharacterBuilder AddToInventory(string item);
        Character Build();
    }

    // Будівельник Героя
    public class HeroBuilder : ICharacterBuilder
    {
        private Character _character = new Character();

        public ICharacterBuilder SetName(string name) { _character.Name = name; return this; }
        public ICharacterBuilder SetHeight(string height) { _character.Height = height; return this; }
        public ICharacterBuilder SetBuild(string build) { _character.Build = build; return this; }
        public ICharacterBuilder SetAppearance(string hairColor, string eyeColor) { _character.HairColor = hairColor; _character.EyeColor = eyeColor; return this; }
        public ICharacterBuilder SetClothing(string clothing) { _character.Clothing = clothing; return this; }
        public ICharacterBuilder AddToInventory(string item) { _character.Inventory.Add(item); return this; }

        // Специфічний метод для героя
        public HeroBuilder DoGoodDeed(string deed)
        {
            _character.Deeds.Add($"Good: {deed}");
            return this;
        }

        public Character Build() { return _character; }
    }

    // Будівельник Ворога
    public class EnemyBuilder : ICharacterBuilder
    {
        private Character _character = new Character();

        public ICharacterBuilder SetName(string name) { _character.Name = name; return this; }
        public ICharacterBuilder SetHeight(string height) { _character.Height = height; return this; }
        public ICharacterBuilder SetBuild(string build) { _character.Build = build; return this; }
        public ICharacterBuilder SetAppearance(string hairColor, string eyeColor) { _character.HairColor = hairColor; _character.EyeColor = eyeColor; return this; }
        public ICharacterBuilder SetClothing(string clothing) { _character.Clothing = clothing; return this; }
        public ICharacterBuilder AddToInventory(string item) { _character.Inventory.Add(item); return this; }

        // Специфічний метод для ворога
        public EnemyBuilder DoEvilDeed(string deed)
        {
            _character.Deeds.Add($"Evil: {deed}");
            return this;
        }

        public Character Build() { return _character; }
    }

    // Директор
    public class Director
    {
        public Character ConstructDreamHero(HeroBuilder builder)
        {
            builder.DoGoodDeed("Saved the princess");

            return builder
                .SetName("Arthur Pendragon")
                .SetHeight("185 cm")
                .SetBuild("Athletic")
                .SetAppearance("Blonde", "Blue")
                .SetClothing("Shining Armor")
                .AddToInventory("Excalibur")
                .AddToInventory("Shield")
                .Build();
        }

        public Character ConstructWorstEnemy(EnemyBuilder builder)
        {
            builder.DoEvilDeed("Destroyed the village");

            return builder
                .SetName("Dark Lord Sauron")
                .SetHeight("210 cm")
                .SetBuild("Muscular")
                .SetAppearance("Black", "Red")
                .SetClothing("Dark Cloak")
                .AddToInventory("The One Ring")
                .Build();
        }
    }
}