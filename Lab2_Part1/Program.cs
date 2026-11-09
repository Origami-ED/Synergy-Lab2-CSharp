using System;
using System.Collections.Generic;

namespace Lab2_Part1
{
    public interface IMovable { void Move(int x, int y); }
    public class Point : IMovable
    {
        public int X { get; set; }
        public int Y { get; set; }
        public void Move(int x, int y)
        {
            X += x; Y += y;
            Console.WriteLine($"Точка перемещена в координаты: X={X}, Y={Y}");
        }
    }

    public interface IDrawable { void Draw(); }
    public class Circle : IDrawable { public void Draw() => Console.WriteLine("Рисуется круг (Circle)"); }
    public class Rectangle : IDrawable { public void Draw() => Console.WriteLine("Рисуется прямоугольник (Rectangle)"); }

    public interface IShare
    {
        double GetArea();
        double GetPerimeter();
    }
    public class CircleShape : IShare
    {
        public double Radius { get; set; } = 5;
        public double GetArea() => Math.PI * Radius * Radius;
        public double GetPerimeter() => 2 * Math.PI * Radius;
    }
    public class RectangleShape : IShare
    {
        public double Width { get; set; } = 4;
        public double Height { get; set; } = 6;
        public double GetArea() => Width * Height;
        public double GetPerimeter() => 2 * (Width + Height);
    }
    public interface I3DShare : IShare { double GetVolume(); }
    public class Cube : I3DShare
    {
        public double Side { get; set; } = 3;
        public double GetArea() => 6 * Side * Side;
        public double GetPerimeter() => 12 * Side;
        public double GetVolume() => Side * Side * Side;
    }

    public interface IPrinter { void Print(); }
    public interface IScanner { void Scan(); }
    public interface IFax { void Fax(); }
    public class Printer : IPrinter { public void Print() => Console.WriteLine("Принтер печатает..."); }
    public class Scanner : IScanner { public void Scan() => Console.WriteLine("Сканер сканирует..."); }
    public class MultifunctionDevice : IPrinter, IScanner, IFax
    {
        public void Print() => Console.WriteLine("МФУ печатает...");
        public void Scan() => Console.WriteLine("МФУ сканирует...");
        public void Fax() => Console.WriteLine("МФУ отправляет факс...");
    }

    public interface IPayable { void Pay(decimal amount); }
    public class CreditCard : IPayable { public void Pay(decimal amount) => Console.WriteLine($"Оплачено картой: {amount} руб."); }
    public class Cash : IPayable { public void Pay(decimal amount) => Console.WriteLine($"Оплачено наличными: {amount} руб."); }

    public interface ILogger { void Log(string message); }
    public class ConsoleLogger : ILogger { public void Log(string message) => Console.WriteLine($"[Консоль] {message}"); }
    public class FileLogger : ILogger { public void Log(string message) => Console.WriteLine($"[Файл] (Имитация записи в файл): {message}"); }

    class Program
    {
        static void ProcessPayment(IPayable method, decimal amount) => method.Pay(amount);
        static void DoWork(ILogger logger) => logger.Log("Выполнение какой-то работы...");
        static void PrintShapeInfo(IShare shape) => Console.WriteLine($"Площадь: {shape.GetArea():F2}, Периметр: {shape.GetPerimeter():F2}");

        static void Main(string[] args)
        {
            Console.WriteLine("--- Задание 1 ---");
            Point p = new Point();
            p.Move(10, 20);
            List<IDrawable> shapes = new List<IDrawable> { new Circle(), new Rectangle() };
            foreach (var s in shapes) s.Draw();

            Console.WriteLine("\n--- Задание 2 ---");
            PrintShapeInfo(new CircleShape());
            PrintShapeInfo(new RectangleShape());
            Cube cube = new Cube();
            Console.WriteLine($"Объем куба: {cube.GetVolume()}");

            Console.WriteLine("\n--- Задание 3 ---");
            MultifunctionDevice mfd = new MultifunctionDevice();
            mfd.Print(); mfd.Scan(); mfd.Fax();

            Console.WriteLine("\n--- Задание 4 ---");
            ProcessPayment(new CreditCard(), 1500);
            ProcessPayment(new Cash(), 500);
            DoWork(new ConsoleLogger());
            DoWork(new FileLogger());
        }
    }
}