using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using fractal.lib;

namespace fractal.spec
{
    [TestClass]
    public class fractalSpec
    {
        [TestMethod]
        public void level_0_line()
        {
            //Arrange
            IEnumerable<Segment> line = null;

            //Act
            line = Fractal.make_line(0F, 0F, 100F, 0F);

            //Assert
            Assert.IsNotNull(line);
            Assert.AreEqual(1, line.Count());
            Assert.AreEqual(0F, line.First().A.X);
            Assert.AreEqual(0F, line.First().A.Y);
            Assert.AreEqual(100F, line.First().B.X);
            Assert.AreEqual(0F, line.First().B.Y);
        }

        [TestMethod]
        public void level_1_line_a()
        {
            //Arrange
            IEnumerable<Segment> line = null;

            //Act
            line = Fractal.make_line(0F, 0F, 100F, 0F, 1);

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

        [TestMethod]
        public void level_1_line_b()
        {
            //Arrange
            IEnumerable<Segment> line = null;

            //Act
            line = Fractal.make_line(0F, 0F, 0F, 100F, 2);

            //Assert
            Assert.IsNotNull(line);
            Assert.AreEqual(7, line.Count());

            Assert.AreEqual(0F, line.First().A.X);
            Assert.AreEqual(0F, line.First().A.Y);
            Assert.AreEqual(0F, line.First().B.X);
            Assert.AreEqual(25F, line.First().B.Y);

            Assert.AreEqual(0F, line.ElementAt(1).A.X);
            Assert.AreEqual(25F, line.ElementAt(1).A.Y);
            Assert.AreEqual(-25F, line.ElementAt(1).B.X);
            Assert.AreEqual(25F, line.ElementAt(1).B.Y);

            Assert.AreEqual(-25F, line.ElementAt(2).A.X);
            Assert.AreEqual(25F, line.ElementAt(2).A.Y);
            Assert.AreEqual(-25F, line.ElementAt(2).B.X);
            Assert.AreEqual(50F, line.ElementAt(2).B.Y);

            Assert.AreEqual(-25F, line.ElementAt(3).A.X);
            Assert.AreEqual(50F, line.ElementAt(3).A.Y);
            Assert.AreEqual(25F, line.ElementAt(3).B.X);
            Assert.AreEqual(50F, line.ElementAt(3).B.Y);

            Assert.AreEqual(25F, line.ElementAt(4).A.X);
            Assert.AreEqual(50F, line.ElementAt(4).A.Y);
            Assert.AreEqual(25F, line.ElementAt(4).B.X);
            Assert.AreEqual(75F, line.ElementAt(4).B.Y);

            Assert.AreEqual(25F, line.ElementAt(5).A.X);
            Assert.AreEqual(75F, line.ElementAt(5).A.Y);
            Assert.AreEqual(0F, line.ElementAt(5).B.X);
            Assert.AreEqual(75F, line.ElementAt(5).B.Y);

            Assert.AreEqual(0F, line.Last().A.X);
            Assert.AreEqual(75F, line.Last().A.Y);
            Assert.AreEqual(0F, line.Last().B.X);
            Assert.AreEqual(100F, line.Last().B.Y);
        }

        [TestMethod]
        public void level_1_line_c()
        {
            //Arrange
            IEnumerable<Segment> line = null;

            //Act
            line = Fractal.make_line(0F, 0F, -100F, 0F, 3);

            //Assert
            Assert.IsNotNull(line);
            Assert.AreEqual(7, line.Count());

            Assert.AreEqual(0F, line.First().A.X);
            Assert.AreEqual(0F, line.First().A.Y);
            Assert.AreEqual(-25F, line.First().B.X);
            Assert.AreEqual(0F, line.First().B.Y);

            Assert.AreEqual(-25F, line.ElementAt(1).A.X);
            Assert.AreEqual(0F, line.ElementAt(1).A.Y);
            Assert.AreEqual(-25F, line.ElementAt(1).B.X);
            Assert.AreEqual(-25F, line.ElementAt(1).B.Y);

            Assert.AreEqual(-25F, line.ElementAt(2).A.X);
            Assert.AreEqual(-25F, line.ElementAt(2).A.Y);
            Assert.AreEqual(-50F, line.ElementAt(2).B.X);
            Assert.AreEqual(-25F, line.ElementAt(2).B.Y);

            Assert.AreEqual(-50F, line.ElementAt(3).A.X);
            Assert.AreEqual(-25F, line.ElementAt(3).A.Y);
            Assert.AreEqual(-50F, line.ElementAt(3).B.X);
            Assert.AreEqual(25F, line.ElementAt(3).B.Y);

            Assert.AreEqual(-50F, line.ElementAt(4).A.X);
            Assert.AreEqual(25F, line.ElementAt(4).A.Y);
            Assert.AreEqual(-75F, line.ElementAt(4).B.X);
            Assert.AreEqual(25F, line.ElementAt(4).B.Y);

            Assert.AreEqual(-75F, line.ElementAt(5).A.X);
            Assert.AreEqual(25F, line.ElementAt(5).A.Y);
            Assert.AreEqual(-75F, line.ElementAt(5).B.X);
            Assert.AreEqual(0F, line.ElementAt(5).B.Y);

            Assert.AreEqual(-75F, line.Last().A.X);
            Assert.AreEqual(0F, line.Last().A.Y);
            Assert.AreEqual(-100F, line.Last().B.X);
            Assert.AreEqual(0F, line.Last().B.Y);
        }

        [TestMethod]
        public void level_1_line_d()
        {
            //Arrange
            IEnumerable<Segment> line = null;

            //Act
            line = Fractal.make_line(0F, 0F, 0F, -100F, 4);

            //Assert
            Assert.IsNotNull(line);
            Assert.AreEqual(7, line.Count());

            Assert.AreEqual(0F, line.First().A.X);
            Assert.AreEqual(0F, line.First().A.Y);
            Assert.AreEqual(0F, line.First().B.X);
            Assert.AreEqual(-25F, line.First().B.Y);

            Assert.AreEqual(0F, line.ElementAt(1).A.X);
            Assert.AreEqual(-25F, line.ElementAt(1).A.Y);
            Assert.AreEqual(25F, line.ElementAt(1).B.X);
            Assert.AreEqual(-25F, line.ElementAt(1).B.Y);

            Assert.AreEqual(25F, line.ElementAt(2).A.X);
            Assert.AreEqual(-25F, line.ElementAt(2).A.Y);
            Assert.AreEqual(25F, line.ElementAt(2).B.X);
            Assert.AreEqual(-50F, line.ElementAt(2).B.Y);

            Assert.AreEqual(25F, line.ElementAt(3).A.X);
            Assert.AreEqual(-50F, line.ElementAt(3).A.Y);
            Assert.AreEqual(-25F, line.ElementAt(3).B.X);
            Assert.AreEqual(-50F, line.ElementAt(3).B.Y);

            Assert.AreEqual(-25F, line.ElementAt(4).A.X);
            Assert.AreEqual(-50F, line.ElementAt(4).A.Y);
            Assert.AreEqual(-25F, line.ElementAt(4).B.X);
            Assert.AreEqual(-75F, line.ElementAt(4).B.Y);

            Assert.AreEqual(-25F, line.ElementAt(5).A.X);
            Assert.AreEqual(-75F, line.ElementAt(5).A.Y);
            Assert.AreEqual(0F, line.ElementAt(5).B.X);
            Assert.AreEqual(-75F, line.ElementAt(5).B.Y);

            Assert.AreEqual(0F, line.Last().A.X);
            Assert.AreEqual(-75F, line.Last().A.Y);
            Assert.AreEqual(0F, line.Last().B.X);
            Assert.AreEqual(-100F, line.Last().B.Y);
        }
    }

    [TestClass]
    public class planeSpec
    {
        [TestMethod]
        public void origin()
        {
            //Arrange
            var o = new Point { X = 0F, Y = 0F };
            var plane = new Plane(100F, 100F);

            //Act
            var a = plane.ToAPI(o);

            //Assert
            Assert.AreEqual(50F, a.X);
            Assert.AreEqual(50F, a.Y);
        }

        [TestMethod]
        public void points_1()
        {
            //Arrange
            var _a = new Point { X = 1F, Y = 0F };
            var _b = new Point { X = 1F, Y = 1F };
            var _c = new Point { X = -1F, Y = 1F };
            var _d = new Point { X = -1F, Y = -1F };
            var plane = new Plane(100F, 100F);

            //Act
            var a = plane.ToAPI(_a);
            var b = plane.ToAPI(_b);
            var c = plane.ToAPI(_c);
            var d = plane.ToAPI(_d);

            //Assert
            Assert.AreEqual(51F, a.X);
            Assert.AreEqual(50F, a.Y);

            Assert.AreEqual(51F, b.X);
            Assert.AreEqual(49F, b.Y);

            Assert.AreEqual(49F, c.X);
            Assert.AreEqual(49F, c.Y);

            Assert.AreEqual(49F, d.X);
            Assert.AreEqual(51F, d.Y);
        }

        [TestMethod]
        public void points_1_b()
        {
            //Arrange
            var _a = new Point { X = 1F, Y = 0F };
            var _b = new Point { X = 1F, Y = 1F };
            var _c = new Point { X = -1F, Y = 1F };
            var _d = new Point { X = -1F, Y = -1F };
            var plane = new Plane(100F, 100F);

            //Act
            var a = plane.ToAPI<float>(_a);
            var b = plane.ToAPI<float>(_b);
            var c = plane.ToAPI<float>(_c);
            var d = plane.ToAPI<float>(_d);

            //Assert
            Assert.AreEqual(51F, a.X);
            Assert.AreEqual(50F, a.Y);

            Assert.AreEqual(51F, b.X);
            Assert.AreEqual(49F, b.Y);

            Assert.AreEqual(49F, c.X);
            Assert.AreEqual(49F, c.Y);

            Assert.AreEqual(49F, d.X);
            Assert.AreEqual(51F, d.Y);
        }

        [TestMethod]
        public void points_50()
        {
            //Arrange
            var _a = new Point { X = 50F, Y = 50F };
            var _b = new Point { X = -50F, Y = 50F };
            var _c = new Point { X = -50F, Y = -50F };
            var _d = new Point { X = 50F, Y = -50F };
            var plane = new Plane(100F, 100F);

            //Act
            var a = plane.ToAPI(_a);
            var b = plane.ToAPI(_b);
            var c = plane.ToAPI(_c);
            var d = plane.ToAPI(_d);

            //Assert
            Assert.AreEqual(100F, a.X);
            Assert.AreEqual(0F, a.Y);

            Assert.AreEqual(0F, b.X);
            Assert.AreEqual(0F, b.Y);

            Assert.AreEqual(0F, c.X);
            Assert.AreEqual(100F, c.Y);

            Assert.AreEqual(100F, d.X);
            Assert.AreEqual(100F, d.Y);
        }
    }

    [TestClass]
    public class visualSpec
    {
        [TestMethod]
        public void create_image()
        {
            //Arrange
            IEnumerable<Segment> line = Fractal.make_line(0F, 0F, 0F, -100F, 4);

            //Act
            using var image = Draw.CreateImage(line, 200, 200);

            //Assert
            Assert.IsNotNull(image);
        }
    }
}