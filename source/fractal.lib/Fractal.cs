using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using static System.Console;

namespace fractal.lib
{
    using PointUnitType = System.Double;
    public class Point
    {
        public Point()
        {
            X = default(PointUnitType);
            Y = default(PointUnitType);
        }
        public Point(Point p)
        {
            X = p.X;
            Y = p.Y;
        }
        public PointUnitType X, Y;
    }
    public class Segment { public Point A, B; }
    public class Fractal
    {
        public static IEnumerable<Segment> make_line(PointUnitType x1, PointUnitType y1, PointUnitType x2, PointUnitType y2, int level = 0)
        {
            switch (level)
            {
                case 0: return new List<Segment> { new Segment { A = new Point { X = x1, Y = y1 }, B = new Point { X = x2, Y = y2 } } };
                case 1:
                {
                    var result = new List<Segment>();
                    var whole_distance = Math.Sqrt(Math.Pow(x1 - x2, 2D) + Math.Pow(y1 - y2, 2D));
                    var segment_unit_size = whole_distance / 4D;

                    var a = new Point { X = x1, Y = y1 };
                    var b = new Point { X = x1 + segment_unit_size, Y = y1 };
                    result.Add(new Segment { A = new Point(a), B = new Point(b) });

                    a = new Point(b);
                    b.Y += segment_unit_size;
                    result.Add(new Segment { A = new Point(a), B = new Point(b) });

                    a = new Point(b);
                    b.X += segment_unit_size;
                    result.Add(new Segment { A = new Point(a), B = new Point(b) });

                    a = new Point(b);
                    b.Y -= segment_unit_size * 2;
                    result.Add(new Segment { A = new Point(a), B = new Point(b) });

                    a = new Point(b);
                    b.X += segment_unit_size;
                    result.Add(new Segment { A = new Point(a), B = new Point(b) });

                    a = new Point(b);
                    b.Y += segment_unit_size;
                    result.Add(new Segment { A = new Point(a), B = new Point(b) });

                    a = new Point(b);
                    b.X = x2;
                    result.Add(new Segment { A = new Point(a), B = new Point(b) });

                    return result;
                }
                case 2:
                {
                    var result = new List<Segment>();
                    var whole_distance = Math.Sqrt(Math.Pow(x1 - x2, 2D) + Math.Pow(y1 - y2, 2D));
                    var segment_unit_size = whole_distance / 4D;

                    var a = new Point { X = x1, Y = y1 };
                    var b = new Point { X = x1, Y = y1 + segment_unit_size };
                    result.Add(new Segment { A = new Point(a), B = new Point(b) });

                    a = new Point(b);
                    b.X -= segment_unit_size;
                    result.Add(new Segment { A = new Point(a), B = new Point(b) });

                    a = new Point(b);
                    b.Y += segment_unit_size;
                    result.Add(new Segment { A = new Point(a), B = new Point(b) });

                    a = new Point(b);
                    b.X += segment_unit_size * 2;
                    result.Add(new Segment { A = new Point(a), B = new Point(b) });

                    a = new Point(b);
                    b.Y += segment_unit_size;
                    result.Add(new Segment { A = new Point(a), B = new Point(b) });

                    a = new Point(b);
                    b.X -= segment_unit_size;
                    result.Add(new Segment { A = new Point(a), B = new Point(b) });

                    a = new Point(b);
                    b.Y = y2;
                    result.Add(new Segment { A = new Point(a), B = new Point(b) });

                    return result;
                }
                case 3:
                {
                    var result = new List<Segment>();
                    var whole_distance = Math.Sqrt(Math.Pow(x1 - x2, 2D) + Math.Pow(y1 - y2, 2D));
                    var segment_unit_size = whole_distance / 4D;

                    var a = new Point { X = x1, Y = y1 };
                    var b = new Point { X = x1 - segment_unit_size, Y = y1 };
                    result.Add(new Segment { A = new Point(a), B = new Point(b) });

                    a = new Point(b);
                    b.Y -= segment_unit_size;
                    result.Add(new Segment { A = new Point(a), B = new Point(b) });

                    a = new Point(b);
                    b.X -= segment_unit_size;
                    result.Add(new Segment { A = new Point(a), B = new Point(b) });

                    a = new Point(b);
                    b.Y += segment_unit_size * 2;
                    result.Add(new Segment { A = new Point(a), B = new Point(b) });

                    a = new Point(b);
                    b.X -= segment_unit_size;
                    result.Add(new Segment { A = new Point(a), B = new Point(b) });

                    a = new Point(b);
                    b.Y -= segment_unit_size;
                    result.Add(new Segment { A = new Point(a), B = new Point(b) });

                    a = new Point(b);
                    b.X = x2;
                    result.Add(new Segment { A = new Point(a), B = new Point(b) });

                    return result;
                }
                case 4:
                {
                    var result = new List<Segment>();
                    var whole_distance = Math.Sqrt(Math.Pow(x1 - x2, 2D) + Math.Pow(y1 - y2, 2D));
                    var segment_unit_size = whole_distance / 4D;

                    var a = new Point { X = x1, Y = y1 };
                    var b = new Point { X = x1, Y = y1 - segment_unit_size };
                    result.Add(new Segment { A = new Point(a), B = new Point(b) });

                    a = new Point(b);
                    b.X += segment_unit_size;
                    result.Add(new Segment { A = new Point(a), B = new Point(b) });

                    a = new Point(b);
                    b.Y -= segment_unit_size;
                    result.Add(new Segment { A = new Point(a), B = new Point(b) });

                    a = new Point(b);
                    b.X -= segment_unit_size * 2;
                    result.Add(new Segment { A = new Point(a), B = new Point(b) });

                    a = new Point(b);
                    b.Y -= segment_unit_size;
                    result.Add(new Segment { A = new Point(a), B = new Point(b) });

                    a = new Point(b);
                    b.X += segment_unit_size;
                    result.Add(new Segment { A = new Point(a), B = new Point(b) });

                    a = new Point(b);
                    b.Y = y2;
                    result.Add(new Segment { A = new Point(a), B = new Point(b) });

                    return result;
                }
                default: throw new Exception($"unsupported {level}");
            }
        }
    }
    public class Plane
    {
        public Plane(PointUnitType width, PointUnitType height)
        {
            Width = width;
            Height = height;
            Origin_API = new Point { X = width / 2F, Y = height / 2F };
        }

        public PointUnitType Width, Height;
        public Point Origin_API;

        //https://docs.microsoft.com/en-us/dotnet/framework/winforms/advanced/types-of-coordinate-systems
        public Point ToAPI(Point a) => new Point { X = Origin_API.X + a.X, Y = Origin_API.Y - a.Y };
        public (T X, T Y) ToAPI<T>(Point a) => ((T)Convert.ChangeType(Origin_API.X + a.X, typeof(T)), (T)Convert.ChangeType(Origin_API.Y - a.Y, typeof(T)));

    }
    public class Draw
    {
        //https://devblogs.microsoft.com/dotnet/net-core-image-processing/
        public static System.Drawing.Image CreateImage(IEnumerable<Segment> line, int width, int height)
        {
            var result = new System.Drawing.Bitmap((int)width, (int)height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            var g = System.Drawing.Graphics.FromImage(result);
            var plane = new Plane(width, height);
            foreach (var segment in line)
            {
                var a = plane.ToAPI<float>(segment.A);
                var b = plane.ToAPI<float>(segment.B);
                g.DrawLine(System.Drawing.Pens.Blue, a.X, a.Y, b.X, b.Y);
            }
            return result;
        }
    }

    public class Input
    {
        public FileInfo file;
        public void fractal()
        {
            IEnumerable<Segment> line = Fractal.make_line(0F, 0F, 0F, -100F, 4);
            using var image = Draw.CreateImage(line, 200, 200);
            image.Save(file?.FullName, System.Drawing.Imaging.ImageFormat.Jpeg);
        }
    }
    public class CLI
    {
        public static void MainEntryPoint(string[] args, string runtime_name)
        {
            try
            {
                var runtime = $"{runtime_name}/{System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription}";
                if (isThereUI())
                {
                    WriteLine($"Hello, {Environment.UserDomainName}\\{Environment.UserName} !!!");
                    WriteLine($"PID: {System.Diagnostics.Process.GetCurrentProcess().Id} | Thread: {System.Threading.Thread.CurrentThread.ManagedThreadId} | Runtime: {runtime} | Culture: {System.Threading.Thread.CurrentThread?.CurrentUICulture?.Name}, {System.Threading.Thread.CurrentThread?.CurrentCulture?.Name}\n");
                }
                if (!(args?.Any() == true) || args?.First().Contains("?") == true)
                {
                    WriteLine($"Working with {GetHostProcessName()}:");
                    nutility.Switch.ShowUsage(typeof(Input));
                }
                else
                {
                    nutility.Switch.AsType<Input>(args);
                }
            }
            catch (Exception ex)
            {
                for (int level = 0; ex != null; ex = ex.InnerException, ++level)
                {
                    WriteLine($"\r\n[Level {level}] {ex.GetType().FullName}: {ex.Message} {ex.StackTrace}");
                }
            }

            string GetHostProcessName()
            {
                var result = Environment.GetCommandLineArgs()?.FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(result))
                {
                    result = System.IO.Path.GetFileNameWithoutExtension(result);
                }
                return result;
            }
            bool isThereUI() =>
                !Console.IsInputRedirected;
            //Console.OpenStandardInput(1) != System.IO.Stream.Null;
            //Console.In != System.IO.StreamReader.Null;
            //TODO required further test design on Environment.UserInteractive;
        }
    }
}