using System;
using System.Linq.Expressions;
using System.Reflection;

namespace ExpressValidator
{
	internal static class MemberInfoParser
	{
		public static bool TryParse<T, TProperty>(Expression<Func<T, TProperty>> getExpression, out PropertyInfo result)
		{
			result = (getExpression.Body as MemberExpression)?.Member as PropertyInfo;
			return !(result is null);
		}

		public static bool TryParse<T, TProperty>(Expression<Func<T, TProperty>> getExpression, MemberTypes memberTypes, out MemberInfo result)
		{
			result = (getExpression.Body as MemberExpression)?.Member;
			if (result is null)
				return false;

			return result.MemberType == memberTypes;
		}
	}
}
