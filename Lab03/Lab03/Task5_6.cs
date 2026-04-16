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

    // Command Pattern Interface
    public interface ICommand
    {
        void Execute();
        void Undo();
    }

    // Concrete command for inserting a node
    public class InsertNodeCommand : ICommand
    {
        private readonly LightElementNode _parent;
        private readonly LightNode _child;

        public InsertNodeCommand(LightElementNode parent, LightNode child)
        {
            _parent = parent;
            _child = child;
        }

        public void Execute()
        {
            _parent.Add(_child);
            Console.WriteLine($"[Command] Executed: added node to <{_parent.TagName}>");
        }

        public void Undo()
        {
            _parent.Remove(_child);
            Console.WriteLine($"[Command] Undo: removed node from <{_parent.TagName}>");
        }
    }

    // Command Invoker for history management
    public class CommandInvoker
    {
        private readonly Stack<ICommand> _history = new Stack<ICommand>();

        public void ExecuteCommand(ICommand command)
        {
            command.Execute();
            _history.Push(command);
        }

        public void UndoLastCommand()
        {
            if (_history.Count > 0)
            {
                var command = _history.Pop();
                command.Undo();
            }
            else
            {
                Console.WriteLine("[Command] No actions to undo.");
            }
        }
    }

    // Composite Component
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
        }

        public void Add(LightNode node)
        {
            Children.Add(node);
        }

        // Added for Command Pattern Undo
        public void Remove(LightNode node)
        {
            if (Children.Contains(node))
            {
                Children.Remove(node);
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
            th1.Add(new LightTextNode("Ім'я"));
            var th2 = new LightElementNode("th", "inline", "paired", new List<string> { "header-cell" });
            th2.Add(new LightTextNode("Вік"));

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

            Console.WriteLine($"Згенеровано вузлів: {document.Children.Count}");
            Console.WriteLine($"Унікальних станів (Flyweight) у пам'яті: {ElementStateFactory.StatesCount}");
            Console.WriteLine($"Використано пам'яті: {(memoryAfter - memoryBefore) / 1024.0 / 1024.0:F2} MB");

            Console.WriteLine("\n=== Command Pattern Test (Undo/Redo) ===");

            var commandDoc = new LightElementNode("html", "block", "paired");
            var invoker = new CommandInvoker();

            var newDiv = new LightElementNode("div", "block", "paired");
            newDiv.Add(new LightTextNode("Test DIV"));

            var newP = new LightElementNode("p", "block", "paired");
            newP.Add(new LightTextNode("Test paragraph"));

            invoker.ExecuteCommand(new InsertNodeCommand(commandDoc, newDiv));
            invoker.ExecuteCommand(new InsertNodeCommand(commandDoc, newP));

            Console.WriteLine("\nHTML state after commands execution:");
            Console.WriteLine(commandDoc.OuterHTML);

            Console.WriteLine("\n--- Triggering Undo (reverting last action) ---");
            invoker.UndoLastCommand();

            Console.WriteLine("\nHTML state after Undo (paragraph should be gone):");
            Console.WriteLine(commandDoc.OuterHTML);
        }
    }
}