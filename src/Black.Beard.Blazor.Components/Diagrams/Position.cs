using Blazor.Diagrams.Core.Geometry;
using System.Diagnostics;

namespace Bb.Diagrams
{

    [DebuggerDisplay("{X},{Y}")]
    public class Position : IComparable, IComparable<Position>
    {

        public Position()
        {

        }

        public Position(Point point)
            : this(point.X, point.Y)
        {

        }

        public Position(double x, double y)
        {
            X = x;
            Y = y;
        }

        public double X { get; set; }

        public double Y { get; set; }

        /// <summary>
        /// Implicit conversion from Point to Position
        /// </summary>
        /// <param name="p"></param>
        public static implicit operator Point(Position p)
        {
            return new Point(p.X, p.Y);
        }

        public int CompareTo(object? obj)
        {
            if (obj is Position p)
                return CompareTo(p);
            return -1;
        }

        public int CompareTo(Position? other)
        {

            if (other == null)
                return -1;

            if (X == other.X && Y == other.Y)
                return 0;

            if (X > other.X)
                return 1;

            if (X < other.X)
                return -1;

            if (Y > other.Y)
                return 1;

            if (Y < other.Y)
                return -1;

            return 0;

        }
        
        public override bool Equals(object? obj)
        {

            if (obj is Position p)
                return CompareTo(p) == 0;

            return base.Equals(obj);

        }

    }


}
