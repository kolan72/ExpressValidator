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

		public static bool TryParse<T, TProperty>(Expression<Func<T, TProperty>> getExpression, out MemberInfo result)
		{
			result = (getExpression.Body as MemberExpression)?.Member;
			if (result is null)
				return false;

			return result.MemberType == MemberTypes.Property || result.MemberType == MemberTypes.Field;
		}

		internal static bool TryParseMethodCallExpression<T, TProperty>(Expression<Func<T, TProperty>> expression, out ParameterInfo[] parameters)
		{
			parameters = null;
			if (expression == null) return false;

			// 1. Unwrap any implicit casts (e.g., ExpressionType.Convert)
			Expression body = expression.Body;
			while (body is UnaryExpression unary)
			{
				body = unary.Operand;
			}

			PropertyInfo propertyInfo = null;

			if (body is MethodCallExpression methodCallExpr)
			{
				var method = methodCallExpr.Method;

				// Indexers are compiled as special "get_" methods
				if (method.IsSpecialName && method.Name.StartsWith("get_"))
				{
					propertyInfo = Array
#pragma warning restore S3011 // Reflection should not be used to increase accessibility of classes, methods, or fields
						.Find(method.DeclaringType
#pragma warning disable S3011 // Reflection should not be used to increase accessibility of classes, methods, or fields
						.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static), p => p.GetGetMethod(true) == method);
				}
			}

			// 4. Check if we found a property and if it actually has index parameters
			if (propertyInfo != null)
			{
				var indexParams = propertyInfo.GetIndexParameters();

				// If Length > 0, it is genuinely an indexer property
				if (indexParams.Length > 0)
				{
					parameters = indexParams;
					return true;
				}
			}
			return false;
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
