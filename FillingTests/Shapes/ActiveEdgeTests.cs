using System.Windows;
using FillingDemo.Shapes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FillingTests.Shapes
{
	[TestClass]
	public class ActiveEdgeTests
	{
		[TestMethod]
		public void Constructor_StartAndEndPoints_CurrentPointSetToStart()
		{
			var start = new Point(0, 0);
			var end = new Point(10, 0);

			var unitUnderTest = new ActiveEdge(start, end);

			Assert.AreEqual(start, unitUnderTest.StartPoint);
			Assert.AreEqual(start, unitUnderTest.CurrentPoint);
			Assert.AreEqual(end, unitUnderTest.EndPoint);
		}

		[TestMethod]
		public void Constructor_StartYGreaterThanEndY_StartAndEndSwitched()
		{
			var start = new Point(0, 10);
			var end = new Point(10, 0);

			var unitUnderTest = new ActiveEdge(start, end);

			Assert.AreEqual(end, unitUnderTest.StartPoint);
			Assert.AreEqual(end, unitUnderTest.CurrentPoint);
			Assert.AreEqual(start, unitUnderTest.EndPoint);
		}

		[TestMethod]
		public void Constructor_SameXCoordinates_DeltaZero()
		{
			var start = new Point(10, 10);
			var end = new Point(10, 0);

			var unitUnderTest = new ActiveEdge(start, end);

			Assert.AreEqual(0, unitUnderTest.Delta);
		}

		[TestMethod]
		public void Constructor_SameYCoordinates_DeltaZero()
		{
			var start = new Point(0, 10);
			var end = new Point(10, 10);

			var unitUnderTest = new ActiveEdge(start, end);

			Assert.AreEqual(0, unitUnderTest.Delta);
		}

		[TestMethod]
		[DataRow(10, 10, 0, 0, 1)]
		[DataRow(0, 0, 1, 2, 0.5)]
		[DataRow(0, 10, 10, 0, -1)]
		public void Constructor_BothCoordinatesDifferent_DeltaCalculated(double x1, double y1, double x2, double y2, double result)
		{
			var start = new Point(x1, y1);
			var end = new Point(x2, y2);

			var unitUnderTest = new ActiveEdge(start, end);

			Assert.AreEqual(result, unitUnderTest.Delta);
		}
	}
}