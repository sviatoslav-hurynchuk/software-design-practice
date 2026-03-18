using Lab3.Task3;
using System;

namespace Lab3.Task3
{
    public interface IRenderer
    {
        void Render(string shapeName);
    }

    public class VectorRenderer : IRenderer
    {
        public void Render(string shapeName)
        {
            Console.WriteLine($"Drawing {shapeName} as lines (Vector)");
        }
    }

    public class RasterRenderer : IRenderer
    {
        public void Render(string shapeName)
        {
            Console.WriteLine($"Drawing {shapeName} as pixels (Raster)");
        }
    }

    public abstract class Shape
    {
        protected IRenderer _renderer; // міст

        public Shape(IRenderer renderer)
        {
            _renderer = renderer;
        }

        public abstract void Draw();
    }

    // конкретні фігури
    public class Circle : Shape
    {
        public Circle(IRenderer renderer) : base(renderer) { }
        public override void Draw() => _renderer.Render("Circle");
    }

    public class Square : Shape
    {
        public Square(IRenderer renderer) : base(renderer) { }
        public override void Draw() => _renderer.Render("Square");
    }

    public class Triangle : Shape
    {
        public Triangle(IRenderer renderer) : base(renderer) { }
        public override void Draw() => _renderer.Render("Triangle");
    }

    public static class Task3Demo
    {
        public static void Run()
        {
            Console.WriteLine("=== Завдання 3: Міст ===");

            IRenderer vectorRenderer = new VectorRenderer();
            IRenderer rasterRenderer = new RasterRenderer();

            Shape circle = new Circle(vectorRenderer);
            Shape square = new Square(rasterRenderer);
            Shape triangle = new Triangle(rasterRenderer);

            circle.Draw();
            square.Draw();
            triangle.Draw();

            Console.WriteLine();
        }
    }
}