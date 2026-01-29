using System;
using System.Collections.Generic;

namespace ExpressValidator
{
	internal static class TypeInfo<T>
	{
		private static readonly bool canBeNull = !typeof(T).IsValueType || (Nullable.GetUnderlyingType(typeof(T)) != null);

		public static bool IsValueNull(T value)
		{
			return canBeNull && EqualityComparer<T>.Default.Equals(value, default);
		}
	}
}