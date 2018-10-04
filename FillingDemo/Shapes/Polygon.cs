using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using FillingDemo.Helpers;
using Point = System.Windows.Point;

namespace FillingDemo.Shapes
{
	public class Polygon : Shape
	{
		private readonly List<Point> _points;

		public IEnumerable<ActiveEdge> ActiveEdges { get; }

		public GraphicsPath GraphicsPath { get; }

		public Polygon(IEnumerable<Point> points)
		{
			_points = points.ToList();

			GraphicsPath = _points.CreateGraphicsPath();
			ActiveEdges = CreateActiveEdges(_points);
		}

		/// <summary>
		/// Fills the polygon with the given texture. Uses edge sort fill.
		/// </summary>
		/// <param name="texture">Texture to fill the polygon with.</param>
		/// <param name="resultBitmap">Resulting bitmap.</param>
		/// <param name="opacity">Opacity of the texture.</param>
		public void EdgesSortFill(Bitmap texture, Bitmap resultBitmap, int opacity)
		{
			var activeEdges = new List<ActiveEdge>();
			var polygonEdges = ActiveEdges.ToList();
			polygonEdges.Sort((p, q) => (p.StartPoint.Y >= q.StartPoint.Y) ? 1 : -1);
			var scanLine = (int)polygonEdges.First().StartPoint.Y;

			do
			{
				activeEdges.AddRange(polygonEdges.Where(edge => edge.CurrentPoint.Y == scanLine && !edge.IsHorizontal));
				polygonEdges.RemoveAll(edge => edge.CurrentPoint.Y == scanLine);
				activeEdges.Sort((p, q) => (p.CurrentPoint.X >= q.CurrentPoint.X) ? 1 : -1);

				for (int i = 0; i < activeEdges.Count - 1; i += 2)
				{
					var activeEdge = activeEdges[i];
					var nextActiveEdge = activeEdges[i + 1];

					if (activeEdge.IsEdgeEnd)
					{
						i -= 2;
						activeEdges.Remove(activeEdge);
						continue;
					}

					if (nextActiveEdge.IsEdgeEnd)
					{
						i -= 2;
						activeEdges.Remove(nextActiveEdge);
						continue;
					}

					var startX = (int)activeEdge.CurrentPoint.X;
					var endX = (int)nextActiveEdge.CurrentPoint.X;
					resultBitmap.DrawLine(texture, opacity, startX, endX, scanLine);
				}

				activeEdges = activeEdges.Where(p => p.EndPoint.Y != scanLine).ToList();
				activeEdges.ForEach(point => point.CurrentPoint = new Point(point.CurrentPoint.X + point.Delta, point.CurrentPoint.Y + 1));
				scanLine++;
			} while (polygonEdges.Any() || activeEdges.Any());
		}

		private static IEnumerable<ActiveEdge> CreateActiveEdges(IEnumerable<Point> points)
		{
			points = points.ToArray();
			var edges = new List<ActiveEdge>();

			for (int i = 0; i < points.Count(); i++)
			{
				edges.Add(new ActiveEdge(points.ElementAt(i), points.ElementAt((i + 1) % points.Count())));
			}

			return edges;
		}
	}
}