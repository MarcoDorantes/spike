using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace fractal.spec
{
    #region SUT
    public class Point { public float X, Y; }
    public class Segment { public Point A, B; }
    public class SUT
    {
        public static IEnumerable<Segment> make_line(float x1, float y1, float x2, float y2)
        {
            return new List<Segment> { new Segment { A = new Point { X = x1, Y = y1 }, B = new Point { X = x2, Y = y2 } } };
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
    }
}