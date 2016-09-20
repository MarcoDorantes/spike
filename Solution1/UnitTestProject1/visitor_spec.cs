using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace GOF_Visitor
{
  abstract class ShapeVisitor
  {
    public abstract void VisitCircle(Circle c);
    public abstract void VisitRectangle(Rectangle r);
  }

  abstract class Shape
  {
    public Shape(System.Drawing.Point topleft)
    {
      TopLeft = topleft;
    }
    public System.Drawing.Point TopLeft;
    public abstract double Area { get; }
    public abstract void Accept(ShapeVisitor v);
  }
  class Circle : Shape
  {
    public Circle(System.Drawing.Point topleft, double ratio) : base(topleft)
    {
      Ratio = ratio;
    }
    public double Ratio;
    public override double Area => Math.PI * Math.Pow(Ratio, 2);
    public override void Accept(ShapeVisitor v) { v.VisitCircle(this); }
  }
  class Rectangle : Shape
  {
    public Rectangle(System.Drawing.Point topleft, double width, double length) : base(topleft)
    {
      Width = width;
      Length = length;
    }
    public double Width;
    public double Length;
    public override double Area => Width * Length;
    public override void Accept(ShapeVisitor v) { v.VisitRectangle(this); }
  }

  class WidthOperation : ShapeVisitor
  {
    public WidthOperation()
    {
      Result = 0D;
    }
    public double Result;
    public override void VisitCircle(Circle c) { Result += c.TopLeft.X + c.Ratio * 2; }
    public override void VisitRectangle(Rectangle r) { Result += r.TopLeft.X + r.Width; }
  }
  class LengthOperation : ShapeVisitor
  {
    public LengthOperation()
    {
      Result = 0D;
    }
    public double Result;
    public override void VisitCircle(Circle c) { Result += c.TopLeft.Y + c.Ratio * 2; }
    public override void VisitRectangle(Rectangle r) { Result += r.TopLeft.Y + r.Length; }
  }
  class AreaOperation : ShapeVisitor
  {
    public AreaOperation()
    {
      Result = 0D;
    }
    public double Result;
    public override void VisitCircle(Circle c) { Result += c.Area; }
    public override void VisitRectangle(Rectangle r) { Result += r.Area; }
  }
  [TestClass]
  public class gof_visitor_spec
  {
    [TestMethod]
    public void basic_gof_visitor()
    {
      IEnumerable<Shape> draw = new List<Shape> { new Circle(new System.Drawing.Point(0, 1), 10), new Rectangle(new System.Drawing.Point(20, 5), 10, 30) };
      double all_area = draw.Sum(s => s.Area);
      Assert.AreEqual<double>(100D * Math.PI + 300, all_area);

      var width = new WidthOperation();
      draw.Aggregate(width, (whole, next) => { next.Accept(whole); return whole; });
      Assert.AreEqual<double>(50D, width.Result);

      var length = new LengthOperation();
      draw.Aggregate(length, (whole, next) => { next.Accept(whole); return whole; });
      Assert.AreEqual<double>(56D, length.Result);

      var area = new AreaOperation();
      draw.Aggregate(area, (whole, next) => { next.Accept(whole); return whole; });
      Assert.AreEqual<double>(all_area, area.Result);
    }
  }
}

namespace Acyclic_Visitor
{
  interface IShapeVisitor {}

  abstract class Shape
  {
    public Shape(System.Drawing.Point topleft)
    {
      TopLeft = topleft;
    }
    public System.Drawing.Point TopLeft;
    public abstract double Area { get; }
    public abstract void Accept(IShapeVisitor v);
  }

  interface ICircleVisitor
  {
    void Visit(Circle c);
  }
  class Circle : Shape
  {
    public Circle(System.Drawing.Point topleft, double ratio) : base(topleft)
    {
      Ratio = ratio;
    }
    public double Ratio;
    public override double Area => Math.PI * Math.Pow(Ratio, 2);
    public override void Accept(IShapeVisitor v)
    {
      var visitor = v as ICircleVisitor;
      if (v != null) visitor.Visit(this);
      //else v cannot visit this class
    }
  }

  interface IRectangleVisitor
  {
    void Visit(Rectangle c);
  }
  class Rectangle : Shape
  {
    public Rectangle(System.Drawing.Point topleft, double width, double length) : base(topleft)
    {
      Width = width;
      Length = length;
    }
    public double Width;
    public double Length;
    public override double Area => Width * Length;
    public override void Accept(IShapeVisitor v)
    {
      var visitor = v as IRectangleVisitor;
      if (v != null) visitor.Visit(this);
      //else v cannot visit this class
    }
  }

  class WidthOperation : IShapeVisitor, ICircleVisitor , IRectangleVisitor
  {
    public WidthOperation()
    {
      Result = 0D;
    }
    public double Result;
    public void Visit(Circle c) { Result += c.TopLeft.X + c.Ratio * 2; }
    public void Visit(Rectangle r) { Result += r.TopLeft.X + r.Width; }
  }
  class LengthOperation : IShapeVisitor, ICircleVisitor, IRectangleVisitor
  {
    public LengthOperation()
    {
      Result = 0D;
    }
    public double Result;
    public void Visit(Circle c) { Result += c.TopLeft.Y + c.Ratio * 2; }
    public void Visit(Rectangle r) { Result += r.TopLeft.Y + r.Length; }
  }
  class AreaOperation : IShapeVisitor, ICircleVisitor, IRectangleVisitor
  {
    public AreaOperation()
    {
      Result = 0D;
    }
    public double Result;
    public void Visit(Circle c) { Result += c.Area; }
    public void Visit(Rectangle r) { Result += r.Area; }
  }
  [TestClass]
  public class acyclic_visitor_spec
  {
    [TestMethod]
    public void basic_acyclic_visitor()
    {
      IEnumerable<Shape> draw = new List<Shape> { new Circle(new System.Drawing.Point(0, 1), 10), new Rectangle(new System.Drawing.Point(20, 5), 10, 30) };
      double all_area = draw.Sum(s => s.Area);
      Assert.AreEqual<double>(100D * Math.PI + 300, all_area);

      var width = new WidthOperation();
      draw.Aggregate(width, (whole, next) => { next.Accept(whole); return whole; });
      Assert.AreEqual<double>(50D, width.Result);

      var length = new LengthOperation();
      draw.Aggregate(length, (whole, next) => { next.Accept(whole); return whole; });
      Assert.AreEqual<double>(56D, length.Result);

      var area = new AreaOperation();
      draw.Aggregate(area, (whole, next) => { next.Accept(whole); return whole; });
      Assert.AreEqual<double>(all_area, area.Result);
    }
  }
}

namespace a_cs_spike
{
  abstract class Shape
  {
    public Shape(System.Drawing.Point topleft)
    {
      TopLeft = topleft;
    }
    public System.Drawing.Point TopLeft;
    public abstract double Area { get; }
  }
  class Circle : Shape
  {
    public Circle(System.Drawing.Point topleft, double ratio) : base(topleft)
    {
      Ratio = ratio;
    }
    public double Ratio;
    public override double Area => Math.PI * Math.Pow(Ratio, 2);
  }
  class Rectangle : Shape
  {
    public Rectangle(System.Drawing.Point topleft, double width, double length) : base(topleft)
    {
      Width = width;
      Length = length;
    }
    public double Width;
    public double Length;
    public override double Area => Width * Length;
  }

  [TestClass]
  public class a_cs_visitor_spec
  {
    [TestMethod]
    public void basic_cs_spike_visitor()
    {
      IEnumerable<Shape> draw = new List<Shape> { new Circle(new System.Drawing.Point(0, 1), 10), new Rectangle(new System.Drawing.Point(20, 5), 10, 30) };
      double all_area = draw.Sum(s => s.Area);
      Assert.AreEqual<double>(100D * Math.PI + 300, all_area);

      double width = draw.Where(s => s is Circle).Cast<Circle>().Sum(c => c.TopLeft.X + c.Ratio * 2) + draw.Where(s => s is Rectangle).Cast<Rectangle>().Sum(r => r.TopLeft.X + r.Width);
      Assert.AreEqual<double>(50D, width);

      var length = draw.Where(s => s is Circle).Cast<Circle>().Sum(c => c.TopLeft.Y + c.Ratio * 2) + draw.Where(s => s is Rectangle).Cast<Rectangle>().Sum(r => r.TopLeft.Y + r.Length);
      Assert.AreEqual<double>(56D, length);

      double area = draw.Sum(s => s.Area);
      Assert.AreEqual<double>(all_area, area);
    }
  }
}