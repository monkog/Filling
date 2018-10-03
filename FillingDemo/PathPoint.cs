using System.Windows;

namespace FillingDemo
{
    class PathPoint
    {
        public PathPoint(Point point, byte type)
        {
            Point = point;
            Type = type;
        }

        public Point Point { get; set; }
        public byte Type { get; set; }
    }
}
