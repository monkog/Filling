using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows;

namespace FillingDemo.Helpers
{
	public static class GraphicsPathExtensions
	{
		public static GraphicsPath CreateGraphicsPath(this IEnumerable<Point> points)
		{
			points = points.ToArray();
			var path = new GraphicsPath();
			for (int i = 0; i < points.Count(); i++)
			{
				var p1 = points.ElementAt(i).ToDrawingPoint();
				var p2 = points.ElementAt((i + 1) % points.Count()).ToDrawingPoint();
				path.AddLine(p1, p2);
			}

			return path;
		}
	}
}