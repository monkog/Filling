using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Color = System.Drawing.Color;
using FlowDirection = System.Windows.FlowDirection;
using FontFamily = System.Drawing.FontFamily;
using FontStyle = System.Drawing.FontStyle;
using Point = System.Drawing.Point;

namespace FillingDemo.Shapes
{
	public class Text : Shape
	{
		public Text(string text, float fontSize)
		{
			GraphicsPath = ConvertTextToGraphics(text, fontSize);
			ActiveEdges = CreateActiveEdgesList(GraphicsPath);
		}

		/// <summary>
		/// Create path from text.
		/// </summary>
		public static PathGeometry ToSmootherGraphics(
			string text,
			double fontSize,
			System.Windows.Media.FontFamily fontFamily,
			System.Windows.FontStyle fontStyle,
			FontWeight fontWeight,
			FontStretch fontStretch,
			System.Windows.Media.Brush fill)
		{
			FormattedText formattedText = new FormattedText(text, Thread.CurrentThread.CurrentUICulture,
				FlowDirection.LeftToRight,
				new Typeface(fontFamily, fontStyle, fontWeight, fontStretch),
				fontSize, fill);

			var textGeometry = formattedText.BuildGeometry(new System.Windows.Point(0, 0));

			var path = new Path { Fill = fill, Data = textGeometry };
			//Canvas.SetZIndex(canvas, int.MaxValue);
			//Canvas.Children.Add(path);

			return textGeometry.GetOutlinedPathGeometry();
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

		private static GraphicsPath ConvertTextToGraphics(string text, float fontSize)
		{
			var graphicsPath = new GraphicsPath();
			graphicsPath.AddString(text, FontFamily.GenericSerif, (int)FontStyle.Bold, fontSize, new Point(0, 0), StringFormat.GenericDefault);
			graphicsPath.Flatten();
			return graphicsPath;
		}
	}
}