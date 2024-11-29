using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Benchmark
{
	public static class PropertyInfoToFuncConverter
	{
		public static Func<TObj, T> GetPropertyFuncByExpression<TObj, T>(MemberInfo memberInfo)
		{
			return BuildGetAccessor<TObj, T>(((PropertyInfo)memberInfo).GetGetMethod());
		}

		public static Func<TObj, T> GetPropertyFuncByReflection<TObj, T>(MemberInfo memberInfo)
		{
			return (obj) => (T)((PropertyInfo)memberInfo).GetValue(obj);
		}

		public static Func<TObj, T> GetPropertyFuncByMethodInfo<TObj, T>(MemberInfo memberInfo)
		{
			var m = ((PropertyInfo)memberInfo).GetGetMethod();
			return (obj) => (T)m.Invoke(obj, null);
		}

		internal static Func<TObj, T> BuildGetAccessor<TObj, T>(MethodInfo method)
		{
			ParameterExpression instance = Expression.Parameter(typeof(TObj), "instance");

			var body = Expression.Call(instance, method);

			return Expression.Lambda<Func<TObj, T>>(body, instance).Compile();
		}
	}
}
