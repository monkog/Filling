using System;

namespace FillingDemo.Helpers
{
	public static class DoubleExtensions
	{
		private const double Epsilon = 0.00000000001;

		/// <summary>
		/// Determines whether the given value can be treated as zero.
		/// </summary>
		/// <param name="value">Value to check.</param>
		/// <returns>True if the provided value is close to zero, false otherwise.</returns>
		public static bool IsZero(this double value)
		{
			return Math.Abs(value) < Epsilon;
		}
	}
}