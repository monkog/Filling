using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Media;
using FillingDemo.Helpers;
using Point = System.Windows.Point;

namespace FillingDemo.Shapes
{
	public class Background
	{
		private readonly Random _random = new Random(DateTime.Now.Millisecond);

		private readonly int _width;

		private readonly int _height;

		private IEnumerable<Polygon> _polygons;

		private List<Bitmap> _bitmaps;

		public Background(int width, int height)
		{
			_width = width;
			_height = height;
			InitializeBitmaps();
			InitializePolygons();
		}

		public ImageBrush Draw()
		{
			int polygonCount = _random.Next(3, 6);
			var background = new Bitmap(_width, _height);

			for (int i = 0; i < polygonCount; i++)
			{
				var polygon = _polygons.ElementAt(i);
				polygon.EdgesSortFill(_bitmaps[_random.Next() % _bitmaps.Count], background, 180);
			}

			return background.CreateImageBrush();
		}

		private void InitializeBitmaps()
		{
			_bitmaps = new List<Bitmap>
			{
				Properties.Resources.Hexagons,
				Properties.Resources.Material,
				Properties.Resources.Panther
			};
		}

		private void InitializePolygons()
		{
			_polygons = new List<Polygon>
			{
				new Polygon(new[] { new Point(100, 100), new Point(100, 300), new Point(200, 400), new Point(200, 150), new Point(150, 200) }),
				new Polygon(new[] { new Point(800, 300), new Point(700, 400), new Point(700, 600), new Point(800, 500), new Point(1000, 500) }),
				new Polygon(new[] { new Point(900, 50), new Point(700, 270), new Point(500, 180), new Point(400, 260), new Point(400, 140) }),
				new Polygon(new[] { new Point(950, 200), new Point(1050, 400), new Point(1300, 600), new Point(1250, 400), new Point(1275, 335), new Point(1100, 140) }),
				new Polygon(new[] { new Point(375, 350), new Point(475, 473), new Point(550, 570), new Point(375, 550), new Point(275, 250) }),
				new Polygon(new[] { new Point(100, 500), new Point(160, 523), new Point(170, 620), new Point(150, 600), new Point(140, 550) })
			};
		}
	}
}