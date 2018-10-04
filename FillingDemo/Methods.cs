using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using FillingDemo.Helpers;
using Point = System.Windows.Point;
using Polygon = FillingDemo.Shapes.Polygon;
using Shape = FillingDemo.Shapes.Shape;

namespace FillingDemo
{
	partial class MainWindow
	{
		private readonly Random _random = new Random(DateTime.Now.Millisecond);

		private IEnumerable<Polygon> _polygons;

        private void InitializePolygons()
        {
            _polygonGraphics = new List<GraphicsPath>();
            _visiblePolygonGraphics = new List<GraphicsPath>();

			var polygon1 = new[]
			{
				new Point(100, 100),
		        new Point(100, 300),
		        new Point(200, 400),
		        new Point(200, 150),
		        new Point(150, 200)
	        };

			var polygon2 = new[]
			{
				new Point(800, 300),
		        new Point(700, 400),
		        new Point(700, 600),
		        new Point(800, 500),
				new Point(1000, 500)
			};

			var polygon3 = new[]
			{
				new Point(900, 50),
		        new Point(700, 270),
		        new Point(500, 180),
		        new Point(400, 260),
				new Point(400, 140)
			};

			var polygon4 = new[]
			{
				new Point(950, 200),
		        new Point(1050, 400),
		        new Point(1300, 600),
		        new Point(1250, 400),
				new Point(1275, 335),
				new Point(1100, 140)
			};

			var polygon5 = new[]
			{
				new Point(375, 350),
				new Point(475, 473),
		        new Point(550, 570),
		        new Point(375, 550),
				new Point(275, 250)
			};

			var polygon6 = new[]
			{
				new Point(100, 500),
				new Point(160, 523),
		        new Point(170, 620),
		        new Point(150, 600),
				new Point(140, 550)
			};

	        _polygons = new[]
	        {
				new Polygon(polygon1),
				new Polygon(polygon2),
				new Polygon(polygon3),
				new Polygon(polygon4),
				new Polygon(polygon5),
				new Polygon(polygon6)
	        };

	        _polygonGraphics = _polygons.Select(p => p.GraphicsPath).ToList();
		}

		private void FillPolygons()
		{
			int polygonCount = _random.Next(3, 6);
			_background = new Bitmap((int)DrawingCanvas.ActualWidth, (int)DrawingCanvas.ActualHeight);

			for (int i = 0; i < 6; i++)
			{
				var polygon = _polygons.ElementAt(i);

				try
				{
					polygon.EdgesSortFill(_bitmaps[_random.Next() % polygonCount], _background, 180);
				}
				catch (Exception)
				{
					Bitmap bitmap = new Bitmap(1, 1);
					bitmap.SetPixel(0, 0, System.Drawing.Color.FromArgb(255, _random.Next(255), _random.Next(255), _random.Next(255)));
					polygon.EdgesSortFill(bitmap, _background, 180);
				}

				_visiblePolygonGraphics.Add(polygon.GraphicsPath);
			}
			DrawingCanvas.Background = _background.CreateImageBrush();
		}

		/// <summary>
		/// Second way to create path from text.
		/// Actually the function is never called. 
		/// I put it here jus to keep in mind that there is another way to convert text to graphics.
		/// </summary>
		private void ConvertTextToSmootherGraphics()
        {
            FormattedText text = new FormattedText(InputTextBox.Text, Thread.CurrentThread.CurrentUICulture,
                FlowDirection.LeftToRight,
                new Typeface(InputTextBox.FontFamily, InputTextBox.FontStyle, InputTextBox.FontWeight,
                    InputTextBox.FontStretch),
                double.Parse(SizeTextBox.Text), ColorCanvas.Background);

            Geometry textGeometry = text.BuildGeometry(new Point(0, 0));

            Path path = new Path();
            path.Fill = ColorCanvas.Background;
            path.Data = textGeometry;
            Canvas.SetZIndex(_textCanvas, int.MaxValue);
            DrawingCanvas.Children.Add(path);

            PathGeometry m_pathGeometry = textGeometry.GetOutlinedPathGeometry();
        }

        private void ConvertTextToGraphics()
        {
            System.Drawing.FontFamily fontFamily = System.Drawing.FontFamily.GenericSerif;
            int fontStyle = (int)System.Drawing.FontStyle.Bold;

            GraphicsPath graphicsPath = new GraphicsPath();
            graphicsPath.AddString(InputTextBox.Text, fontFamily, fontStyle, _textSize
                , new System.Drawing.Point(0, 0), StringFormat.GenericDefault);
            graphicsPath.Flatten();
            PointF[] floatPoints = graphicsPath.PathPoints;
            var pointTypes = graphicsPath.PathTypes;

            RectangleF boundRect = graphicsPath.GetBounds();

            _textCanvas = new Canvas();
            _textCanvas.Width = (int)(boundRect.Size.Width + boundRect.X + 1);
            _textCanvas.Height = (int)(boundRect.Size.Height + boundRect.Y + 1);
            Bitmap bitmap = new Bitmap((int)(boundRect.Size.Width + boundRect.X + 1),
                (int)(boundRect.Size.Height + boundRect.Y + 1));
            var points = floatPoints.Select(p => p.ToWindowsPoint());

            Random random = new Random(DateTime.Now.Millisecond);

            Bitmap colourBitmap = new Bitmap(1, 1);
            colourBitmap.SetPixel(0, 0, System.Drawing.Color.FromArgb(255, _color.R
                , _color.G, _color.B));
            try
            {
	            Shape.EdgesSortFill(colourBitmap, bitmap, points, pointTypes, 255);
            }
            catch (Exception)
            {
                MessageBox.Show("The text you provided is too long or the font is to big.");
                _textCanvas = new Canvas();
                return;
            }

            _textGraphicsPath = graphicsPath;
            _textCanvas.Background = bitmap.CreateImageBrush();
            DrawingCanvas.Children.Add(_textCanvas);
            _textCanvas.MouseDown += TextCanvas_MouseDown;
        }

        public Point DoLinesIntersect(PointInfo lineA, PointInfo lineB)
        {
            double deltaA, deltaB;
            if (lineA.Delta == 0)
                deltaA = 0;
            else
                deltaA = 1 / lineA.Delta;
            if (lineB.Delta == 0)
                deltaB = 0;
            else
                deltaB = 1 / lineB.Delta;

            // Lines are paralel or perpendicular.
            if (deltaA == deltaB)
            {
                if (!((lineA.StartPoint.Y == lineA.EndPoint.Y && lineB.StartPoint.Y == lineB.EndPoint.Y)
                      || (lineA.StartPoint.X == lineA.EndPoint.X && lineB.StartPoint.X == lineB.EndPoint.X)))
                {
                    double X, Y;
                    if (lineA.StartPoint.Y == lineA.EndPoint.Y)
                    {
                        X = lineB.StartPoint.X;
                        Y = lineA.StartPoint.Y;
                    }
                    else
                    {
                        X = lineA.StartPoint.X;
                        Y = lineB.StartPoint.Y;
                    }

                    if (Math.Min(lineA.StartPoint.X, lineA.EndPoint.X) <= X
                        && X <= Math.Max(lineA.StartPoint.X, lineA.EndPoint.X)
                        && Math.Min(lineB.StartPoint.X, lineB.EndPoint.X) <= X
                        && X <= Math.Max(lineB.StartPoint.X, lineB.EndPoint.X)
                        && Math.Min(lineA.StartPoint.Y, lineA.EndPoint.Y) <= Y
                        && Y <= Math.Max(lineA.StartPoint.Y, lineA.EndPoint.Y)
                        && Math.Min(lineB.StartPoint.Y, lineB.EndPoint.Y) <= Y
                        && Y <= Math.Max(lineB.StartPoint.Y, lineB.EndPoint.Y))
                        return new Point(X, Y);
                }
                return new Point(-1, -1);
            }

            double bA = lineA.StartPoint.Y - deltaA * lineA.StartPoint.X;
            double bB = lineB.StartPoint.Y - deltaB * lineB.StartPoint.X;

            double x = (bB - bA) / (deltaA - deltaB);
            double y = deltaA * x + bA;

            if (Math.Min(lineA.StartPoint.X, lineA.EndPoint.X) <= x
                && x <= Math.Max(lineA.StartPoint.X, lineA.EndPoint.X)
                && Math.Min(lineB.StartPoint.X, lineB.EndPoint.X) <= x
                && x <= Math.Max(lineB.StartPoint.X, lineB.EndPoint.X))
                return new Point(x, y);

            return new Point(-1, -1);
        }

    //    private void WeilerAtherton()
    //    {
    //        Vector offset = VisualTreeHelper.GetOffset(_textCanvas);

    //        foreach (GraphicsPath polygonGraphic in _visiblePolygonGraphics)
    //        {
    //            RectangleF polygonRectangle = polygonGraphic.GetBounds();

    //            if (offset.X > polygonRectangle.X + polygonRectangle.Width
    //                || offset.X + _textCanvas.Width < polygonRectangle.X
    //                || offset.Y > polygonRectangle.Y + polygonRectangle.Height
    //                || offset.Y + _textCanvas.Height < polygonRectangle.Y)
    //                continue;

    //            var polygonPoints = polygonGraphic.PathPoints.Select(p => p.ToWindowsPoint());
    //            var polygonPointList = Shape.CreateActiveEdgesList(polygonPoints, polygonGraphic.PathTypes);

    //            var textPoints = _textGraphicsPath.PathPoints.Select(p => p.ToWindowsPoint());
    //            var textPointList = Shape.CreateActiveEdgesList(textPoints, _textGraphicsPath.PathTypes);

    //            List<Point> intersectingPoints = new List<Point>();
    //            LinkedList<PathPoint> currentPolygonPointsList = new LinkedList<PathPoint>();
    //            LinkedList<PathPoint> currentTextPointsList = new LinkedList<PathPoint>();

    //            for (int j = 0; j < polygonPoints.Count(); j++)
    //                currentPolygonPointsList.AddLast(new PathPoint(polygonPoints.ElementAt(j), polygonGraphic.PathTypes[j]));
    //            for (int j = 0; j < textPoints.Count(); j++)
    //                currentTextPointsList.AddLast(new PathPoint(textPoints.ElementAt(j), _textGraphicsPath.PathTypes[j]));

    //            foreach (var textLine in textPointList)
    //            {
    //                PointInfo pointInfo = new PointInfo();
    //                pointInfo.StartPoint = new Point(offset.X + textLine.StartPoint.X, offset.Y + textLine.StartPoint.Y);
    //                pointInfo.EndPoint = new Point(offset.X + textLine.EndPoint.X, offset.Y + textLine.EndPoint.Y);
    //                pointInfo.Delta = textLine.Delta;

    //                foreach (var polygonLine in polygonPointList)
    //                {
    //                    Point foundPoint = DoLinesIntersect(pointInfo, polygonLine);
    //                    if (foundPoint.X != -1)
    //                    {
    //                        intersectingPoints.Add(foundPoint);
    //                        LinkedListNode<PathPoint> node = null;

    //                        foreach (PathPoint pathPoint in currentPolygonPointsList)
    //                            if (pathPoint.Point == polygonLine.EndPoint)
    //                            {
    //                                node = currentPolygonPointsList.Find(pathPoint);
    //                                break;
    //                            }

    //                        currentPolygonPointsList.AddAfter(node, new PathPoint(foundPoint, 1));
    //                        node = null;

    //                        foreach (PathPoint textPoint in currentTextPointsList)
    //                            if (textPoint.Point == textLine.EndPoint)
    //                            {
    //                                node = currentTextPointsList.Find(textPoint);
    //                                break;
    //                            }

    //                        currentTextPointsList.AddAfter(node, new PathPoint(new Point(foundPoint.X - offset.X, foundPoint.Y - offset.Y), 1));
    //                    }
    //                }
    //            }

    //            if (intersectingPoints.Count == 0)
    //                continue;

    //            polygonPoints = new Point[currentPolygonPointsList.Count];
    //            textPoints = new Point[currentTextPointsList.Count];
    //            byte[] polygonBytes = new byte[currentPolygonPointsList.Count];
    //            byte[] textBytes = new byte[currentTextPointsList.Count];

    //            PathPoint[] polygonPointsArray = currentPolygonPointsList.ToArray();
    //            PathPoint[] textPointsArray = currentTextPointsList.ToArray();

    //            for (int j = 0; j < currentPolygonPointsList.Count; j++)
    //            {
    //                polygonPoints.ElementAt(j) = polygonPointsArray[j].Point;
    //                polygonBytes[j] = polygonPointsArray[j].Type;
    //            }
    //            for (int j = 0; j < currentTextPointsList.Count; j++)
    //            {
    //                textPoints.ElementAt(j) = textPointsArray[j].Point;
    //                textBytes[j] = textPointsArray[j].Type;
    //            }

    //            polygonPointList = Shape.CreateActiveEdgesList(polygonPoints, polygonBytes);
				//textPointList = Shape.CreateActiveEdgesList(textPoints, textBytes);

				//List<Point> entryList = new List<Point>();
    //            for (int j = 0; j < intersectingPoints.Count; j += 2)
    //                entryList.Add(intersectingPoints[j]);

    //            int i = 0;
    //            List<PathPoint> newFigurePoints = new List<PathPoint>();
    //            Point startPoint = intersectingPoints[0];
    //            newFigurePoints.Add(new PathPoint(intersectingPoints[0], 0));
    //            intersectingPoints.Remove(intersectingPoints[0]);
    //            entryList.Remove(entryList[0]);
    //            Point lastPoint = startPoint;

    //            do
    //            {
    //                do
    //                {
    //                    if (i == 0)
    //                    {
    //                        int currentPointIndex = (polygonPointList.FindIndex(p => p.StartPoint.Equals(lastPoint)) + 1);
    //                        Point currentPolygonPoint = polygonPointList[currentPointIndex].StartPoint;
    //                        do
    //                        {
    //                            currentPointIndex = (polygonPointList.FindIndex(p => p.StartPoint.Equals(lastPoint)) + 1);
    //                            currentPolygonPoint = polygonPointList[currentPointIndex].StartPoint;
    //                            lastPoint = currentPolygonPoint;

    //                            if (lastPoint == startPoint)
    //                                continue;

    //                            newFigurePoints.Add(new PathPoint(currentPolygonPoint, 1));
    //                            currentPointIndex++;
    //                        } while ((intersectingPoints.Count > 0 && lastPoint != intersectingPoints[0])
    //                            || (intersectingPoints.Count == 0 && lastPoint != startPoint));
    //                    }
    //                    else
    //                    {
    //                        int currentTextIndex = (textPointList.FindIndex(
    //                            p => p.StartPoint.Equals(new Point(lastPoint.X - offset.X, lastPoint.Y - offset.Y))) + 1) % textPointList.Count;
    //                        Point currentTextPoint = textPointList[currentTextIndex].StartPoint;
    //                        do
    //                        {
    //                            currentTextIndex = (textPointList.FindIndex(
    //                            p => p.StartPoint.Equals(new Point(lastPoint.X - offset.X, lastPoint.Y - offset.Y))) + 1) % textPointList.Count;
    //                            currentTextPoint = new Point(textPointList[currentTextIndex].StartPoint.X + offset.X
    //                                , textPointList[currentTextIndex].StartPoint.Y + offset.Y);
    //                            lastPoint = currentTextPoint;

    //                            if (lastPoint == startPoint)
    //                                continue;

    //                            newFigurePoints.Add(new PathPoint(currentTextPoint, 1));
    //                            currentTextIndex++;
    //                        } while ((intersectingPoints.Count > 0 && lastPoint != intersectingPoints[0])
    //                            || (intersectingPoints.Count == 0 && lastPoint != startPoint));
    //                    }
    //                    intersectingPoints.Remove(lastPoint);
    //                    i = (i + 1) % 2;
    //                } while (lastPoint != startPoint);
    //            } while (entryList.Count > 0);

    //            return;
    //            // PointF[] points = Array.ConvertAll(newFigurePoints.ToArray(), new Converter<Point, PointF>(pointToPointF));

    //        }
    //    }
    }
}
