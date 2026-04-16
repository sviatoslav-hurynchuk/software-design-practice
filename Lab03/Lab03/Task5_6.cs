using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lab3.Task5_6
{
    // Visitor Interface
    public interface IVisitor
    {
        void Visit(LightElementNode element);
        void Visit(LightTextNode text);
    }

    // Concrete Visitor 1: Extracts pure text without tags
    public class PlainTextVisitor : IVisitor
    {
        private readonly StringBuilder _textBuilder = new StringBuilder();

        public string GetPlainText() => _textBuilder.ToString().Trim();

        public void Visit(LightElementNode element)
        {
            if (element.TagName == "p" || element.TagName == "h1" || element.TagName == "h2")
            {
                _textBuilder.AppendLine();
            }
        }

        public void Visit(LightTextNode text)
        {
            _textBuilder.Append(text.InnerHTML).Append(" ");
        }
    }

    // Concrete Visitor 2: Counts specific tags
    public class TagCountVisitor : IVisitor
    {
        private readonly string _targetTag;
        public int Count { get; private set; }

        public TagCountVisitor(string targetTag)
        {
            _targetTag = targetTag;
            Count = 0;
        }

        public void Visit(LightElementNode element)
        {
            if (element.TagName == _targetTag)
            {
                Count++;
            }
        }

        public void Visit(LightTextNode text) { }
    }

    // Base Component
    public abstract class LightNode
    {
        public abstract string OuterHTML { get; }
        public abstract string InnerHTML { get; }

        // Added for Visitor Pattern
        public abstract void Accept(IVisitor visitor);
    }

    // Leaf Component
    public class LightTextNode : LightNode
    {
        private readonly string _text;

        public LightTextNode(string text) => _text = text;

        public override string OuterHTML => _text;
        public override string InnerHTML => _text;

        // Added for Visitor Pattern
        public override void Accept(IVisitor visitor) => visitor.Visit(this);
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
        public string TagName => _state.TagName; // Exposed for visitors
        public List<string> CssClasses { get; }
        public List<LightNode> Children { get; }

        public LightElementNode(string tagName, string displayType, string closingType, List<string> cssClasses = null)
        {
            _state = ElementStateFactory.GetState(tagName, displayType, closingType);
            CssClasses = cssClasses ?? new List<string>();
            Children = new List<LightNode>();
        }

        public void Add(LightNode node) => Children.Add(node);

        // Added for Visitor Pattern
        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
            foreach (var child in Children)
            {
                child.Accept(visitor);
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

            Console.WriteLine("\n=== Visitor Pattern Test ===");

            var visitorDoc = new LightElementNode("html", "block", "paired");
            var vBody = new LightElementNode("body", "block", "paired");
            var vH1 = new LightElementNode("h1", "block", "paired");
            vH1.Add(new LightTextNode("Title of the page"));

            var vDiv = new LightElementNode("div", "block", "paired");
            var vP1 = new LightElementNode("p", "block", "paired");
            vP1.Add(new LightTextNode("First paragraph text."));
            var vP2 = new LightElementNode("p", "block", "paired");
            vP2.Add(new LightTextNode("Second paragraph text."));

            vDiv.Add(vP1);
            vDiv.Add(vP2);
            vBody.Add(vH1);
            vBody.Add(vDiv);
            visitorDoc.Add(vBody);

            // Test 1: Plain Text Extraction
            var textVisitor = new PlainTextVisitor();
            visitorDoc.Accept(textVisitor);
            Console.WriteLine("--- Plain Text Extracted ---");
            Console.WriteLine(textVisitor.GetPlainText());

            // Test 2: Tag Counting
            var pCounter = new TagCountVisitor("p");
            var divCounter = new TagCountVisitor("div");
            visitorDoc.Accept(pCounter);
            visitorDoc.Accept(divCounter);

            Console.WriteLine("\n--- Tag Counts ---");
            Console.WriteLine($"Paragraphs (<p>) found: {pCounter.Count}");
            Console.WriteLine($"Divs (<div>) found: {divCounter.Count}");
        }
    }
}