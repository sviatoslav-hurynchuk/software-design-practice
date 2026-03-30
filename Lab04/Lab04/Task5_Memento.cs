using System;
using System.Collections.Generic;

namespace Lab04
{
    public class TextDocumentMemento
    {
        public string Content { get; }

        public TextDocumentMemento(string content)
        {
            Content = content;
        }
    }

    public class TextDocument
    {
        public string Content { get; set; } = string.Empty;

        public void Write(string text)
        {
            Content += text;
        }

        public TextDocumentMemento Save()
        {
            return new TextDocumentMemento(Content);
        }

        public void Restore(TextDocumentMemento memento)
        {
            Content = memento.Content;
        }
    }

    public class TextEditor
    {
        private readonly TextDocument _document;
        private readonly Stack<TextDocumentMemento> _history;

        public TextEditor(TextDocument document)
        {
            _document = document;
            _history = new Stack<TextDocumentMemento>();
        }

        public void Write(string text)
        {
            _document.Write(text);
        }

        public void Print()
        {
            Console.WriteLine($"Поточний текст: '{_document.Content}'");
        }

        public void Save()
        {
            _history.Push(_document.Save());
            Console.WriteLine("[Система]: Стан збережено.");
        }

        public void Undo()
        {
            if (_history.Count > 0)
            {
                var memento = _history.Pop();
                _document.Restore(memento);
                Console.WriteLine("[Система]: Останню зміну скасовано (Ctrl+Z).");
            }
            else
            {
                Console.WriteLine("[Система]: Історія порожня, немає чого скасовувати.");
            }
        }
    }

    public static class Task5Demo
    {
        public static void Run()
        {
            Console.WriteLine("\n=== Завдання 5: Мементо ===");

            var document = new TextDocument();
            var editor = new TextEditor(document);

            editor.Write("Привіт, світ! ");
            editor.Print();
            editor.Save(); 

            editor.Write("Це новий рядок. ");
            editor.Print();
            editor.Save();

            editor.Write("А це випадково написаний текст (помилка)!");
            editor.Print();

            Console.WriteLine("\n--- Користувач натискає Undo ---");
            editor.Undo();
            editor.Print();

            Console.WriteLine("\n--- Користувач ще раз натискає Undo ---");
            editor.Undo();
            editor.Print();
        }
    }
}