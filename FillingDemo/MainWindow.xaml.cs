using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using FillingDemo.Helpers;
using FillingDemo.Shapes;
using Point = System.Windows.Point;

namespace FillingDemo
{
	public partial class MainWindow : INotifyPropertyChanged
	{
		private Canvas _textCanvas;

		private int _textSize;

		private string _text;

		private Text _textGraphic;

		private System.Drawing.Color _color;

		private Point _lastMousePosition;

		private bool _isMouseDown;

		private IList<Polygon> _polygons;

		private IList<Canvas> _intersections;

		public int TextSize
		{
			get { return _textSize; }
			set
			{
				if (_textSize == value) return;
				_textSize = value;
				OnPropertyChanged(nameof(TextSize));
			}
		}

		public string Text
		{
			get { return _text; }
			set
			{
				if (_text == value) return;
				_text = value;
				OnPropertyChanged(nameof(Text));
			}
		}

		public MainWindow()
		{
			InitializeComponent();
			DataContext = this;
		}

		#region events
		private void Window_Loaded_1(object sender, RoutedEventArgs e)
		{
			TextSize = 200;
			_isMouseDown = false;
			_color = System.Drawing.Color.MidnightBlue;
			MouseUp += MainWindow_MouseUp;
			MouseMove += MainWindow_MouseMove;
			var background = new Background((int)DrawingCanvas.ActualWidth, (int)DrawingCanvas.ActualHeight);
			_polygons = background.Polygons;
			DrawingCanvas.Background = background.Draw();
			_intersections = new Canvas[0];
		}

		private void TextCanvas_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			_lastMousePosition = e.GetPosition(DrawingCanvas);
			_isMouseDown = true;
		}

		private void MainWindow_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
		{
			if (!_isMouseDown) return;

			var mouseMovementX = e.GetPosition(DrawingCanvas).X - _lastMousePosition.X;
			var mouseMovementY = e.GetPosition(DrawingCanvas).Y - _lastMousePosition.Y;
			var offset = VisualTreeHelper.GetOffset(_textCanvas);
			var oldX = _textGraphic.X;
			var oldY = _textGraphic.Y;

			if (offset.Y + mouseMovementY >= -0.2 * _textCanvas.ActualHeight)
				if (offset.Y + mouseMovementY <= DrawingCanvas.ActualHeight - _textCanvas.ActualHeight)
				{
					Canvas.SetTop(_textCanvas, offset.Y + mouseMovementY);
					_textGraphic.Y = offset.Y + mouseMovementY;
				}
				else
				{
					Canvas.SetTop(_textCanvas, DrawingCanvas.ActualHeight - _textCanvas.ActualHeight);
					_textGraphic.Y = DrawingCanvas.ActualHeight - _textCanvas.ActualHeight;
				}
			else
			{
				Canvas.SetTop(_textCanvas, -0.2 * _textCanvas.ActualHeight);
				_textGraphic.Y = -0.2 * _textCanvas.ActualHeight;
			}

			if (offset.X + mouseMovementX < DrawingCanvas.ActualWidth - _textCanvas.ActualWidth)
				if (offset.X + mouseMovementX >= 0)
				{
					Canvas.SetLeft(_textCanvas, offset.X + mouseMovementX);
					_textGraphic.X = offset.X + mouseMovementX;
				}
				else
				{
					Canvas.SetLeft(_textCanvas, 0);
					_textGraphic.X = 0;
				}
			else
			{
				Canvas.SetLeft(_textCanvas, DrawingCanvas.ActualWidth - _textCanvas.ActualWidth);
				_textGraphic.X = DrawingCanvas.ActualWidth - _textCanvas.ActualWidth;
			}

			foreach (var edge in _textGraphic.ActiveEdges)
			{
				edge.Move(_textGraphic.X - oldX, _textGraphic.Y - oldY);
			}

			_lastMousePosition = e.GetPosition(DrawingCanvas);
			foreach (var intersection in _intersections) DrawingCanvas.Children.Remove(intersection);
			_intersections = _textGraphic.FindIntersections(_polygons).ToList();
			foreach (var intersection in _intersections) DrawingCanvas.Children.Add(intersection);
			//WeilerAtherton();
		}

		private void MainWindow_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			_isMouseDown = false;
		}

		private void ColorCanvas_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			var colorDialog = new ColorDialog();
			var dialogResult = colorDialog.ShowDialog();

			if (dialogResult != System.Windows.Forms.DialogResult.OK) return;

			ColorCanvas.Background = new SolidColorBrush(Color.FromArgb(colorDialog.Color.A, colorDialog.Color.R, colorDialog.Color.G, colorDialog.Color.B));
			_color = colorDialog.Color;
			DrawingCanvas.Children.Remove(_textCanvas);
			ConvertTextToGraphics();
		}

		private void ConvertTextToGraphics()
		{
			_textGraphic = new Text(Text, TextSize);

			try
			{
				var resultBitmap = _textGraphic.Draw(_color);
				_textCanvas = new Canvas
				{
					Width = resultBitmap.Width,
					Height = resultBitmap.Height,
					Background = resultBitmap.CreateImageBrush()
				};
			}
			catch (Exception)
			{
				System.Windows.MessageBox.Show("The text you provided is too long or the font is to big.");
				_textCanvas = new Canvas();
				return;
			}

			DrawingCanvas.Children.Add(_textCanvas);
			_textCanvas.MouseDown += TextCanvas_MouseDown;

			foreach (var intersection in _intersections) DrawingCanvas.Children.Remove(intersection);
			_intersections = _textGraphic.FindIntersections(_polygons).ToList();
			foreach (var intersection in _intersections) DrawingCanvas.Children.Add(intersection);
		}

		private void SetTextButton_Click(object sender, RoutedEventArgs e)
		{
			if (_textCanvas != null)
				DrawingCanvas.Children.Remove(_textCanvas);

			ConvertTextToGraphics();
		}
		#endregion

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged(string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
