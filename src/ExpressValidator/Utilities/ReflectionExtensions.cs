using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;

namespace ExpressValidator
{
	internal static class ReflectionExtensions
	{
		// Keyed by (MemberInfo, TObj, T) via the closed generic — one compiled delegate per
		// unique (member, object type, value type) combination, shared across all instances.
		private static class AccessorCache<TObj, T>
		{
			// ConcurrentDictionary is safe for concurrent reads/writes without external locking.
			internal static readonly ConcurrentDictionary<MemberInfo, Func<TObj, T>> Cache = new ConcurrentDictionary<MemberInfo, Func<TObj, T>>();
		}

		public static T GetTypedValue<TObj, T>(this MemberInfo memberInfo, TObj obj)
		{
			var accessor = AccessorCache<TObj, T>.Cache.GetOrAdd(memberInfo, mi =>
			{
				// Build and compile a strongly-typed lambda: (TObj obj) => obj.Member
				// Compiled delegates are ~10-50x faster than MethodInfo.Invoke on repeated calls
				// because they avoid argument array allocation and value-type boxing.
				var param = Expression.Parameter(typeof(TObj), "obj");
				Expression body = mi.MemberType == MemberTypes.Property
					? Expression.Property(param, (PropertyInfo)mi)
					: Expression.Field(param, (FieldInfo)mi);
				return Expression.Lambda<Func<TObj, T>>(body, param).Compile();
			});

			return accessor(obj);
		}
	}
}
