using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using FillingDemo.Helpers;
using Point = System.Windows.Point;

namespace FillingDemo.Shapes
{
	public abstract class Shape
	{
		/// <summary>
		/// Gets the graphics path.
		/// </summary>
		public GraphicsPath GraphicsPath { get; protected set; }

		/// <summary>
		/// Fills this object with the given texture using active edge sort fill.
		/// </summary>
		/// <param name="texture">Texture to fill.</param>
		/// <param name="resultBitmap">Resulting bitmap containing this object.</param>
		/// <param name="opacity">Opacity of the texture.</param>
		public void EdgesSortFill(Bitmap texture, Bitmap resultBitmap, int opacity)
		{
			var activeEdges = new List<ActiveEdge>();
			var polygonEdges = CreateActiveEdgesList(GraphicsPath);
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

		private static List<ActiveEdge> CreateActiveEdgesList(GraphicsPath path)
		{
			var edges = new List<ActiveEdge>();
			var segments = path.GetPathSegments();

			foreach (var segment in segments)
			{
				for (int i = 0; i < segment.Count(); i++)
				{
					var edge = new ActiveEdge(segment.ElementAt(i), segment.ElementAt((i + 1) % segment.Count()));
					edges.Add(edge);
				}
			}

			return edges;
		}
	}
}