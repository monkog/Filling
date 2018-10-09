using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using FillingDemo.Helpers;
using Point = System.Windows.Point;

namespace FillingDemo.Shapes
{
	public abstract class Shape
	{
		public static void EdgesSortFill(Bitmap texture, Bitmap resultBitmap, IEnumerable<Point> points, byte[] pointTypes, int opacity)
		{
			var polygonEdges = new List<ActiveEdge>();
			var activeEdges = CreateActiveEdgesList(points, pointTypes);
			activeEdges.Sort((p, q) => (p.StartPoint.Y >= q.StartPoint.Y) ? 1 : -1);
			int scanLine = (int)activeEdges[0].StartPoint.Y;

			do
			{
				polygonEdges.AddRange(activeEdges.Where(edge => edge.CurrentPoint.Y == scanLine && edge.StartPoint.Y != edge.EndPoint.Y));
				activeEdges.RemoveAll(edge => edge.CurrentPoint.Y == scanLine);
				polygonEdges.Sort((p, q) => (p.CurrentPoint.X >= q.CurrentPoint.X) ? 1 : -1);

				for (int i = 0; i < polygonEdges.Count - 1; i += 2)
				{
					var currentPointInfo = polygonEdges[i];
					var nextPointInfo = polygonEdges[i + 1];

					if (currentPointInfo.CurrentPoint.Y == currentPointInfo.EndPoint.Y)
					{
						i -= 2;
						polygonEdges.Remove(currentPointInfo);
						continue;
					}

					if (nextPointInfo.CurrentPoint.Y == nextPointInfo.EndPoint.Y)
					{
						i -= 2;
						polygonEdges.Remove(nextPointInfo);
						continue;
					}

					var startX = (int)currentPointInfo.CurrentPoint.X;
					var endX = (int)nextPointInfo.CurrentPoint.X;
					resultBitmap.DrawLine(texture, opacity, startX, endX, scanLine);
				}

				polygonEdges = polygonEdges.Where(p => p.EndPoint.Y != scanLine).ToList();
				polygonEdges.ForEach(point => point.CurrentPoint = new Point(point.CurrentPoint.X + point.Delta, point.CurrentPoint.Y + 1));
				scanLine++;
			} while (activeEdges.Any() || polygonEdges.Any());
		}

		public static List<ActiveEdge> CreateActiveEdgesList(IEnumerable<Point> points, byte[] pointTypes)
		{
			var pointInfoList = new List<ActiveEdge>();

			for (int i = 0; i < points.Count(); i++)
			{
				var helperNextPointList = new List<Point>();
				var helperCurrentPointList = new List<ActiveEdge>();
				bool isFirstPoint = true;

				while (i < pointTypes.Length && (pointTypes[i] != 0 || isFirstPoint))
				{
					isFirstPoint = false;
					Point startPoint = new Point(points.ElementAt(i).X, points.ElementAt(i).Y);
					var pointInfo = new ActiveEdge(startPoint, startPoint);
					helperCurrentPointList.Add(pointInfo);
					helperNextPointList.Add(points.ElementAt(i));
					i++;
				}

				if (i < pointTypes.Length && pointTypes[i] == 0)
					i--;

				for (int j = 0; j < helperCurrentPointList.Count; j++)
				{
					var tmpPoint = helperNextPointList[(j + 1) % helperCurrentPointList.Count];

					// Keep the point with lower y coordinate the m_startPoint.
					if (helperCurrentPointList[j].StartPoint.Y > tmpPoint.Y)
					{
						helperCurrentPointList[j].EndPoint = helperCurrentPointList[j].StartPoint;
						helperCurrentPointList[j].StartPoint = tmpPoint;
					}
					else
						helperCurrentPointList[j].EndPoint = tmpPoint;

					double deltaX = helperCurrentPointList[j].EndPoint.X - helperCurrentPointList[j].StartPoint.X;
					double deltaY = helperCurrentPointList[j].EndPoint.Y - helperCurrentPointList[j].StartPoint.Y;

					// Calculate the difference between next x-coordinates.
					if (deltaX != 0.0 && deltaY != 0.0)
						helperCurrentPointList[j].Delta = 1 / (deltaY / deltaX);
					else
						helperCurrentPointList[j].Delta = 0;

					helperCurrentPointList[j].CurrentPoint = helperCurrentPointList[j].StartPoint;
					pointInfoList.Add(helperCurrentPointList[j]);
				}
			}

			return pointInfoList;
		}
	}
}