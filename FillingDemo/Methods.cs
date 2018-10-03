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

namespace FillingDemo
{
	partial class MainWindow
    {
        private void InitializePolygons()
        {
            GraphicsPath graphicsPath = new GraphicsPath();
            _polygonGraphics = new List<GraphicsPath>();
            _visiblePolygonGraphics = new List<GraphicsPath>();

            //graphicsPath.AddLine(new System.Drawing.Point(100, 100), new System.Drawing.Point(150, 200));
            //graphicsPath.AddLine(new System.Drawing.Point(150, 200), new System.Drawing.Point(200, 150));
            //graphicsPath.AddLine(new System.Drawing.Point(200, 150), new System.Drawing.Point(200, 400));
            //graphicsPath.AddLine(new System.Drawing.Point(200, 400), new System.Drawing.Point(100, 300));
            //graphicsPath.AddLine(new System.Drawing.Point(100, 300), new System.Drawing.Point(100, 300));

            graphicsPath.AddLine(new System.Drawing.Point(100, 100), new System.Drawing.Point(100, 300));
            graphicsPath.AddLine(new System.Drawing.Point(100, 300), new System.Drawing.Point(200, 400));
            graphicsPath.AddLine(new System.Drawing.Point(200, 400), new System.Drawing.Point(200, 150));
            graphicsPath.AddLine(new System.Drawing.Point(200, 150), new System.Drawing.Point(150, 200));
            graphicsPath.AddLine(new System.Drawing.Point(150, 200), new System.Drawing.Point(100, 100));
            _polygonGraphics.Add(graphicsPath);

            graphicsPath = new GraphicsPath();
            graphicsPath.AddLine(new System.Drawing.Point(800, 300), new System.Drawing.Point(700, 400));
            graphicsPath.AddLine(new System.Drawing.Point(700, 400), new System.Drawing.Point(700, 600));
            graphicsPath.AddLine(new System.Drawing.Point(700, 600), new System.Drawing.Point(800, 500));
            graphicsPath.AddLine(new System.Drawing.Point(800, 500), new System.Drawing.Point(1000, 500));
            graphicsPath.AddLine(new System.Drawing.Point(1000, 500), new System.Drawing.Point(800, 300));
            _polygonGraphics.Add(graphicsPath);

            graphicsPath = new GraphicsPath();
            graphicsPath.AddLine(new System.Drawing.Point(900, 50), new System.Drawing.Point(700, 270));
            graphicsPath.AddLine(new System.Drawing.Point(700, 270), new System.Drawing.Point(500, 180));
            graphicsPath.AddLine(new System.Drawing.Point(500, 180), new System.Drawing.Point(400, 260));
            graphicsPath.AddLine(new System.Drawing.Point(400, 260), new System.Drawing.Point(400, 140));
            graphicsPath.AddLine(new System.Drawing.Point(400, 140), new System.Drawing.Point(900, 50));
            _polygonGraphics.Add(graphicsPath);

            graphicsPath = new GraphicsPath();
            graphicsPath.AddLine(new System.Drawing.Point(950, 200), new System.Drawing.Point(1050, 400));
            graphicsPath.AddLine(new System.Drawing.Point(1050, 400), new System.Drawing.Point(1300, 600));
            graphicsPath.AddLine(new System.Drawing.Point(1300, 600), new System.Drawing.Point(1250, 400));
            graphicsPath.AddLine(new System.Drawing.Point(1250, 400), new System.Drawing.Point(1275, 335));
            graphicsPath.AddLine(new System.Drawing.Point(1275, 335), new System.Drawing.Point(1100, 140));
            graphicsPath.AddLine(new System.Drawing.Point(1100, 140), new System.Drawing.Point(950, 200));
            _polygonGraphics.Add(graphicsPath);

            graphicsPath = new GraphicsPath();
            graphicsPath.AddLine(new System.Drawing.Point(375, 350), new System.Drawing.Point(475, 473));
            graphicsPath.AddLine(new System.Drawing.Point(475, 473), new System.Drawing.Point(550, 570));
            graphicsPath.AddLine(new System.Drawing.Point(550, 570), new System.Drawing.Point(375, 550));
            graphicsPath.AddLine(new System.Drawing.Point(375, 550), new System.Drawing.Point(275, 250));
            graphicsPath.AddLine(new System.Drawing.Point(275, 250), new System.Drawing.Point(375, 350));
            _polygonGraphics.Add(graphicsPath);

            graphicsPath = new GraphicsPath();
            graphicsPath.AddLine(new System.Drawing.Point(100, 500), new System.Drawing.Point(160, 523));
            graphicsPath.AddLine(new System.Drawing.Point(160, 523), new System.Drawing.Point(170, 620));
            graphicsPath.AddLine(new System.Drawing.Point(170, 620), new System.Drawing.Point(150, 600));
            graphicsPath.AddLine(new System.Drawing.Point(150, 600), new System.Drawing.Point(140, 550));
            graphicsPath.AddLine(new System.Drawing.Point(140, 550), new System.Drawing.Point(100, 500));
            _polygonGraphics.Add(graphicsPath);
        }

        private void FillPolygons()
        {
            Random random = new Random(DateTime.Now.Millisecond);
            int intRandom = 3 + random.Next() % 4;
            _background = new Bitmap((int)DrawingCanvas.ActualWidth, (int)DrawingCanvas.ActualHeight);

            for (int i = 0; i < intRandom; i++)
            {
                GraphicsPath polygon = _polygonGraphics[i];

                byte[] pointTypes = new byte[polygon.PointCount];
                for (int j = 0; j < pointTypes.Length; j++)
                    pointTypes[j] = 1;

                Point[] points = Array.ConvertAll(polygon.PathPoints, PointFToPoint);

                try
                {
                    EdgesSortFill(_bitmaps[random.Next() % intRandom], _background, points, pointTypes, 180);
                }
                catch (Exception)
                {
                    Bitmap colourBitmap = new Bitmap(1, 1);
                    colourBitmap.SetPixel(0, 0, System.Drawing.Color.FromArgb(255, random.Next() % 255
                        , random.Next() % 255, random.Next() % 255));
                    EdgesSortFill(colourBitmap, _background, points, pointTypes, 180);
                }

                _visiblePolygonGraphics.Add(polygon);
            }
            DrawingCanvas.Background = _background.CreateImageBrush();
        }

        private void EdgesSortFill(Bitmap brushBitmap, Bitmap bitmap, Point[] points, byte[] pointTypes, int opacity)
        {
            List<PointInfo> activePointsList = new List<PointInfo>();
            List<PointInfo> pointInfoList;

            CreateActiveEdgesList(points, out pointInfoList, pointTypes);
            pointInfoList.Sort((p, q) => (p.StartPoint.Y >= q.StartPoint.Y) ? 1 : -1);

            int scanLine = (int)pointInfoList[0].StartPoint.Y;

            do
            {
                while (pointInfoList.Count > 0 && (int)pointInfoList[0].CurrentPoint.Y == scanLine)
                {
                    // Ignore horizontal lines.
                    if (pointInfoList[0].StartPoint.Y != pointInfoList[0].EndPoint.Y)
                        activePointsList.Add(pointInfoList[0]);
                    pointInfoList.Remove(pointInfoList[0]);
                }

                activePointsList.Sort((p, q) => (p.CurrentPoint.X >= q.CurrentPoint.X) ? 1 : -1);

                for (int i = 0; i < activePointsList.Count - 1; i += 2)
                {
                    PointInfo currentPointInfo = activePointsList[i];
                    PointInfo nextPointInfo = activePointsList[i + 1];

                    if (currentPointInfo.CurrentPoint.Y == currentPointInfo.EndPoint.Y)
                    {
                        i -= 2;
                        activePointsList.Remove(currentPointInfo);
                        continue;
                    }

                    if (nextPointInfo.CurrentPoint.Y == nextPointInfo.EndPoint.Y)
                    {
                        i -= 2;
                        activePointsList.Remove(nextPointInfo);
                        continue;
                    }

                    int startX, endX;
                    startX = (int)currentPointInfo.CurrentPoint.X;
                    endX = (int)nextPointInfo.CurrentPoint.X;

                    for (int k = startX; k < endX; k++)
                    {
                        System.Drawing.Color color = brushBitmap.GetPixel(k % brushBitmap.Width,
                            scanLine % brushBitmap.Height);
                        color = System.Drawing.Color.FromArgb(opacity % 256, color.R, color.G, color.B);
                        bitmap.SetPixel(k, scanLine, color);
                    }
                }

                List<PointInfo> pointsToRemove = new List<PointInfo>();

                for (int i = 0; i < activePointsList.Count; i++)
                    if ((int)activePointsList[i].EndPoint.Y == scanLine)
                        pointsToRemove.Add(activePointsList[i]);

                for (int i = 0; i < pointsToRemove.Count; i++)
                    activePointsList.Remove(pointsToRemove[i]);

                scanLine++;

                foreach (PointInfo pointInfo in activePointsList)
                {
                    Point tmpPoint = new Point(pointInfo.CurrentPoint.X + pointInfo.Delta,
                        pointInfo.CurrentPoint.Y + 1);
                    pointInfo.CurrentPoint = tmpPoint;
                }

            } while (pointInfoList.Count != 0 || activePointsList.Count != 0);
        }

        private void CreateActiveEdgesList(Point[] points, out List<PointInfo> pointInfoList, byte[] pointTypes)
        {
            pointInfoList = new List<PointInfo>();

            for (int i = 0; i < points.Length; i++)
            {
                List<Point> helperNextPointList = new List<Point>();
                List<PointInfo> helperCurrentPointList = new List<PointInfo>();
                bool isFirstPoint = true;

                while (i < pointTypes.Length && (pointTypes[i] != 0 || isFirstPoint))
                {
                    isFirstPoint = false;
                    PointInfo pointInfo = new PointInfo();
                    Point startPoint = new Point(points[i].X, points[i].Y);
                    pointInfo.StartPoint = startPoint;
                    helperCurrentPointList.Add(pointInfo);
                    helperNextPointList.Add(points[i]);
                    i++;
                }

                if (i < pointTypes.Length && pointTypes[i] == 0)
                    i--;

                for (int j = 0; j < helperCurrentPointList.Count; j++)
                {
                    Point tmpPoint = helperNextPointList[(j + 1) % helperCurrentPointList.Count];

                    // Keep the point with lower y coordinate the m_startPoint.
                    if (helperCurrentPointList[j].StartPoint.Y > tmpPoint.Y)
                    {
                        helperCurrentPointList[j].EndPoint = helperCurrentPointList[j].StartPoint;
                        helperCurrentPointList[j].StartPoint = tmpPoint;
                    }
                    else
                        helperCurrentPointList[j].EndPoint = tmpPoint;

                    double deltaX = helperCurrentPointList[j].EndPoint.X - helperCurrentPointList[j].StartPoint.X;
                    double deltaY = helperCurrentPointList[j].EndPoint.Y - helperCurrentPointList[j].StartPoint.Y;

                    // Calculate the difference between next x-coordinates.
                    if (deltaX != 0 && deltaY != 0)
                        helperCurrentPointList[j].Delta = 1 / (deltaY / deltaX);
                    else
                        helperCurrentPointList[j].Delta = 0;

                    helperCurrentPointList[j].CurrentPoint = helperCurrentPointList[j].StartPoint;
                    pointInfoList.Add(helperCurrentPointList[j]);
                }
            }
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
            Point[] points = Array.ConvertAll(floatPoints, PointFToPoint);

            Random random = new Random(DateTime.Now.Millisecond);

            Bitmap colourBitmap = new Bitmap(1, 1);
            colourBitmap.SetPixel(0, 0, System.Drawing.Color.FromArgb(255, _color.R
                , _color.G, _color.B));
            try
            {
                EdgesSortFill(colourBitmap, bitmap, points, pointTypes, 255);
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

        public static Point PointFToPoint(PointF pointF)
        {
            return new Point(((int)pointF.X), ((int)pointF.Y));
        }

        public static PointF PointToPointF(Point point)
        {
            return new PointF(((float)point.X), ((float)point.Y));
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

        private void WeilerAtherton()
        {
            Vector offset = VisualTreeHelper.GetOffset(_textCanvas);

            foreach (GraphicsPath polygonGraphic in _visiblePolygonGraphics)
            {
                RectangleF polygonRectangle = polygonGraphic.GetBounds();

                if (offset.X > polygonRectangle.X + polygonRectangle.Width
                    || offset.X + _textCanvas.Width < polygonRectangle.X
                    || offset.Y > polygonRectangle.Y + polygonRectangle.Height
                    || offset.Y + _textCanvas.Height < polygonRectangle.Y)
                    continue;

                Point[] polygonPoints = Array.ConvertAll(polygonGraphic.PathPoints, PointFToPoint);
                List<PointInfo> polygonPointList;

                CreateActiveEdgesList(polygonPoints, out polygonPointList, polygonGraphic.PathTypes);

                Point[] textPoints = Array.ConvertAll(_textGraphicsPath.PathPoints, PointFToPoint);
                List<PointInfo> textPointList;

                CreateActiveEdgesList(textPoints, out textPointList, _textGraphicsPath.PathTypes);

                List<Point> intersectingPoints = new List<Point>();
                LinkedList<PathPoint> currentPolygonPointsList = new LinkedList<PathPoint>();
                LinkedList<PathPoint> currentTextPointsList = new LinkedList<PathPoint>();

                for (int j = 0; j < polygonPoints.Length; j++)
                    currentPolygonPointsList.AddLast(new PathPoint(polygonPoints[j], polygonGraphic.PathTypes[j]));
                for (int j = 0; j < textPoints.Length; j++)
                    currentTextPointsList.AddLast(new PathPoint(textPoints[j], _textGraphicsPath.PathTypes[j]));

                foreach (var textLine in textPointList)
                {
                    PointInfo pointInfo = new PointInfo();
                    pointInfo.StartPoint = new Point(offset.X + textLine.StartPoint.X, offset.Y + textLine.StartPoint.Y);
                    pointInfo.EndPoint = new Point(offset.X + textLine.EndPoint.X, offset.Y + textLine.EndPoint.Y);
                    pointInfo.Delta = textLine.Delta;

                    foreach (var polygonLine in polygonPointList)
                    {
                        Point foundPoint = DoLinesIntersect(pointInfo, polygonLine);
                        if (foundPoint.X != -1)
                        {
                            intersectingPoints.Add(foundPoint);
                            LinkedListNode<PathPoint> node = null;

                            foreach (PathPoint pathPoint in currentPolygonPointsList)
                                if (pathPoint.Point == polygonLine.EndPoint)
                                {
                                    node = currentPolygonPointsList.Find(pathPoint);
                                    break;
                                }

                            currentPolygonPointsList.AddAfter(node, new PathPoint(foundPoint, 1));
                            node = null;

                            foreach (PathPoint textPoint in currentTextPointsList)
                                if (textPoint.Point == textLine.EndPoint)
                                {
                                    node = currentTextPointsList.Find(textPoint);
                                    break;
                                }

                            currentTextPointsList.AddAfter(node, new PathPoint(new Point(foundPoint.X - offset.X, foundPoint.Y - offset.Y), 1));
                        }
                    }
                }

                if (intersectingPoints.Count == 0)
                    continue;

                polygonPoints = new Point[currentPolygonPointsList.Count];
                textPoints = new Point[currentTextPointsList.Count];
                byte[] polygonBytes = new byte[currentPolygonPointsList.Count];
                byte[] textBytes = new byte[currentTextPointsList.Count];

                PathPoint[] polygonPointsArray = currentPolygonPointsList.ToArray();
                PathPoint[] textPointsArray = currentTextPointsList.ToArray();

                for (int j = 0; j < currentPolygonPointsList.Count; j++)
                {
                    polygonPoints[j] = polygonPointsArray[j].Point;
                    polygonBytes[j] = polygonPointsArray[j].Type;
                }
                for (int j = 0; j < currentTextPointsList.Count; j++)
                {
                    textPoints[j] = textPointsArray[j].Point;
                    textBytes[j] = textPointsArray[j].Type;
                }

                polygonPointList = new List<PointInfo>();
                textPointList = new List<PointInfo>();

                CreateActiveEdgesList(polygonPoints, out polygonPointList, polygonBytes);
                CreateActiveEdgesList(textPoints, out textPointList, textBytes);

                List<Point> entryList = new List<Point>();
                for (int j = 0; j < intersectingPoints.Count; j += 2)
                    entryList.Add(intersectingPoints[j]);

                int i = 0;
                List<PathPoint> newFigurePoints = new List<PathPoint>();
                Point startPoint = intersectingPoints[0];
                newFigurePoints.Add(new PathPoint(intersectingPoints[0], 0));
                intersectingPoints.Remove(intersectingPoints[0]);
                entryList.Remove(entryList[0]);
                Point lastPoint = startPoint;

                do
                {
                    do
                    {
                        if (i == 0)
                        {
                            int currentPointIndex = (polygonPointList.FindIndex(p => p.StartPoint.Equals(lastPoint)) + 1);
                            Point currentPolygonPoint = polygonPointList[currentPointIndex].StartPoint;
                            do
                            {
                                currentPointIndex = (polygonPointList.FindIndex(p => p.StartPoint.Equals(lastPoint)) + 1);
                                currentPolygonPoint = polygonPointList[currentPointIndex].StartPoint;
                                lastPoint = currentPolygonPoint;

                                if (lastPoint == startPoint)
                                    continue;

                                newFigurePoints.Add(new PathPoint(currentPolygonPoint, 1));
                                currentPointIndex++;
                            } while ((intersectingPoints.Count > 0 && lastPoint != intersectingPoints[0])
                                || (intersectingPoints.Count == 0 && lastPoint != startPoint));
                        }
                        else
                        {
                            int currentTextIndex = (textPointList.FindIndex(
                                p => p.StartPoint.Equals(new Point(lastPoint.X - offset.X, lastPoint.Y - offset.Y))) + 1) % textPointList.Count;
                            Point currentTextPoint = textPointList[currentTextIndex].StartPoint;
                            do
                            {
                                currentTextIndex = (textPointList.FindIndex(
                                p => p.StartPoint.Equals(new Point(lastPoint.X - offset.X, lastPoint.Y - offset.Y))) + 1) % textPointList.Count;
                                currentTextPoint = new Point(textPointList[currentTextIndex].StartPoint.X + offset.X
                                    , textPointList[currentTextIndex].StartPoint.Y + offset.Y);
                                lastPoint = currentTextPoint;

                                if (lastPoint == startPoint)
                                    continue;

                                newFigurePoints.Add(new PathPoint(currentTextPoint, 1));
                                currentTextIndex++;
                            } while ((intersectingPoints.Count > 0 && lastPoint != intersectingPoints[0])
                                || (intersectingPoints.Count == 0 && lastPoint != startPoint));
                        }
                        intersectingPoints.Remove(lastPoint);
                        i = (i + 1) % 2;
                    } while (lastPoint != startPoint);
                } while (entryList.Count > 0);

                return;
                // PointF[] points = Array.ConvertAll(newFigurePoints.ToArray(), new Converter<Point, PointF>(pointToPointF));

            }
        }
    }
}
