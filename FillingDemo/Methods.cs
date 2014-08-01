using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Security.AccessControl;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Point = System.Windows.Point;

namespace FillingDemo
{
    partial class MainWindow
    {
        private static ImageBrush createImageBrushFromBitmap(Bitmap bitmap)
        {
            BitmapSource bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(
                bitmap.GetHbitmap(),
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());

            return new ImageBrush(bitmapSource);
        }

        private void initializePolygons()
        {
            GraphicsPath graphicsPath = new GraphicsPath();
            m_polygonGraphics = new List<GraphicsPath>();
            m_visiblePolygonGraphics = new List<GraphicsPath>();

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
            m_polygonGraphics.Add(graphicsPath);

            graphicsPath = new GraphicsPath();
            graphicsPath.AddLine(new System.Drawing.Point(800, 300), new System.Drawing.Point(700, 400));
            graphicsPath.AddLine(new System.Drawing.Point(700, 400), new System.Drawing.Point(700, 600));
            graphicsPath.AddLine(new System.Drawing.Point(700, 600), new System.Drawing.Point(800, 500));
            graphicsPath.AddLine(new System.Drawing.Point(800, 500), new System.Drawing.Point(1000, 500));
            graphicsPath.AddLine(new System.Drawing.Point(1000, 500), new System.Drawing.Point(800, 300));
            m_polygonGraphics.Add(graphicsPath);

            graphicsPath = new GraphicsPath();
            graphicsPath.AddLine(new System.Drawing.Point(900, 50), new System.Drawing.Point(700, 270));
            graphicsPath.AddLine(new System.Drawing.Point(700, 270), new System.Drawing.Point(500, 180));
            graphicsPath.AddLine(new System.Drawing.Point(500, 180), new System.Drawing.Point(400, 260));
            graphicsPath.AddLine(new System.Drawing.Point(400, 260), new System.Drawing.Point(400, 140));
            graphicsPath.AddLine(new System.Drawing.Point(400, 140), new System.Drawing.Point(900, 50));
            m_polygonGraphics.Add(graphicsPath);

            graphicsPath = new GraphicsPath();
            graphicsPath.AddLine(new System.Drawing.Point(950, 200), new System.Drawing.Point(1050, 400));
            graphicsPath.AddLine(new System.Drawing.Point(1050, 400), new System.Drawing.Point(1300, 600));
            graphicsPath.AddLine(new System.Drawing.Point(1300, 600), new System.Drawing.Point(1250, 400));
            graphicsPath.AddLine(new System.Drawing.Point(1250, 400), new System.Drawing.Point(1275, 335));
            graphicsPath.AddLine(new System.Drawing.Point(1275, 335), new System.Drawing.Point(1100, 140));
            graphicsPath.AddLine(new System.Drawing.Point(1100, 140), new System.Drawing.Point(950, 200));
            m_polygonGraphics.Add(graphicsPath);

            graphicsPath = new GraphicsPath();
            graphicsPath.AddLine(new System.Drawing.Point(375, 350), new System.Drawing.Point(475, 473));
            graphicsPath.AddLine(new System.Drawing.Point(475, 473), new System.Drawing.Point(550, 570));
            graphicsPath.AddLine(new System.Drawing.Point(550, 570), new System.Drawing.Point(375, 550));
            graphicsPath.AddLine(new System.Drawing.Point(375, 550), new System.Drawing.Point(275, 250));
            graphicsPath.AddLine(new System.Drawing.Point(275, 250), new System.Drawing.Point(375, 350));
            m_polygonGraphics.Add(graphicsPath);

            graphicsPath = new GraphicsPath();
            graphicsPath.AddLine(new System.Drawing.Point(100, 500), new System.Drawing.Point(160, 523));
            graphicsPath.AddLine(new System.Drawing.Point(160, 523), new System.Drawing.Point(170, 620));
            graphicsPath.AddLine(new System.Drawing.Point(170, 620), new System.Drawing.Point(150, 600));
            graphicsPath.AddLine(new System.Drawing.Point(150, 600), new System.Drawing.Point(140, 550));
            graphicsPath.AddLine(new System.Drawing.Point(140, 550), new System.Drawing.Point(100, 500));
            m_polygonGraphics.Add(graphicsPath);
        }

        private void fillPolygons()
        {
            Random random = new Random(DateTime.Now.Millisecond);
            int intRandom = 3 + random.Next() % 4;
            m_background = new Bitmap((int)m_drawingCanvas.ActualWidth, (int)m_drawingCanvas.ActualHeight);

            for (int i = 0; i < intRandom; i++)
            {
                GraphicsPath polygon = m_polygonGraphics[i];

                byte[] pointTypes = new byte[polygon.PointCount];
                for (int j = 0; j < pointTypes.Length; j++)
                    pointTypes[j] = 1;

                Point[] points = Array.ConvertAll(polygon.PathPoints, pointFToPoint);

                try
                {
                    edgesSortFill(m_bitmaps[random.Next() % intRandom], m_background, points, pointTypes, 180);
                }
                catch (Exception)
                {
                    Bitmap colourBitmap = new Bitmap(1, 1);
                    colourBitmap.SetPixel(0, 0, System.Drawing.Color.FromArgb(255, random.Next() % 255
                        , random.Next() % 255, random.Next() % 255));
                    edgesSortFill(colourBitmap, m_background, points, pointTypes, 180);
                }

                m_visiblePolygonGraphics.Add(polygon);
            }
            m_drawingCanvas.Background = createImageBrushFromBitmap(m_background);
        }

        private void edgesSortFill(Bitmap brushBitmap, Bitmap bitmap, Point[] points, byte[] pointTypes, int opacity)
        {
            List<PointInfo> activePointsList = new List<PointInfo>();
            List<PointInfo> pointInfoList;

            createActiveEdgesList(points, out pointInfoList, pointTypes);
            pointInfoList.Sort((p, q) => (p.m_startPoint.Y >= q.m_startPoint.Y) ? 1 : -1);

            int scanLine = (int)pointInfoList[0].m_startPoint.Y;

            do
            {
                while (pointInfoList.Count > 0 && (int)pointInfoList[0].m_currentPoint.Y == scanLine)
                {
                    // Ignore horizontal lines.
                    if (pointInfoList[0].m_startPoint.Y != pointInfoList[0].m_endPoint.Y)
                        activePointsList.Add(pointInfoList[0]);
                    pointInfoList.Remove(pointInfoList[0]);
                }

                activePointsList.Sort((p, q) => (p.m_currentPoint.X >= q.m_currentPoint.X) ? 1 : -1);

                for (int i = 0; i < activePointsList.Count - 1; i += 2)
                {
                    PointInfo currentPointInfo = activePointsList[i];
                    PointInfo nextPointInfo = activePointsList[i + 1];

                    if (currentPointInfo.m_currentPoint.Y == currentPointInfo.m_endPoint.Y)
                    {
                        i -= 2;
                        activePointsList.Remove(currentPointInfo);
                        continue;
                    }

                    if (nextPointInfo.m_currentPoint.Y == nextPointInfo.m_endPoint.Y)
                    {
                        i -= 2;
                        activePointsList.Remove(nextPointInfo);
                        continue;
                    }

                    int startX, endX;
                    startX = (int)currentPointInfo.m_currentPoint.X;
                    endX = (int)nextPointInfo.m_currentPoint.X;

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
                    if ((int)activePointsList[i].m_endPoint.Y == scanLine)
                        pointsToRemove.Add(activePointsList[i]);

                for (int i = 0; i < pointsToRemove.Count; i++)
                    activePointsList.Remove(pointsToRemove[i]);

                scanLine++;

                foreach (PointInfo pointInfo in activePointsList)
                {
                    Point tmpPoint = new Point(pointInfo.m_currentPoint.X + pointInfo.m_delta,
                        pointInfo.m_currentPoint.Y + 1);
                    pointInfo.m_currentPoint = tmpPoint;
                }

            } while (pointInfoList.Count != 0 || activePointsList.Count != 0);
        }

        private void createActiveEdgesList(Point[] points, out List<PointInfo> pointInfoList, byte[] pointTypes)
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
                    pointInfo.m_startPoint = startPoint;
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
                    if (helperCurrentPointList[j].m_startPoint.Y > tmpPoint.Y)
                    {
                        helperCurrentPointList[j].m_endPoint = helperCurrentPointList[j].m_startPoint;
                        helperCurrentPointList[j].m_startPoint = tmpPoint;
                    }
                    else
                        helperCurrentPointList[j].m_endPoint = tmpPoint;

                    double deltaX = helperCurrentPointList[j].m_endPoint.X - helperCurrentPointList[j].m_startPoint.X;
                    double deltaY = helperCurrentPointList[j].m_endPoint.Y - helperCurrentPointList[j].m_startPoint.Y;

                    // Calculate the difference between next x-coordinates.
                    if (deltaX != 0 && deltaY != 0)
                        helperCurrentPointList[j].m_delta = 1 / (deltaY / deltaX);
                    else
                        helperCurrentPointList[j].m_delta = 0;

                    helperCurrentPointList[j].m_currentPoint = helperCurrentPointList[j].m_startPoint;
                    pointInfoList.Add(helperCurrentPointList[j]);
                }
            }
        }

        /// <summary>
        /// Second way to create path from text.
        /// Actually the function is never called. 
        /// I put it here jus to keep in mind that there is another way to convert text to graphics.
        /// </summary>
        private void convertTextToSmootherGraphics()
        {
            FormattedText text = new FormattedText(m_inputTextBox.Text, Thread.CurrentThread.CurrentUICulture,
                FlowDirection.LeftToRight,
                new Typeface(m_inputTextBox.FontFamily, m_inputTextBox.FontStyle, m_inputTextBox.FontWeight,
                    m_inputTextBox.FontStretch),
                double.Parse(m_sizeTextBox.Text), m_colorCanvas.Background);

            Geometry textGeometry = text.BuildGeometry(new Point(0, 0));

            Path path = new Path();
            path.Fill = m_colorCanvas.Background;
            path.Data = textGeometry;
            Canvas.SetZIndex(m_textCanvas, int.MaxValue);
            m_drawingCanvas.Children.Add(path);

            PathGeometry m_pathGeometry = textGeometry.GetOutlinedPathGeometry();
        }

        private void convertTextToGraphics()
        {
            System.Drawing.FontFamily fontFamily = System.Drawing.FontFamily.GenericSerif;
            int fontStyle = (int)System.Drawing.FontStyle.Bold;

            GraphicsPath graphicsPath = new GraphicsPath();
            graphicsPath.AddString(m_inputTextBox.Text, fontFamily, fontStyle, m_textSize
                , new System.Drawing.Point(0, 0), StringFormat.GenericDefault);
            graphicsPath.Flatten();
            PointF[] floatPoints = graphicsPath.PathPoints;
            var pointTypes = graphicsPath.PathTypes;

            RectangleF boundRect = graphicsPath.GetBounds();

            m_textCanvas = new Canvas();
            m_textCanvas.Width = (int)(boundRect.Size.Width + boundRect.X + 1);
            m_textCanvas.Height = (int)(boundRect.Size.Height + boundRect.Y + 1);
            Bitmap bitmap = new Bitmap((int)(boundRect.Size.Width + boundRect.X + 1),
                (int)(boundRect.Size.Height + boundRect.Y + 1));
            Point[] points = Array.ConvertAll(floatPoints, pointFToPoint);

            Random random = new Random(DateTime.Now.Millisecond);

            Bitmap colourBitmap = new Bitmap(1, 1);
            colourBitmap.SetPixel(0, 0, System.Drawing.Color.FromArgb(255, m_color.R
                , m_color.G, m_color.B));
            try
            {
                edgesSortFill(colourBitmap, bitmap, points, pointTypes, 255);
            }
            catch (Exception)
            {
                MessageBox.Show("The text you provided is too long or the font is to big.");
                m_textCanvas = new Canvas();
                return;
            }

            m_textGraphicsPath = graphicsPath;
            m_textCanvas.Background = createImageBrushFromBitmap(bitmap);
            m_drawingCanvas.Children.Add(m_textCanvas);
            m_textCanvas.MouseDown += m_textCanvas_MouseDown;
        }

        public static Point pointFToPoint(PointF pointF)
        {
            return new Point(((int)pointF.X), ((int)pointF.Y));
        }

        public static PointF pointToPointF(Point point)
        {
            return new PointF(((float)point.X), ((float)point.Y));
        }

        public Point doLinesIntersect(PointInfo lineA, PointInfo lineB)
        {
            double deltaA, deltaB;
            if (lineA.m_delta == 0)
                deltaA = 0;
            else
                deltaA = 1 / lineA.m_delta;
            if (lineB.m_delta == 0)
                deltaB = 0;
            else
                deltaB = 1 / lineB.m_delta;

            // Lines are paralel or perpendicular.
            if (deltaA == deltaB)
            {
                if (!((lineA.m_startPoint.Y == lineA.m_endPoint.Y && lineB.m_startPoint.Y == lineB.m_endPoint.Y)
                      || (lineA.m_startPoint.X == lineA.m_endPoint.X && lineB.m_startPoint.X == lineB.m_endPoint.X)))
                {
                    double X, Y;
                    if (lineA.m_startPoint.Y == lineA.m_endPoint.Y)
                    {
                        X = lineB.m_startPoint.X;
                        Y = lineA.m_startPoint.Y;
                    }
                    else
                    {
                        X = lineA.m_startPoint.X;
                        Y = lineB.m_startPoint.Y;
                    }

                    if (Math.Min(lineA.m_startPoint.X, lineA.m_endPoint.X) <= X
                        && X <= Math.Max(lineA.m_startPoint.X, lineA.m_endPoint.X)
                        && Math.Min(lineB.m_startPoint.X, lineB.m_endPoint.X) <= X
                        && X <= Math.Max(lineB.m_startPoint.X, lineB.m_endPoint.X)
                        && Math.Min(lineA.m_startPoint.Y, lineA.m_endPoint.Y) <= Y
                        && Y <= Math.Max(lineA.m_startPoint.Y, lineA.m_endPoint.Y)
                        && Math.Min(lineB.m_startPoint.Y, lineB.m_endPoint.Y) <= Y
                        && Y <= Math.Max(lineB.m_startPoint.Y, lineB.m_endPoint.Y))
                        return new Point(X, Y);
                }
                return new Point(-1, -1);
            }

            double bA = lineA.m_startPoint.Y - deltaA * lineA.m_startPoint.X;
            double bB = lineB.m_startPoint.Y - deltaB * lineB.m_startPoint.X;

            double x = (bB - bA) / (deltaA - deltaB);
            double y = deltaA * x + bA;

            if (Math.Min(lineA.m_startPoint.X, lineA.m_endPoint.X) <= x
                && x <= Math.Max(lineA.m_startPoint.X, lineA.m_endPoint.X)
                && Math.Min(lineB.m_startPoint.X, lineB.m_endPoint.X) <= x
                && x <= Math.Max(lineB.m_startPoint.X, lineB.m_endPoint.X))
                return new Point(x, y);

            return new Point(-1, -1);
        }

        private void weilerAtherton()
        {
            Vector offset = VisualTreeHelper.GetOffset(m_textCanvas);

            foreach (GraphicsPath polygonGraphic in m_visiblePolygonGraphics)
            {
                RectangleF polygonRectangle = polygonGraphic.GetBounds();

                if (offset.X > polygonRectangle.X + polygonRectangle.Width
                    || offset.X + m_textCanvas.Width < polygonRectangle.X
                    || offset.Y > polygonRectangle.Y + polygonRectangle.Height
                    || offset.Y + m_textCanvas.Height < polygonRectangle.Y)
                    continue;

                Point[] polygonPoints = Array.ConvertAll(polygonGraphic.PathPoints, pointFToPoint);
                List<PointInfo> polygonPointList;

                createActiveEdgesList(polygonPoints, out polygonPointList, polygonGraphic.PathTypes);

                Point[] textPoints = Array.ConvertAll(m_textGraphicsPath.PathPoints, pointFToPoint);
                List<PointInfo> textPointList;

                createActiveEdgesList(textPoints, out textPointList, m_textGraphicsPath.PathTypes);

                List<Point> intersectingPoints = new List<Point>();
                LinkedList<PathPoint> currentPolygonPointsList = new LinkedList<PathPoint>();
                LinkedList<PathPoint> currentTextPointsList = new LinkedList<PathPoint>();

                for (int j = 0; j < polygonPoints.Length; j++)
                    currentPolygonPointsList.AddLast(new PathPoint(polygonPoints[j], polygonGraphic.PathTypes[j]));
                for (int j = 0; j < textPoints.Length; j++)
                    currentTextPointsList.AddLast(new PathPoint(textPoints[j], m_textGraphicsPath.PathTypes[j]));

                foreach (var textLine in textPointList)
                {
                    PointInfo pointInfo = new PointInfo();
                    pointInfo.m_startPoint = new Point(offset.X + textLine.m_startPoint.X, offset.Y + textLine.m_startPoint.Y);
                    pointInfo.m_endPoint = new Point(offset.X + textLine.m_endPoint.X, offset.Y + textLine.m_endPoint.Y);
                    pointInfo.m_delta = textLine.m_delta;

                    foreach (var polygonLine in polygonPointList)
                    {
                        Point foundPoint = doLinesIntersect(pointInfo, polygonLine);
                        if (foundPoint.X != -1)
                        {
                            intersectingPoints.Add(foundPoint);
                            LinkedListNode<PathPoint> node = null;

                            foreach (PathPoint pathPoint in currentPolygonPointsList)
                                if (pathPoint.m_point == polygonLine.m_endPoint)
                                {
                                    node = currentPolygonPointsList.Find(pathPoint);
                                    break;
                                }

                            currentPolygonPointsList.AddAfter(node, new PathPoint(foundPoint, 1));
                            node = null;

                            foreach (PathPoint textPoint in currentTextPointsList)
                                if (textPoint.m_point == textLine.m_endPoint)
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
                    polygonPoints[j] = polygonPointsArray[j].m_point;
                    polygonBytes[j] = polygonPointsArray[j].m_type;
                }
                for (int j = 0; j < currentTextPointsList.Count; j++)
                {
                    textPoints[j] = textPointsArray[j].m_point;
                    textBytes[j] = textPointsArray[j].m_type;
                }

                polygonPointList = new List<PointInfo>();
                textPointList = new List<PointInfo>();

                createActiveEdgesList(polygonPoints, out polygonPointList, polygonBytes);
                createActiveEdgesList(textPoints, out textPointList, textBytes);

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
                            int currentPointIndex = (polygonPointList.FindIndex(p => p.m_startPoint.Equals(lastPoint)) + 1);
                            Point currentPolygonPoint = polygonPointList[currentPointIndex].m_startPoint;
                            do
                            {
                                currentPointIndex = (polygonPointList.FindIndex(p => p.m_startPoint.Equals(lastPoint)) + 1);
                                currentPolygonPoint = polygonPointList[currentPointIndex].m_startPoint;
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
                                p => p.m_startPoint.Equals(new Point(lastPoint.X - offset.X, lastPoint.Y - offset.Y))) + 1) % textPointList.Count;
                            Point currentTextPoint = textPointList[currentTextIndex].m_startPoint;
                            do
                            {
                                currentTextIndex = (textPointList.FindIndex(
                                p => p.m_startPoint.Equals(new Point(lastPoint.X - offset.X, lastPoint.Y - offset.Y))) + 1) % textPointList.Count;
                                currentTextPoint = new Point(textPointList[currentTextIndex].m_startPoint.X + offset.X
                                    , textPointList[currentTextIndex].m_startPoint.Y + offset.Y);
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
