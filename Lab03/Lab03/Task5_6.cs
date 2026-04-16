using System;
using System.Collections.Generic;
using System.Linq;

namespace Lab3.Task5_6
{
    // State Interface
    public interface IRenderState
    {
        string Render(LightElementNode element, string innerHtml);
    }

    // Concrete State: Normal rendering
    public class NormalRenderState : IRenderState
    {
        public string Render(LightElementNode element, string innerHtml)
        {
            string classes = element.CssClasses.Count > 0
                ? $" class=\"{string.Join(" ", element.CssClasses)}\""
                : "";

            if (element.GetClosingType() == "single")
            {
                return $"<{element.TagName}{classes} />";
            }
            return $"<{element.TagName}{classes}>{innerHtml}</{element.TagName}>";
        }
    }

    // Concrete State: Hidden rendering
    public class HiddenRenderState : IRenderState
    {
        public string Render(LightElementNode element, string innerHtml)
        {
            string classes = element.CssClasses.Count > 0
                ? $" class=\"{string.Join(" ", element.CssClasses)}\""
                : "";

            string style = " style=\"display: none;\"";

            if (element.GetClosingType() == "single")
            {
                return $"<{element.TagName}{classes}{style} />";
            }
            return $"<{element.TagName}{classes}{style}>{innerHtml}</{element.TagName}>";
        }
    }

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

        public LightTextNode(string text)
        {
            _text = text;
        }

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

    // Composite Component
    public class LightElementNode : LightNode
    {
        private readonly ElementState _state;

        // Exposed properties for State access
        public string TagName => _state.TagName;
        public string GetClosingType() => _state.ClosingType;

        public List<string> CssClasses { get; }
        public List<LightNode> Children { get; }

        private IRenderState _renderState;

        public LightElementNode(string tagName, string displayType, string closingType, List<string> cssClasses = null)
        {
            _state = ElementStateFactory.GetState(tagName, displayType, closingType);
            CssClasses = cssClasses ?? new List<string>();
            Children = new List<LightNode>();

            // Set initial state
            _renderState = new NormalRenderState();
        }

        // Method to change state
        public void SetRenderState(IRenderState newState)
        {
            _renderState = newState;
        }

        public void Add(LightNode node)
        {
            Children.Add(node);
        }

        public override string InnerHTML => string.Join("", Children.Select(c => c.OuterHTML));

        // Delegate rendering to the current state
        public override string OuterHTML => _renderState.Render(this, InnerHTML);
    }

    // Demo execution
    public static class Task5_6Demo
    {
        public static void Run()
        {
            Console.WriteLine("=== Task 5: Composite ===");

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

            Console.WriteLine("\n=== Task 6: Flyweight ===");

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

            Console.WriteLine("\n=== State Pattern Test ===");

            var stateDiv = new LightElementNode("div", "block", "paired", new List<string> { "alert-box" });
            stateDiv.Add(new LightTextNode("Important message!"));

            Console.WriteLine("--- Normal State ---");
            Console.WriteLine(stateDiv.OuterHTML);

            Console.WriteLine("\n--- Changed to Hidden State ---");
            stateDiv.SetRenderState(new HiddenRenderState());
            Console.WriteLine(stateDiv.OuterHTML);

            Console.WriteLine("\n--- Reverted to Normal State ---");
            stateDiv.SetRenderState(new NormalRenderState());
            Console.WriteLine(stateDiv.OuterHTML);
        }
    }
}