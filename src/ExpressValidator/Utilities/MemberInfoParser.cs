using System;
using System.Linq.Expressions;
using System.Reflection;

namespace ExpressValidator
{
	internal static class MemberInfoParser
	{
		public static bool TryParse<T, TProperty>(Expression<Func<T, TProperty>> getExpression, MemberTypes memberTypes, out MemberInfo result)
		{
			result = (getExpression.Body as MemberExpression)?.Member;
			if (result is null)
				return false;

			return result.MemberType == memberTypes;
		}

		public static MemberInfo ParseProperty<T, TProperty>(Expression<Func<T, TProperty>> getExpression)
		{
			if (!TryParse(getExpression, MemberTypes.Property, out MemberInfo memInfo))
			{
				throw new ArgumentException("Can not get property from expression.");
			}
			return memInfo;
		}

		public static MemberInfo ParseField<T, TProperty>(Expression<Func<T, TProperty>> getExpression)
		{
			if (!TryParse(getExpression, MemberTypes.Field, out MemberInfo memInfo))
			{
				throw new ArgumentException("Can not get field from expression.");
			}
			return memInfo;
		}
	}
}
