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

        private readonly Dictionary<string, List<Action>> _eventListeners;

        public LightElementNode(string tagName, string displayType, string closingType, List<string> cssClasses = null)
        {
            _state = ElementStateFactory.GetState(tagName, displayType, closingType);
            CssClasses = cssClasses ?? new List<string>();
            Children = new List<LightNode>();

            _eventListeners = new Dictionary<string, List<Action>>();
        }

        public void Add(LightNode node)
        {
            Children.Add(node);
        }

        public void AddEventListener(string eventType, Action listener)
        {
            if (!_eventListeners.ContainsKey(eventType))
            {
                _eventListeners[eventType] = new List<Action>();
            }
            _eventListeners[eventType].Add(listener);
        }

        public void DispatchEvent(string eventType)
        {
            Console.WriteLine($"\n[Подія '{eventType}'] спрацювала на елементі <{_state.TagName}>");
            if (_eventListeners.ContainsKey(eventType))
            {
                foreach (var listener in _eventListeners[eventType])
                {
                    listener.Invoke();
                }
            }
            else
            {
                Console.WriteLine("-> Немає підписників на цю подію.");
            }
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

    public static class Task5_6Demo
    {
        public static void Run()
        {
            Console.WriteLine("\n=== Завдання 5_6: Спостерігач ===");
            var button = new LightElementNode("button", "inline", "paired");
            button.Add(new LightTextNode("Натисни мене"));

            button.AddEventListener("click", () => Console.WriteLine("Обробник 1: Кнопку натиснуто!"));
            button.AddEventListener("mouseover", () => Console.WriteLine("Обробник 2: Колір змінено на червоний."));

            button.DispatchEvent("mouseover");
            button.DispatchEvent("click");
        }
    }
}