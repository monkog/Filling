using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using FillingDemo.Helpers;
using Color = System.Drawing.Color;
using FontFamily = System.Drawing.FontFamily;
using FontStyle = System.Drawing.FontStyle;
using Point = System.Drawing.Point;

namespace FillingDemo.Shapes
{
	public class Text : Shape
	{
		/// <summary>
		/// Gets the graphics path.
		/// </summary>
		public GraphicsPath GraphicsPath { get; }

		public Text(string text, float fontSize)
		{
			GraphicsPath = ConvertTextToGraphics(text, fontSize);
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
			var pathPoints = GraphicsPath.PathPoints.Select(p => p.ToWindowsPoint());

			var texture = new Bitmap(1, 1);
			texture.SetPixel(0, 0, color);

			EdgesSortFill(texture, resultBitmap, pathPoints, GraphicsPath.PathTypes, 255);

			return resultBitmap;
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