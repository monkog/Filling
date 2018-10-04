using System.Diagnostics;
using System.Windows;

namespace FillingDemo
{
	[DebuggerDisplay("Start = {StartPoint.X}, {StartPoint.Y} End = {EndPoint.X}, {EndPoint.Y}, Delta = {Delta}")]
	public class PointInfo
    {
        public Point StartPoint { get; set; }
        public Point EndPoint { get; set; }
        public Point CurrentPoint { get; set; }
        public double Delta { get; set; } 
    }
}
