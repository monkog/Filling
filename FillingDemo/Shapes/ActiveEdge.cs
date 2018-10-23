using System;
using System.Diagnostics;
using System.Windows;

namespace FillingDemo.Shapes
{
	[DebuggerDisplay("Start = {StartPoint.X}, {StartPoint.Y} End = {EndPoint.X}, {EndPoint.Y}, Delta = {Delta}")]
	public class ActiveEdge
	{
		/// <summary>
		/// Gets or sets the active edge's start point.
		/// </summary>
		public Point StartPoint { get; set; }

		/// <summary>
		/// Gets or sets the active edge's end point.
		/// </summary>
		public Point EndPoint { get; set; }

		/// <summary>
		/// Gets or sets the active edge's in point.
		/// </summary>
		public Point InPoint { get; set; }

		/// <summary>
		/// Gets or sets the active edge's out point.
		/// </summary>
		public Point OutPoint { get; set; }

		/// <summary>
		/// Gets or sets the active edge's currently printed point.
		/// </summary>
		public Point CurrentPoint { get; set; }

		/// <summary>
		/// Gets or sets the a value.
		/// </summary>
		/// <remarks>Considering that the line's equation is y = ax + b.</remarks>
		public double A { get; set; }

		/// <summary>
		/// Gets or sets the b value.
		/// </summary>
		/// <remarks>Considering that the line's equation is y = ax + b.</remarks>
		public double B { get; set; }

		/// <summary>
		/// Gets or sets the 1/a value.
		/// </summary>
		/// <remarks>Considering that the line's equation is y = ax + b.</remarks>
		public double Delta { get; set; }

		/// <summary>
		/// Gets the value indicating whether the edge is horizontal.
		/// </summary>
		public bool IsHorizontal => StartPoint.Y == EndPoint.Y;

		/// <summary>
		/// Gets the value indicating whether the edge is vertical.
		/// </summary>
		public bool IsVertical => StartPoint.X == EndPoint.X;

		/// <summary>
		/// Gets the value indicating whether the current point is the end point of the edge.
		/// </summary>
		public bool IsEdgeEnd => CurrentPoint.Y == EndPoint.Y;

		public ActiveEdge(Point startPoint, Point endPoint)
		{
			InPoint = startPoint;
			OutPoint = endPoint;

			if (startPoint.Y > endPoint.Y)
			{
				StartPoint = endPoint;
				EndPoint = startPoint;
			}
			else
			{
				StartPoint = startPoint;
				EndPoint = endPoint;
			}

			CurrentPoint = StartPoint;

			Delta = CalculateDelta();
			A = Delta == 0.0 ? 0 : 1 / Delta;
			B = StartPoint.Y - (A * StartPoint.X);
		}

		/// <summary>
		/// Returns a value indicating whether the edge contains the given point.
		/// </summary>
		/// <param name="p">Point to check.</param>
		/// <returns>True if the edge contains the given point, false otherwise.</returns>
		public bool Contains(Point p)
		{
			var xMin = Math.Min(StartPoint.X, EndPoint.X);
			var xMax = Math.Max(StartPoint.X, EndPoint.X);
			var yMin = StartPoint.Y;
			var yMax = EndPoint.Y;
			
			// For vertical the equation is met when provided point is within max and min boundaries.
			var matchesEquation = IsVertical || p.Y == (A * p.X) + B;

			return p.X >= xMin && p.X <= xMax && p.Y >= yMin && p.Y <= yMax && matchesEquation;
		}

		/// <summary>
		/// Finds the intersection point with a provided line.
		/// </summary>
		/// <param name="line">Line to find intersection with.</param>
		/// <returns>Point of the intersection or null if such does not exist.</returns>
		public Point? FindIntersection(ActiveEdge line)
		{
			double x;
			double y;

			// Lines are parallel or perpendicular and horizontal or vertical.
			if (A == line.A && A == 0.0)
			{
				if ((IsHorizontal && line.IsHorizontal) || (IsVertical && line.IsVertical))
					return null;

				if (IsHorizontal)
				{
					x = line.StartPoint.X;
					y = StartPoint.Y;
				}
				else
				{
					x = StartPoint.X;
					y = line.StartPoint.Y;
				}

				var p = new Point(x, y);

				if (Contains(p) && line.Contains(p))
					return p;

				return null;
			}

			x = (line.B - B) / (A - line.A);
			y = A * x + B;
			var point = new Point(x, y);

			if (Contains(point) && line.Contains(point))
				return point;

			return null;
		}

		private double CalculateDelta()
		{
			double deltaX = EndPoint.X - StartPoint.X;
			double deltaY = EndPoint.Y - StartPoint.Y;

			// Calculate the difference between next x-coordinates.
			if (deltaY != 0.0)
				return deltaX / deltaY;

			return 0;
		}
	}
}