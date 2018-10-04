using System;
using System.Drawing;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace FillingDemo.Helpers
{
	public static class BitmapExtensions
	{
		public static ImageBrush CreateImageBrush(this Bitmap bitmap)
		{
			BitmapSource bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(
				bitmap.GetHbitmap(),
				IntPtr.Zero,
				Int32Rect.Empty,
				BitmapSizeOptions.FromEmptyOptions());

			return new ImageBrush(bitmapSource);
		}

		public static void DrawLine(this Bitmap resultBitmap, Bitmap texture, int opacity, int startX, int endX, int scanLine)
		{
			for (int k = startX; k < endX; k++)
			{
				var color = texture.GetPixel(k % texture.Width, scanLine % texture.Height);
				color = System.Drawing.Color.FromArgb(opacity % 256, color.R, color.G, color.B);
				resultBitmap.SetPixel(k, scanLine, color);
			}
		}
	}
}