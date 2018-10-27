using System.Linq;
using FillingDemo.Shapes;
using NUnit.Framework;

namespace FillingTests.Shapes
{
	[TestFixture]
	public class TextTests
	{
		private Text _unitUnderTest;

		[SetUp]
		public void Initialize()
		{
			_unitUnderTest = new Text("ʕ•ᴥ•ʔ", 1);
		}

		[Test]
		[TestCase(-100, 0)]
		[TestCase(10, 10)]
		public void Move_X_XSetToExpectedValue(double x, double expected)
		{
			_unitUnderTest.Move(x, 0, 100, 100);

			Assert.AreEqual(expected, _unitUnderTest.X);
		}

		[Test]
		[TestCase(-100, 0)]
		[TestCase(10, 10)]
		public void Move_Y_YSetToExpectedValue(double y, double expected)
		{
			_unitUnderTest.Move(0, y, 100, 100);

			Assert.AreEqual(expected, _unitUnderTest.Y);
		}

		[Test]
		public void Move_XY_EdgesMoved()
		{
			var edges = _unitUnderTest.ActiveEdges.Select(e => e.StartPoint).ToArray();

			_unitUnderTest.Move(5, 7, 100, 100);
			var movedEdges = _unitUnderTest.ActiveEdges.Select(e => e.StartPoint).ToArray();

			for (int i = 0; i < edges.Length; i++)
			{
				Assert.AreEqual(edges[i].X + 5, movedEdges[i].X);
				Assert.AreEqual(edges[i].Y + 7, movedEdges[i].Y);
			}
		}
	}
}