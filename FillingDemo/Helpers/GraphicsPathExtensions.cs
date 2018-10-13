using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows;

namespace FillingDemo.Helpers
{
	public static class GraphicsPathExtensions
	{
		/// <summary>
		/// Creates a GraphicsPath for the given collection of points.
		/// </summary>
		/// <param name="points">Collection of points.</param>
		/// <returns>Graphics path.</returns>
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

		/// <summary>
		/// Splits the given path into closed segments.
		/// For letter j it would be two segments: the dot and the major part of the letter.
		/// </summary>
		/// <param name="path">Path to divide.</param>
		/// <returns>Collection of points for each segment.</returns>
		public static IEnumerable<IEnumerable<Point>> GetPathSegments(this GraphicsPath path)
		{
			var segments = new List<IEnumerable<Point>>();
			var segment = new List<Point>();

			var points = path.PathPoints.ToList();
			var pointTypes = path.PathTypes;
			for (var index = 0; index < points.Count; index++)
			{
				var point = points[index];
				segment.Add(point.ToWindowsPoint());

				// If this point type is the last point in the segment.
				// See https://docs.microsoft.com/en-us/dotnet/api/system.drawing.drawing2d.graphicspath.pathtypes?view=netframework-4.7.2
				if (pointTypes[index] != 129) continue;

				segments.Add(segment);
				segment = new List<Point>();
			}

			if(segment.Any()) segments.Add(segment);

			return segments;
		}
	}
}