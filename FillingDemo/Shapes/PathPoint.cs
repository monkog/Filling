using System.Windows;

namespace FillingDemo.Shapes
{
    public class PathPoint
    {
        public PathPoint(Point point, PointType type)
        {
            Point = point;
            Type = type;
        }

		/// <summary>
		/// The coordinates of the point.
		/// </summary>
        public Point Point { get; set; }

		/// <summary>
		/// The type of the point.
		/// </summary>
        public PointType Type { get; set; }
    }
}
