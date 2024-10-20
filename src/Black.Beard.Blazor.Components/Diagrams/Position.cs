using Blazor.Diagrams.Core.Geometry;
using System.Diagnostics;

namespace Bb.Diagrams
{

    [DebuggerDisplay("{X},{Y}")]
    public class Position
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

    }


}
