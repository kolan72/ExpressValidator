using System;
using System.Linq.Expressions;
using System.Reflection;

namespace ExpressValidator
{
	internal static class PropertyInfoParser
	{
		public static bool TryParse<T, TProperty>(Expression<Func<T, TProperty>> getExpression, out PropertyInfo result)
		{
			result = (getExpression.Body as MemberExpression)?.Member as PropertyInfo;
			return !(result is null);
		}
	}
}
