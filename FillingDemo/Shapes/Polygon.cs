using System.Collections.Generic;
using FillingDemo.Helpers;
using Point = System.Windows.Point;

namespace FillingDemo.Shapes
{
	public class Polygon : Shape
	{
		public Polygon(IEnumerable<Point> points)
		{
			GraphicsPath = points.CreateGraphicsPath();
			ActiveEdges = CreateActiveEdgesList(GraphicsPath);
		}
	}
}