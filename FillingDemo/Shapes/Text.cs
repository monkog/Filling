using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Controls;
using System.Windows.Media;
using Color = System.Drawing.Color;
using FontFamily = System.Drawing.FontFamily;
using FontStyle = System.Drawing.FontStyle;
using Point = System.Drawing.Point;

namespace FillingDemo.Shapes
{
	public class Text : Shape
	{
		private readonly double _width;

		private readonly double _height;

		public Text(string text, float fontSize)
		{
			GraphicsPath = ConvertTextToGraphics(text, fontSize);
			ActiveEdges = CreateActiveEdgesList(GraphicsPath);

			var bounds = GraphicsPath.GetBounds();
			_width = bounds.Right;
			_height = bounds.Bottom;
		}

		public Bitmap Draw(Color color)
		{
			var boundRect = GraphicsPath.GetBounds();
			var resultBitmap = new Bitmap((int)(boundRect.Size.Width + boundRect.X + 1), (int)(boundRect.Size.Height + boundRect.Y + 1));

			var texture = new Bitmap(1, 1);
			texture.SetPixel(0, 0, color);

			EdgesSortFill(texture, resultBitmap, 255);

			return resultBitmap;
		}

		public IEnumerable<Canvas> FindIntersections(IEnumerable<Polygon> polygons)
		{
			var intersections = new List<Canvas>();
			foreach (var polygon in polygons)
			{
				foreach (var edge in polygon.ActiveEdges)
				{
					foreach (var textEdge in ActiveEdges)
					{
						var intersection = edge.FindIntersection(textEdge);
						if (intersection == null) continue;

						var canvas = new Canvas
						{
							Width = 10,
							Height = 10,
							Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 255, 0))
						};
						Canvas.SetTop(canvas, intersection.Value.Y);
						Canvas.SetLeft(canvas, intersection.Value.X);

						intersections.Add(canvas);
					}
				}
			}

			return intersections;
		}

		/// <summary>
		/// Moves the text by the provided coordinates.
		/// </summary>
		/// <param name="deltaX">Value to move the X coordinate by.</param>
		/// <param name="deltaY">Value to move the y coordinate by.</param>
		/// <param name="maxX">Width boundary.</param>
		/// <param name="maxY">Height boundary.</param>
		public void Move(double deltaX, double deltaY, double maxX, double maxY)
		{
			var oldX = X;
			var oldY = Y;

			Y = Math.Min(Math.Max(0, Y + deltaY), maxY - _height);
			X = Math.Min(Math.Max(0, X + deltaX), maxX - _width);

			foreach (var edge in ActiveEdges)
			{
				edge.Move(X - oldX, Y - oldY);
			}
		}

		private static GraphicsPath ConvertTextToGraphics(string text, float fontSize)
		{
			var graphicsPath = new GraphicsPath();
			graphicsPath.AddString(text, FontFamily.GenericSerif, (int)FontStyle.Bold, fontSize, new Point(0, 0), StringFormat.GenericDefault);
			graphicsPath.Flatten();
			return graphicsPath;
		}
	}
}