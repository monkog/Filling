namespace FillingDemo.Helpers
{
	public static class PointExtensions
	{
		public static System.Windows.Point ToWindowsPoint(this System.Drawing.Point p)
		{
			return new System.Windows.Point(p.X, p.Y);
		}

		public static System.Drawing.Point ToDrawingPoint(this System.Windows.Point p)
		{
			return new System.Drawing.Point((int)p.X, (int)p.Y);
		}

		public static System.Windows.Point ToWindowsPoint(this System.Drawing.PointF pointF)
		{
			return new System.Windows.Point((int)pointF.X, (int)pointF.Y);
		}

		public static System.Drawing.PointF ToPointF(this System.Windows.Point point)
		{
			return new System.Drawing.PointF((float)point.X, (float)point.Y);
		}
	}
}