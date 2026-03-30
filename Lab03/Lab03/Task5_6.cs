using System;
using System.Collections.Generic;
using System.Linq;

namespace Lab3.Task5_6
{
    public abstract class LightNode
    {
        public abstract string OuterHTML { get; }
        public abstract string InnerHTML { get; }
    }

    public class LightTextNode : LightNode
    {
        private readonly string _text;

        public LightTextNode(string text)
        {
            _text = text;
        }

        public override string OuterHTML => _text;
        public override string InnerHTML => _text;
    }

    public class ElementState
    {
        public string TagName { get; }
        public string DisplayType { get; }
        public string ClosingType { get; }

        public ElementState(string tagName, string displayType, string closingType)
        {
            TagName = tagName;
            DisplayType = displayType;
            ClosingType = closingType;
        }
    }

    public class ElementStateFactory
    {
        private static readonly Dictionary<string, ElementState> _states = new Dictionary<string, ElementState>();

        public static ElementState GetState(string tagName, string displayType, string closingType)
        {
            string key = $"{tagName}_{displayType}_{closingType}";
            if (!_states.ContainsKey(key))
            {
                _states[key] = new ElementState(tagName, displayType, closingType);
            }
            return _states[key];
        }

        public static int StatesCount => _states.Count;
    }

    public class LightElementNode : LightNode
    {
        private readonly ElementState _state;
        public List<string> CssClasses { get; }
        public List<LightNode> Children { get; }

        public LightElementNode(string tagName, string displayType, string closingType, List<string> cssClasses = null)
        {
            _state = ElementStateFactory.GetState(tagName, displayType, closingType);
            CssClasses = cssClasses ?? new List<string>();
            Children = new List<LightNode>();
        }

        public void Add(LightNode node)
        {
            Children.Add(node);
        }

        public override string InnerHTML => string.Join("", Children.Select(c => c.OuterHTML));

        public override string OuterHTML
        {
            get
            {
                string classes = CssClasses.Count > 0 ? $" class=\"{string.Join(" ", CssClasses)}\"" : "";
                if (_state.ClosingType == "single")
                {
                    return $"<{_state.TagName}{classes} />";
                }
                return $"<{_state.TagName}{classes}>{InnerHTML}</{_state.TagName}>";
            }
        }
    }

    public interface IImageLoadStrategy
    {
        void Load(string href);
    }

    public class NetworkLoadStrategy : IImageLoadStrategy
    {
        public void Load(string href)
        {
            Console.WriteLine($"[Мережа]: Завантаження картинки по HTTP з адреси -> {href}");
        }
    }

    public class FileSystemLoadStrategy : IImageLoadStrategy
    {
        public void Load(string href)
        {
            Console.WriteLine($"[Файлова система]: Читання локального файлу з шляху -> {href}");
        }
    }

    public class LightImageNode : LightNode
    {
        public string Href { get; }
        private readonly IImageLoadStrategy _loadStrategy;

        public LightImageNode(string href)
        {
            Href = href;

            if (href.StartsWith("http://") || href.StartsWith("https://"))
            {
                _loadStrategy = new NetworkLoadStrategy();
            }
            else
            {
                _loadStrategy = new FileSystemLoadStrategy();
            }
        }

        public override string InnerHTML => "";

        public override string OuterHTML
        {
            get
            {
                _loadStrategy.Load(Href);
                return $"<img src=\"{Href}\" />";
            }
        }
    }


    public static class Task5_6Demo
    {
        public static void Run()
        {
            Console.WriteLine("\n=== Завдання 4: Стратегія ===");

            var localImage = new LightImageNode("C:\\images\\avatar.png");
            Console.WriteLine(localImage.OuterHTML);

            Console.WriteLine();

            var networkImage = new LightImageNode("https://example.com/banner.jpg");
            Console.WriteLine(networkImage.OuterHTML);
        }
    }
}