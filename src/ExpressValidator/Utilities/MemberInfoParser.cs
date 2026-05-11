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

		/// <summary>
		/// Parses a property or field from the expression.
		/// Throws <see cref="ArgumentException"/> if the expression does not resolve to a property or field.
		/// </summary>
		public static MemberInfo ParseMember<T, TProperty>(Expression<Func<T, TProperty>> getExpression)
		{
			var member = (getExpression.Body as MemberExpression)?.Member;
			if (member is null || (member.MemberType != MemberTypes.Property && member.MemberType != MemberTypes.Field))
			{
				throw new ArgumentException("Expression must refer to a property or field.");
			}
			return member;
		}
	}
}
