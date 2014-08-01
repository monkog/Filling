using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Shapes;
using Color = System.Windows.Media.Color;
using Point = System.Windows.Point;
using Size = System.Drawing.Size;

namespace FillingDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region globals
        private List<GraphicsPath> m_polygonGraphics;
        private List<GraphicsPath> m_visiblePolygonGraphics;
        private List<Bitmap> m_bitmaps;
        private Canvas m_textCanvas;
        private GraphicsPath m_textGraphicsPath;
        private GraphicsPath m_currentTextGraphicsPath;
        private Point m_lastMousePosition;
        private bool m_isMouseDown;
        private Bitmap m_background;
        private int m_textSize;
        private System.Drawing.Color m_color;
        #endregion

        public MainWindow()
        {
            InitializeComponent();
        }

        #region events
        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            m_textSize = 200;
            m_isMouseDown = false;
            m_color = System.Drawing.Color.MidnightBlue;
            MouseUp += MainWindow_MouseUp;
            MouseMove += MainWindow_MouseMove;
            KeyDown += MainWindow_KeyDown;

            // Initialize bitmaps.
            m_bitmaps = new List<Bitmap>();
            m_bitmaps.Add((Properties.Resources.m_stars));
            m_bitmaps.Add((Properties.Resources.m_darkClouds));
            m_bitmaps.Add((Properties.Resources.m_stone));

            initializePolygons();
            fillPolygons();
        }

        void m_textCanvas_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            m_lastMousePosition = e.GetPosition(m_drawingCanvas);
            m_isMouseDown = true;
        }

        void MainWindow_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (m_isMouseDown)
            {
                double deltaX = e.GetPosition(m_drawingCanvas).X - m_lastMousePosition.X;
                double deltaY = e.GetPosition(m_drawingCanvas).Y - m_lastMousePosition.Y;
                Vector offset = VisualTreeHelper.GetOffset(m_textCanvas);

                if (offset.Y + deltaY >= -0.2*m_textCanvas.ActualHeight)
                    if (offset.Y + deltaY <= m_drawingCanvas.ActualHeight - m_textCanvas.ActualHeight)
                        Canvas.SetTop(m_textCanvas, offset.Y + deltaY);
                    else
                        Canvas.SetTop(m_textCanvas, m_drawingCanvas.ActualHeight - m_textCanvas.ActualHeight);
                else
                    Canvas.SetTop(m_textCanvas, -0.2*m_textCanvas.ActualHeight);

                if (offset.X + deltaX < m_drawingCanvas.ActualWidth - m_textCanvas.ActualWidth)
                    if (offset.X + deltaX >= 0)
                        Canvas.SetLeft(m_textCanvas, offset.X + deltaX);
                    else
                        Canvas.SetLeft(m_textCanvas, 0);
                else
                    Canvas.SetLeft(m_textCanvas, m_drawingCanvas.ActualWidth - m_textCanvas.ActualWidth);

                m_lastMousePosition = e.GetPosition(m_drawingCanvas);
                // weilerAtherton();
            }
        }

        void MainWindow_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            m_isMouseDown = false;
        }

        private void m_colorCanvas_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();
            DialogResult dialogResult = colorDialog.ShowDialog();

            if (dialogResult == System.Windows.Forms.DialogResult.OK)
            {
                m_colorCanvas.Background = new SolidColorBrush(Color.FromArgb(colorDialog.Color.A, colorDialog.Color.R, colorDialog.Color.G, colorDialog.Color.B));
                m_color = colorDialog.Color;
                m_drawingCanvas.Children.Remove(m_textCanvas);
                convertTextToGraphics();
            }
        }

        private void m_setTextButton_Click(object sender, RoutedEventArgs e)
        {
            if (m_textCanvas != null)
                m_drawingCanvas.Children.Remove(m_textCanvas);

            if (m_inputTextBox.Text == "")
                return;

            m_sizeTextBox.RaiseEvent(new RoutedEventArgs(LostFocusEvent));
            m_textSize = int.Parse(m_sizeTextBox.Text);

            convertTextToGraphics();
        }

        private void MainWindow_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
                m_setTextButton.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Button.ClickEvent));
        }

        private void m_sizeTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                int textSize;
                int.TryParse(m_sizeTextBox.Text, out textSize);

                if (textSize <= 0)
                    throw new Exception();
            }
            catch (Exception)
            {
                m_sizeTextBox.Text = m_textSize.ToString();
                System.Windows.MessageBox.Show("You have to type a positive number.");
            }
        }
        #endregion
    }
}
