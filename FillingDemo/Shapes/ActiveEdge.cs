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
		/// Gets or sets the active edge's currently printed point.
		/// </summary>
		public Point CurrentPoint { get; set; }

		/// <summary>
		/// Gets or sets the 1 / ((EndPoint.Y - StartPoint.Y) / (EndPoint.X - StartPoint.X)) or 0 if at least one of the values is 0.
		/// </summary>
		public double Delta { get; set; }

		/// <summary>
		/// Gets the value indicating whether the edge is horizontal.
		/// </summary>
		public bool IsHorizontal { get; }

		/// <summary>
		/// Gets the value indicating whether the current point is the end point of the edge.
		/// </summary>
		public bool IsEdgeEnd => CurrentPoint.Y == EndPoint.Y;

		public ActiveEdge(Point startPoint, Point endPoint)
		{
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
			IsHorizontal = StartPoint.Y == EndPoint.Y;
		}

		private double CalculateDelta()
		{
			double deltaX = EndPoint.X - StartPoint.X;
			double deltaY = EndPoint.Y - StartPoint.Y;

			// Calculate the difference between next x-coordinates.
			if (deltaX != 0.0 && deltaY != 0.0)
				return 1 / (deltaY / deltaX);

			return 0;
		}
	}
}