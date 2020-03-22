using System;
using System.Collections.Generic;

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

        public Point ToAPI(Point a) => new Point { X = Origin_API.X + a.X, Y = Origin_API.Y - a.Y };
        public (T X, T Y) ToAPI<T>(Point a) => ((T)Convert.ChangeType(Origin_API.X + a.X, typeof(T)), (T)Convert.ChangeType(Origin_API.Y - a.Y, typeof(T)));

    }
}