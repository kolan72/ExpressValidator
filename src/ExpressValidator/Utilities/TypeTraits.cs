using System;

namespace ExpressValidator
{
	internal static class TypeTraits<T>
	{
		public static readonly bool CanBeNull = !typeof(T).IsValueType || Nullable.GetUnderlyingType(typeof(T)) != null;
	}
}