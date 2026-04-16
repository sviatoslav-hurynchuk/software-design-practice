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

        // Iterator factory methods
        public IEnumerator<LightNode> GetDepthFirstIterator() => new DepthFirstIterator(this);
        public IEnumerator<LightNode> GetBreadthFirstIterator() => new BreadthFirstIterator(this);
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

            OnCreated();
        }

        public void Add(LightNode node)
        {
            Children.Add(node);
            OnInserted(node);
        }

        public override string InnerHTML => string.Join("", Children.Select(c => c.OuterHTML));
        public override string OuterHTML => Render();

        // Template Method defining the skeleton of rendering
        private string Render()
        {
            string classes = "";
            if (CssClasses.Count > 0)
            {
                OnClassListApplied();
                classes = $" class=\"{string.Join(" ", CssClasses)}\"";
            }

            string result = _state.ClosingType == "single"
                ? $"<{_state.TagName}{classes} />"
                : $"<{_state.TagName}{classes}>{InnerHTML}</{_state.TagName}>";

            OnRendered();
            return result;
        }

        // Lifecycle Hooks
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

    // DFS Iterator
    public class DepthFirstIterator : IEnumerator<LightNode>
    {
        private readonly LightNode _root;
        private Stack<LightNode> _stack;

        public LightNode Current { get; private set; }
        object System.Collections.IEnumerator.Current => Current;

        public DepthFirstIterator(LightNode root)
        {
            _root = root;
            Reset();
        }

        public bool MoveNext()
        {
            if (_stack.Count == 0) return false;

            Current = _stack.Pop();

            if (Current is LightElementNode elementNode)
            {
                for (int i = elementNode.Children.Count - 1; i >= 0; i--)
                {
                    _stack.Push(elementNode.Children[i]);
                }
            }
            return true;
        }

        public void Reset()
        {
            _stack = new Stack<LightNode>();
            _stack.Push(_root);
            Current = null;
        }

        public void Dispose() { }
    }

    // BFS Iterator
    public class BreadthFirstIterator : IEnumerator<LightNode>
    {
        private readonly LightNode _root;
        private Queue<LightNode> _queue;

        public LightNode Current { get; private set; }
        object System.Collections.IEnumerator.Current => Current;

        public BreadthFirstIterator(LightNode root)
        {
            _root = root;
            Reset();
        }

        public bool MoveNext()
        {
            if (_queue.Count == 0) return false;

            Current = _queue.Dequeue();

            if (Current is LightElementNode elementNode)
            {
                foreach (var child in elementNode.Children)
                {
                    _queue.Enqueue(child);
                }
            }
            return true;
        }

        public void Reset()
        {
            _queue = new Queue<LightNode>();
            _queue.Enqueue(_root);
            Current = null;
        }

        public void Dispose() { }
    }

    // Demo execution
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

            Console.WriteLine("\n=== Template Method (Hooks) ===");
            var trackedDiv = new TrackedElementNode("div", "block", "paired", new List<string> { "container", "active" });
            trackedDiv.Add(new LightTextNode("Inner text"));
            string html = trackedDiv.OuterHTML;
            Console.WriteLine($"\nResult:\n{html}\n");

            Console.WriteLine("=== Iterators (DFS & BFS) ===");
            var htmlDoc = new LightElementNode("html", "block", "paired");
            var head = new LightElementNode("head", "block", "paired");
            var body = new LightElementNode("body", "block", "paired");
            htmlDoc.Add(head);
            htmlDoc.Add(body);
            head.Add(new LightElementNode("title", "inline", "paired"));
            body.Add(new LightElementNode("h1", "block", "paired"));
            body.Add(new LightElementNode("p", "block", "paired"));

            Console.WriteLine("--- Depth-First Search ---");
            var dfs = htmlDoc.GetDepthFirstIterator();
            while (dfs.MoveNext())
            {
                if (dfs.Current is LightElementNode el) Console.WriteLine($"Tag: <{el.TagName}>");
            }

            Console.WriteLine("\n--- Breadth-First Search ---");
            var bfs = htmlDoc.GetBreadthFirstIterator();
            while (bfs.MoveNext())
            {
                if (bfs.Current is LightElementNode el) Console.WriteLine($"Tag: <{el.TagName}>");
            }
        }
    }
}