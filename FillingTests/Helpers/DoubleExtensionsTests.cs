using FillingDemo.Helpers;
using NUnit.Framework;

namespace FillingTests.Helpers
{
	[TestFixture]
	public class DoubleExtensionsTests
	{
		[Test]
		[TestCase(1)]
		[TestCase(-1)]
		[TestCase(-0.0000000001)]
		[TestCase(0.0000000001)]
		public void IsZero_NotZero_False(double value)
		{
			var result = value.IsZero();

			Assert.IsFalse(result);
		}

		[Test]
		[TestCase(0)]
		[TestCase(-0.000000000009)]
		[TestCase(0.000000000009)]
		public void IsZero_CloseToZero_True(double value)
		{
			var result = value.IsZero();

			Assert.True(result);
		}
	}
}