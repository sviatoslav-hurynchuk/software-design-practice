using System;
using System.Collections.Generic;
using System.Linq;

namespace Lab3.Task5_6
{
    // Base Component
    public abstract class LightNode
    {
        public abstract string OuterHTML { get; }
        public abstract string InnerHTML { get; }
    }

    // Leaf Component
    public class LightTextNode : LightNode
    {
        private readonly string _text;

        public LightTextNode(string text) => _text = text;

        public override string OuterHTML => _text;
        public override string InnerHTML => _text;
    }

    // Flyweight State
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

    // Flyweight Factory
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

    // Composite Component + Template Method
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

            OnCreated(); // Hook call on creation
        }

        public void Add(LightNode node)
        {
            Children.Add(node);
            OnInserted(node); // Hook call on child insertion
        }

        public override string InnerHTML => string.Join("", Children.Select(c => c.OuterHTML));

        // Property now calls the Template Method
        public override string OuterHTML => Render();

        // Template Method defining the skeleton of rendering
        private string Render()
        {
            string classes = "";
            if (CssClasses.Count > 0)
            {
                OnClassListApplied(); // Hook call before classes are applied
                classes = $" class=\"{string.Join(" ", CssClasses)}\"";
            }

            string result = _state.ClosingType == "single"
                ? $"<{_state.TagName}{classes} />"
                : $"<{_state.TagName}{classes}>{InnerHTML}</{_state.TagName}>";

            OnRendered(); // Hook call after rendering
            return result;
        }

        // --- Lifecycle Hooks ---
        protected virtual void OnCreated() { }
        protected virtual void OnInserted(LightNode node) { }
        protected virtual void OnClassListApplied() { }
        protected virtual void OnRendered() { }
    }

    // Concrete element testing Template Method hooks
    public class TrackedElementNode : LightElementNode
    {
        public TrackedElementNode(string tagName, string displayType, string closingType, List<string> cssClasses = null)
            : base(tagName, displayType, closingType, cssClasses) { }

        protected override void OnCreated() => Console.WriteLine($"[Hook] Created <{TagName}>");
        protected override void OnInserted(LightNode node) => Console.WriteLine($"[Hook] Child inserted into <{TagName}>");
        protected override void OnClassListApplied() => Console.WriteLine($"[Hook] Applied {CssClasses.Count} classes to <{TagName}>");
        protected override void OnRendered() => Console.WriteLine($"[Hook] Rendered <{TagName}>");
    }

    public static class Task5_6Demo
    {
        public static void Run()
        {
            Console.WriteLine("=== Task 5 & 6: Composite & Flyweight ===");

            var table = new LightElementNode("table", "block", "paired");
            var tr = new LightElementNode("tr", "block", "paired");
            var th1 = new LightElementNode("th", "inline", "paired", new List<string> { "header-cell" });
            th1.Add(new LightTextNode("Name"));
            var th2 = new LightElementNode("th", "inline", "paired", new List<string> { "header-cell" });
            th2.Add(new LightTextNode("Age"));

            tr.Add(th1);
            tr.Add(th2);
            table.Add(tr);

            Console.WriteLine(table.OuterHTML);

            Console.WriteLine("\n=== Flyweight Performance Test ===");

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

            Console.WriteLine($"Generated nodes: {document.Children.Count}");
            Console.WriteLine($"Unique states (Flyweight) in memory: {ElementStateFactory.StatesCount}");
            Console.WriteLine($"Memory used: {(memoryAfter - memoryBefore) / 1024.0 / 1024.0:F2} MB");

            Console.WriteLine("\n=== Template Method (Hooks) Test ===");
            var trackedDiv = new TrackedElementNode("div", "block", "paired", new List<string> { "container", "active" });
            trackedDiv.Add(new LightTextNode("Inner text"));
            string html = trackedDiv.OuterHTML;
            Console.WriteLine($"\nResult:\n{html}\n");
        }
    }
}