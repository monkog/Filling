using System.Windows;
using FillingDemo.Shapes;
using NUnit.Framework;

namespace FillingTests.Shapes
{
	[TestFixture]
	public class ActiveEdgeTests
	{
		[Test]
		public void Constructor_StartAndEndPoints_CurrentPointSetToStart()
		{
			var start = new Point(0, 0);
			var end = new Point(10, 0);

			var unitUnderTest = new ActiveEdge(start, end);

			Assert.AreEqual(start, unitUnderTest.StartPoint);
			Assert.AreEqual(start, unitUnderTest.CurrentPoint);
			Assert.AreEqual(end, unitUnderTest.EndPoint);
		}

		[Test]
		public void Constructor_StartYGreaterThanEndY_StartAndEndSwitched()
		{
			var start = new Point(0, 10);
			var end = new Point(10, 0);

			var unitUnderTest = new ActiveEdge(start, end);

			Assert.AreEqual(end, unitUnderTest.StartPoint);
			Assert.AreEqual(end, unitUnderTest.CurrentPoint);
			Assert.AreEqual(start, unitUnderTest.EndPoint);
		}

		[Test]
		public void Constructor_SameXCoordinates_DeltaZero()
		{
			var start = new Point(10, 10);
			var end = new Point(10, 0);

			var unitUnderTest = new ActiveEdge(start, end);

			Assert.AreEqual(0, unitUnderTest.Delta);
		}

		[Test]
		public void Constructor_SameYCoordinates_DeltaZero()
		{
			var start = new Point(0, 10);
			var end = new Point(10, 10);

			var unitUnderTest = new ActiveEdge(start, end);

			Assert.AreEqual(0, unitUnderTest.Delta);
		}

		[Test]
		[TestCase(10, 10, 0, 0, 1)]
		[TestCase(0, 0, 1, 2, 0.5)]
		[TestCase(0, 10, 10, 0, -1)]
		public void Constructor_BothCoordinatesDifferent_DeltaCalculated(double x1, double y1, double x2, double y2, double result)
		{
			var start = new Point(x1, y1);
			var end = new Point(x2, y2);

			var unitUnderTest = new ActiveEdge(start, end);

			Assert.AreEqual(result, unitUnderTest.Delta);
		}

		[Test]
		[TestCase(10, 10, 0, 0, 1, 0)]
		[TestCase(10, 12, 0, 2, 1, 2)]
		[TestCase(10, 10, 0, 10, 0, 10)]
		[TestCase(10, 10, 10, 0, 0, 0)]
		public void Constructor_BothCoordinatesDifferent_CoefficientsCalculated(double x1, double y1, double x2, double y2, double a, double b)
		{
			var start = new Point(x1, y1);
			var end = new Point(x2, y2);

			var unitUnderTest = new ActiveEdge(start, end);

			Assert.AreEqual(a, unitUnderTest.A);
			Assert.AreEqual(b, unitUnderTest.B);
		}

		[Test]
		public void IsHorizontal_SameY_True()
		{
			var start = new Point(5, 5);
			var end = new Point(15, 5);

			var unitUnderTest = new ActiveEdge(start, end);
			
			Assert.IsTrue(unitUnderTest.IsHorizontal);
		}

		[Test]
		public void IsHorizontal_DifferentY_False()
		{
			var start = new Point(5, 5);
			var end = new Point(5, 10);
			
			var unitUnderTest = new ActiveEdge(start, end);
			
			Assert.False(unitUnderTest.IsHorizontal);
		}

		[Test]
		public void IsVertical_SameX_True()
		{
			var start = new Point(5, 5);
			var end = new Point(5, 15);

			var unitUnderTest = new ActiveEdge(start, end);
			
			Assert.IsTrue(unitUnderTest.IsVertical);
		}

		[Test]
		public void IsVertical_DifferentX_False()
		{
			var start = new Point(5, 5);
			var end = new Point(15, 5);
			
			var unitUnderTest = new ActiveEdge(start, end);
			
			Assert.False(unitUnderTest.IsVertical);
		}

		[Test]
		[TestCase(0, 0)]
		[TestCase(5, 5)]
		[TestCase(10, 10)]
		public void Contains_PointOnEdge_True(double x, double y)
		{
			var start = new Point(0, 0);
			var end = new Point(10, 10);

			var unitUnderTest = new ActiveEdge(start, end);
			var result = unitUnderTest.Contains(new Point(x, y));

			Assert.IsTrue(result);
		}

		[Test]
		[TestCase(0, 0)]
		[TestCase(0, 5)]
		[TestCase(0, 10)]
		public void Contains_PointOnVerticalEdge_True(double x, double y)
		{
			var start = new Point(0, 0);
			var end = new Point(0, 10);

			var unitUnderTest = new ActiveEdge(start, end);
			var result = unitUnderTest.Contains(new Point(x, y));

			Assert.IsTrue(result);
		}

		[Test]
		[TestCase(0, 0)]
		[TestCase(5, 0)]
		[TestCase(10, 0)]
		public void Contains_PointOnHorizontalEdge_True(double x, double y)
		{
			var start = new Point(0, 0);
			var end = new Point(10, 0);

			var unitUnderTest = new ActiveEdge(start, end);
			var result = unitUnderTest.Contains(new Point(x, y));

			Assert.IsTrue(result);
		}

		[Test]
		[TestCase(-5, -5)]
		[TestCase(2, 5)]
		public void Contains_PointOutsideEdge_False(double x, double y)
		{
			var start = new Point(0, 0);
			var end = new Point(10, 10);

			var unitUnderTest = new ActiveEdge(start, end);
			var result = unitUnderTest.Contains(new Point(x, y));

			Assert.IsFalse(result);
		}

		[Test]
		public void FindIntersection_Intersecting_PointOfIntersection()
		{
			var line1 = new ActiveEdge(new Point(0, 5), new Point(10, 5));
			var line2 = new ActiveEdge(new Point(5, 0), new Point(5, 10));

			var result = line1.FindIntersection(line2);

			Assert.IsNotNull(result);
			Assert.AreEqual(5, result.Value.X);
			Assert.AreEqual(5, result.Value.Y);
		}

		[Test]
		public void FindIntersection_ParallelHorizontal_Null()
		{
			var line1 = new ActiveEdge(new Point(0, 0), new Point(10, 0));
			var line2 = new ActiveEdge(new Point(0, 5), new Point(0, 5));

			var result = line1.FindIntersection(line2);

			Assert.IsNull(result);
		}

		[Test]
		public void FindIntersection_ParallelVertical_Null()
		{
			var line1 = new ActiveEdge(new Point(0, 0), new Point(0, 10));
			var line2 = new ActiveEdge(new Point(5, 0), new Point(5, 10));

			var result = line1.FindIntersection(line2);

			Assert.IsNull(result);
		}

		[Test]
		public void FindIntersection_Parallel_Null()
		{
			var line1 = new ActiveEdge(new Point(0, 0), new Point(5, 5));
			var line2 = new ActiveEdge(new Point(0, 2), new Point(5, 7));

			var result = line1.FindIntersection(line2);

			Assert.IsNull(result);
		}

		[Test]
		public void FindIntersection_NotIntersecting_Null()
		{
			var line1 = new ActiveEdge(new Point(0, 5), new Point(10, 5));
			var line2 = new ActiveEdge(new Point(15, 0), new Point(15, 10));

			var result = line1.FindIntersection(line2);

			Assert.IsNull(result);
		}

		[Test]
		public void FindIntersection_SameLine_Null()
		{
			var line1 = new ActiveEdge(new Point(0, 5), new Point(10, 5));
			var line2 = new ActiveEdge(new Point(0, 5), new Point(10, 5));

			var result = line1.FindIntersection(line2);

			Assert.IsNull(result);
		}
	}
}