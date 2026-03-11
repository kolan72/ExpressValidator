using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace ExpressValidator
{
	internal static class TypeHelper<T>
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsNull(T value)
		{
			return TypeTraits<T>.CanBeNull && EqualityComparer<T>.Default.Equals(value, default);
		}
	}
}
