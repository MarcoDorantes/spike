using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace fractal.spec
{
    #region SUT
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
    public class SUT
    {
        public static IEnumerable<Segment> make_line(PointUnitType x1, PointUnitType y1, PointUnitType x2, PointUnitType y2, int level = 0)
        {
            if (level == 0) return new List<Segment> { new Segment { A = new Point { X = x1, Y = y1 }, B = new Point { X = x2, Y = y2 } } };
            else
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
        }
    }
    #endregion

    [TestClass]
    public class fractalSpec
    {
        [TestMethod]
        public void level_0_line()
        {
            //Arrange
            IEnumerable<Segment> line = null;

            //Act
            line = SUT.make_line(0F, 0F, 100F, 0F);

            //Assert
            Assert.IsNotNull(line);
            Assert.AreEqual(1, line.Count());
            Assert.AreEqual(0F, line.First().A.X);
            Assert.AreEqual(0F, line.First().A.Y);
            Assert.AreEqual(100F, line.First().B.X);
            Assert.AreEqual(0F, line.First().B.Y);
        }

        [TestMethod]
        public void level_1_line()
        {
            //Arrange
            IEnumerable<Segment> line = null;

            //Act
            line = SUT.make_line(0F, 0F, 100F, 0F, 1);

            //Assert
            Assert.IsNotNull(line);
            Assert.AreEqual(7, line.Count());

            Assert.AreEqual(0F, line.First().A.X);
            Assert.AreEqual(0F, line.First().A.Y);
            Assert.AreEqual(25F, line.First().B.X);
            Assert.AreEqual(0F, line.First().B.Y);

            Assert.AreEqual(25F, line.ElementAt(1).A.X);
            Assert.AreEqual(0F, line.ElementAt(1).A.Y);
            Assert.AreEqual(25F, line.ElementAt(1).B.X);
            Assert.AreEqual(25F, line.ElementAt(1).B.Y);

            Assert.AreEqual(25F, line.ElementAt(2).A.X);
            Assert.AreEqual(25F, line.ElementAt(2).A.Y);
            Assert.AreEqual(50F, line.ElementAt(2).B.X);
            Assert.AreEqual(25F, line.ElementAt(2).B.Y);

            Assert.AreEqual(50F, line.ElementAt(3).A.X);
            Assert.AreEqual(25F, line.ElementAt(3).A.Y);
            Assert.AreEqual(50F, line.ElementAt(3).B.X);
            Assert.AreEqual(-25F, line.ElementAt(3).B.Y);

            Assert.AreEqual(50F, line.ElementAt(4).A.X);
            Assert.AreEqual(-25F, line.ElementAt(4).A.Y);
            Assert.AreEqual(75F, line.ElementAt(4).B.X);
            Assert.AreEqual(-25F, line.ElementAt(4).B.Y);

            Assert.AreEqual(75F, line.ElementAt(5).A.X);
            Assert.AreEqual(-25F, line.ElementAt(5).A.Y);
            Assert.AreEqual(75F, line.ElementAt(5).B.X);
            Assert.AreEqual(0F, line.ElementAt(5).B.Y);

            Assert.AreEqual(75F, line.Last().A.X);
            Assert.AreEqual(0F, line.Last().A.Y);
            Assert.AreEqual(100F, line.Last().B.X);
            Assert.AreEqual(0F, line.Last().B.Y);
        }
    }
}