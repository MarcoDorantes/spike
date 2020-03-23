using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using static System.Console;

namespace fractal.lib
{
  using PointUnit = System.Double;
  public class Point
  {
    public Point()
    {
      X = default(PointUnit);
      Y = default(PointUnit);
    }
    public Point(Point p)
    {
      X = p.X;
      Y = p.Y;
    }
    public PointUnit X, Y;
  }
  public abstract class Segment { public Point A, B; public abstract void Draw(Action<Point, Point> draw); }
  public class SingleSegment : Segment { public override void Draw(Action<Point, Point> draw) => draw?.Invoke(A, B); }
  public class RecursiveSegment : Segment
  {
    public IEnumerable<Segment> Line;
    public override void Draw(Action<Point, Point> draw)
    {
      if (Line?.Count() > 0)
      {
        foreach (var segment in Line)
        {
          segment.Draw(draw);
        }
      }
      else draw?.Invoke(A, B);
    }
  }
  public enum SlopeCase { Unsupported, HorizontalRight, VerticalUp, HorizontalLeft, VerticalDown }
  public class Fractal
  {
    public static IEnumerable<Segment> make_line(PointUnit x1, PointUnit y1, PointUnit x2, PointUnit y2, int level = 1)
    {
      if (level == 0) return new List<Segment> { new SingleSegment { A = new Point { X = x1, Y = y1 }, B = new Point { X = x2, Y = y2 } } };

      var result = new List<Segment>();
      var whole_distance = Math.Sqrt(Math.Pow(x1 - x2, 2D) + Math.Pow(y1 - y2, 2D));
      var segment_unit_size = whole_distance / 4D;
      var slope = Plane.GetSlopeCase(x1, y1, x2, y2);
      switch (slope.Case)
      {
        case SlopeCase.HorizontalRight:
        {
          var a = new Point { X = x1, Y = y1 };
          var b = new Point { X = x1 + segment_unit_size, Y = y1 };
          result.Add(new RecursiveSegment { A = new Point(a), B = new Point(b), Line = make_line(a.X, a.Y, b.X, b.Y, level - 1) });

          a = new Point(b);
          b.Y += segment_unit_size;
          result.Add(new RecursiveSegment { A = new Point(a), B = new Point(b), Line = make_line(a.X, a.Y, b.X, b.Y, level - 1) });

          a = new Point(b);
          b.X += segment_unit_size;
          result.Add(new RecursiveSegment { A = new Point(a), B = new Point(b), Line = make_line(a.X, a.Y, b.X, b.Y, level - 1) });

          a = new Point(b);
          b.Y -= segment_unit_size * 2;
          result.Add(new RecursiveSegment { A = new Point(a), B = new Point(b), Line = make_line(a.X, a.Y, b.X, b.Y, level - 1) });

          a = new Point(b);
          b.X += segment_unit_size;
          result.Add(new RecursiveSegment { A = new Point(a), B = new Point(b), Line = make_line(a.X, a.Y, b.X, b.Y, level - 1) });

          a = new Point(b);
          b.Y += segment_unit_size;
          result.Add(new RecursiveSegment { A = new Point(a), B = new Point(b), Line = make_line(a.X, a.Y, b.X, b.Y, level - 1) });

          a = new Point(b);
          b.X = x2;
          result.Add(new RecursiveSegment { A = new Point(a), B = new Point(b), Line = make_line(a.X, a.Y, b.X, b.Y, level - 1) });

          return result;
        }
        case SlopeCase.VerticalUp:
        {
          var a = new Point { X = x1, Y = y1 };
          var b = new Point { X = x1, Y = y1 + segment_unit_size };
          result.Add(new RecursiveSegment { A = new Point(a), B = new Point(b), Line = make_line(a.X, a.Y, b.X, b.Y, level - 1) });

          a = new Point(b);
          b.X -= segment_unit_size;
          result.Add(new RecursiveSegment { A = new Point(a), B = new Point(b), Line = make_line(a.X, a.Y, b.X, b.Y, level - 1) });

          a = new Point(b);
          b.Y += segment_unit_size;
          result.Add(new RecursiveSegment { A = new Point(a), B = new Point(b), Line = make_line(a.X, a.Y, b.X, b.Y, level - 1) });

          a = new Point(b);
          b.X += segment_unit_size * 2;
          result.Add(new RecursiveSegment { A = new Point(a), B = new Point(b), Line = make_line(a.X, a.Y, b.X, b.Y, level - 1) });

          a = new Point(b);
          b.Y += segment_unit_size;
          result.Add(new RecursiveSegment { A = new Point(a), B = new Point(b), Line = make_line(a.X, a.Y, b.X, b.Y, level - 1) });

          a = new Point(b);
          b.X -= segment_unit_size;
          result.Add(new RecursiveSegment { A = new Point(a), B = new Point(b), Line = make_line(a.X, a.Y, b.X, b.Y, level - 1) });

          a = new Point(b);
          b.Y = y2;
          result.Add(new RecursiveSegment { A = new Point(a), B = new Point(b), Line = make_line(a.X, a.Y, b.X, b.Y, level - 1) });

          return result;
        }
        case SlopeCase.HorizontalLeft:
        {
          var a = new Point { X = x1, Y = y1 };
          var b = new Point { X = x1 - segment_unit_size, Y = y1 };
          result.Add(new RecursiveSegment { A = new Point(a), B = new Point(b), Line = make_line(a.X, a.Y, b.X, b.Y, level - 1) });

          a = new Point(b);
          b.Y -= segment_unit_size;
          result.Add(new RecursiveSegment { A = new Point(a), B = new Point(b), Line = make_line(a.X, a.Y, b.X, b.Y, level - 1) });

          a = new Point(b);
          b.X -= segment_unit_size;
          result.Add(new RecursiveSegment { A = new Point(a), B = new Point(b), Line = make_line(a.X, a.Y, b.X, b.Y, level - 1) });

          a = new Point(b);
          b.Y += segment_unit_size * 2;
          result.Add(new RecursiveSegment { A = new Point(a), B = new Point(b), Line = make_line(a.X, a.Y, b.X, b.Y, level - 1) });

          a = new Point(b);
          b.X -= segment_unit_size;
          result.Add(new RecursiveSegment { A = new Point(a), B = new Point(b), Line = make_line(a.X, a.Y, b.X, b.Y, level - 1) });

          a = new Point(b);
          b.Y -= segment_unit_size;
          result.Add(new RecursiveSegment { A = new Point(a), B = new Point(b), Line = make_line(a.X, a.Y, b.X, b.Y, level - 1) });

          a = new Point(b);
          b.X = x2;
          result.Add(new RecursiveSegment { A = new Point(a), B = new Point(b), Line = make_line(a.X, a.Y, b.X, b.Y, level - 1) });

          return result;
        }
        case SlopeCase.VerticalDown:
        {
          var a = new Point { X = x1, Y = y1 };
          var b = new Point { X = x1, Y = y1 - segment_unit_size };
          result.Add(new RecursiveSegment { A = new Point(a), B = new Point(b), Line = make_line(a.X, a.Y, b.X, b.Y, level - 1) });

          a = new Point(b);
          b.X += segment_unit_size;
          result.Add(new RecursiveSegment { A = new Point(a), B = new Point(b), Line = make_line(a.X, a.Y, b.X, b.Y, level - 1) });

          a = new Point(b);
          b.Y -= segment_unit_size;
          result.Add(new RecursiveSegment { A = new Point(a), B = new Point(b), Line = make_line(a.X, a.Y, b.X, b.Y, level - 1) });

          a = new Point(b);
          b.X -= segment_unit_size * 2;
          result.Add(new RecursiveSegment { A = new Point(a), B = new Point(b), Line = make_line(a.X, a.Y, b.X, b.Y, level - 1) });

          a = new Point(b);
          b.Y -= segment_unit_size;
          result.Add(new RecursiveSegment { A = new Point(a), B = new Point(b), Line = make_line(a.X, a.Y, b.X, b.Y, level - 1) });

          a = new Point(b);
          b.X += segment_unit_size;
          result.Add(new RecursiveSegment { A = new Point(a), B = new Point(b), Line = make_line(a.X, a.Y, b.X, b.Y, level - 1) });

          a = new Point(b);
          b.Y = y2;
          result.Add(new RecursiveSegment { A = new Point(a), B = new Point(b), Line = make_line(a.X, a.Y, b.X, b.Y, level - 1) });

          return result;
        }
        default: throw new Exception($"unsupported {slope}");
      }
    }
    public static IEnumerable<IEnumerable<Segment>> make_shape(params IEnumerable<Segment>[] lines)
    {
      return lines?.Aggregate(new List<IEnumerable<Segment>>(), (whole, next) => { if (next != null) whole.Add(next); return whole; });
    }
  }
  public class Plane
  {
    public Plane(PointUnit width, PointUnit height)
    {
      Width = width;
      Height = height;
      Origin_API = new Point { X = width / 2F, Y = height / 2F };
    }

    public PointUnit Width, Height;
    public Point Origin_API;

    //https://docs.microsoft.com/en-us/dotnet/framework/winforms/advanced/types-of-coordinate-systems
    public Point ToAPI(Point a) => new Point { X = Origin_API.X + a.X, Y = Origin_API.Y - a.Y };
    public (T X, T Y) ToAPI<T>(Point a) => ((T)Convert.ChangeType(Origin_API.X + a.X, typeof(T)), (T)Convert.ChangeType(Origin_API.Y - a.Y, typeof(T)));

    public static (PointUnit Slope, SlopeCase Case) GetSlopeCase(PointUnit x1, PointUnit y1, PointUnit x2, PointUnit y2)
    {
      SlopeCase @case = SlopeCase.Unsupported;
      PointUnit m = PointUnit.NaN;
      var dy = y2 - y1;
      var dx = x2 - x1;
      if (dx > 0D || dx < 0D)
      {
        m = dy / dx;
      }
      if (PointUnit.IsPositiveInfinity(dy / dx))
      {
        @case = SlopeCase.VerticalUp;
      }
      else if (PointUnit.IsNegativeInfinity(dy / dx))
      {
        @case = SlopeCase.VerticalDown;
      }
      else if (dx < 0D && m == 0D)
      {
        @case = SlopeCase.HorizontalLeft;
      }
      else if (dx > 0D && m == 0D)
      {
        @case = SlopeCase.HorizontalRight;
      }
      return (m, @case);
    }
  }
  public class Draw
  {
    //https://devblogs.microsoft.com/dotnet/net-core-image-processing/
    public static System.Drawing.Image CreateImage(IEnumerable<IEnumerable<Segment>> shape, int width, int height, System.Drawing.Imaging.PixelFormat format = System.Drawing.Imaging.PixelFormat.Format64bppArgb)
    {
      var result = new System.Drawing.Bitmap((int)width, (int)height, format);
      var g = System.Drawing.Graphics.FromImage(result);
      g.FillRectangle(System.Drawing.Brushes.White, 0, 0, width, height);
      var plane = new Plane(width, height);
      foreach (var line in shape)
      {
        foreach (var segment in line)
        {
          segment.Draw(draw);
        }
      }
      return result;

      void draw(Point a, Point b)
      {
        var _a = plane.ToAPI<float>(a);
        var _b = plane.ToAPI<float>(b);
        g.DrawLine(System.Drawing.Pens.Blue, _a.X, _a.Y, _b.X, _b.Y);
      }
    }
  }

  public class Input
  {
    public FileInfo file;
    public int? level;
    public void fractal()
    {
      if (!level.HasValue)
      {
        level = 1;
      }
      IEnumerable<Segment> line1 = Fractal.make_line(-50F, 50F, 50F, 50F, level.Value);
      IEnumerable<Segment> line2 = Fractal.make_line(50F, 50F, 50F, -50F, level.Value);
      IEnumerable<Segment> line3 = Fractal.make_line(50F, -50F, -50F, -50F, level.Value);
      IEnumerable<Segment> line4 = Fractal.make_line(-50F, -50F, -50F, 50F, level.Value);
      var shape = Fractal.make_shape(line1, line2, line3, line4);
      using var image = Draw.CreateImage(shape, 400, 400);
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