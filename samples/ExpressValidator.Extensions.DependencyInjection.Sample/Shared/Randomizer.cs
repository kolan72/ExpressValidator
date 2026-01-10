using System;

namespace Shared
{
	public static class Randomizer
	{
		private static readonly Random _rnd = new();
		public static int Next(int minInclusive, int maxExclusive)
		{
			return _rnd.Next(minInclusive, maxExclusive);
		}
	}
}
