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
	}
}