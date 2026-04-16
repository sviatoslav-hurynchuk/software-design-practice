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
        public string TagName => _state.TagName;
        public List<string> CssClasses { get; }
        public List<LightNode> Children { get; }

        public LightElementNode(string tagName, string displayType, string closingType, List<string> cssClasses = null)
        {
            _state = ElementStateFactory.GetState(tagName, displayType, closingType);
            CssClasses = cssClasses ?? new List<string>();
            Children = new List<LightNode>();

            OnCreated(); // Виклик хука при створенні
        }

        public void Add(LightNode node)
        {
            Children.Add(node);
            OnInserted(node); // Виклик хука при додаванні дитини
        }

        public override string InnerHTML => string.Join("", Children.Select(c => c.OuterHTML));

        // Властивість тепер просто викликає наш Шаблонний Метод
        public override string OuterHTML => Render();

        // ТЕЙ САМИЙ ШАБЛОННИЙ МЕТОД (Template Method)
        // Він жорстко задає алгоритм формування HTML
        private string Render()
        {
            string classes = "";
            if (CssClasses.Count > 0)
            {
                OnClassListApplied(); // Хук перед застосуванням класів
                classes = $" class=\"{string.Join(" ", CssClasses)}\"";
            }

            string result;
            if (_state.ClosingType == "single")
            {
                result = $"<{_state.TagName}{classes} />";
            }
            else
            {
                result = $"<{_state.TagName}{classes}>{InnerHTML}</{_state.TagName}>";
            }

            OnRendered(); // Хук після генерації розмітки
            return result;
        }

        // --- ХУКИ ЖИТТЄВОГО ЦИКЛУ (Lifecycle Hooks) ---
        protected virtual void OnCreated() { }
        protected virtual void OnInserted(LightNode node) { }
        protected virtual void OnClassListApplied() { }
        protected virtual void OnRendered() { }
    }

    // Конкретний елемент, який використовує хуки Шаблонного методу
    public class TrackedElementNode : LightElementNode
    {
        public TrackedElementNode(string tagName, string displayType, string closingType, List<string> cssClasses = null)
            : base(tagName, displayType, closingType, cssClasses)
        {
        }

        protected override void OnCreated()
        {
            Console.WriteLine($"[Hook] Елемент <{this.TagName}> було створено.");
        }

        protected override void OnInserted(LightNode node)
        {
            Console.WriteLine($"[Hook] У елемент додано нового нащадка.");
        }

        protected override void OnClassListApplied()
        {
            Console.WriteLine($"[Hook] До елемента застосовано {CssClasses.Count} CSS класів.");
        }

        protected override void OnRendered()
        {
            Console.WriteLine($"[Hook] Елемент успішно відрендерився у рядок.");
        }
    }

    public static class Task5_6Demo
    {
        public static void Run()
        {
            Console.WriteLine("=== Завдання 5: Компонувальник ===");

            var table = new LightElementNode("table", "block", "paired");
            var tr = new LightElementNode("tr", "block", "paired");
            var th1 = new LightElementNode("th", "inline", "paired", new List<string> { "header-cell" });
            th1.Add(new LightTextNode("Ім'я"));
            var th2 = new LightElementNode("th", "inline", "paired", new List<string> { "header-cell" });
            th2.Add(new LightTextNode("Вік"));

            tr.Add(th1);
            tr.Add(th2);
            table.Add(tr);

            Console.WriteLine(table.OuterHTML);

            Console.WriteLine("\n=== Завдання 6: Легковаговик ===");

            string[] bookLines = {
                "ACT V",
                "Scene I. Mantua. A Street.",
                "Scene II. Friar Lawrence's Cell.",
                "Scene III. A churchyard; in it a Monument belonging to the Capulets",
                "Dramatis Personæ",
                "ESCALUS, Prince of Verona.",
                "MERCUTIO, kinsman to the Prince, and friend to Romeo.",
                "PARIS, a young Nobleman, kinsman to the Prince.",
                " Page to Paris."
            };

            GC.Collect();
            long memoryBefore = GC.GetTotalMemory(true);

            var document = new LightElementNode("div", "block", "paired");

            for (int i = 0; i < 10000; i++)
            {
                bool isFirstLine = true;
                foreach (var line in bookLines)
                {
                    LightElementNode node;
                    if (isFirstLine)
                    {
                        node = new LightElementNode("h1", "block", "paired");
                        isFirstLine = false;
                    }
                    else if (line.StartsWith(" "))
                    {
                        node = new LightElementNode("blockquote", "block", "paired");
                    }
                    else if (line.Length < 20)
                    {
                        node = new LightElementNode("h2", "block", "paired");
                    }
                    else
                    {
                        node = new LightElementNode("p", "block", "paired");
                    }
                    node.Add(new LightTextNode(line));
                    document.Add(node);
                }
            }

            GC.Collect();
            long memoryAfter = GC.GetTotalMemory(true);

            Console.WriteLine($"Згенеровано вузлів: {document.Children.Count}");
            Console.WriteLine($"Унікальних станів (Flyweight) у пам'яті: {ElementStateFactory.StatesCount}");
            Console.WriteLine($"Використано пам'яті: {(memoryAfter - memoryBefore) / 1024.0 / 1024.0:F2} MB");
            
            
            Console.WriteLine("=== Перевірка Шаблонного методу (Хуки) ===");
            var trackedDiv = new TrackedElementNode("div", "block", "paired", new List<string> { "container", "active" });
            trackedDiv.Add(new LightTextNode("Текст всередині"));
            string html = trackedDiv.OuterHTML;
            Console.WriteLine($"\nРезультат: {html}\n");
        }
    }
}