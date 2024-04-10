using System.Reflection;

namespace ExpressValidator
{
	internal static class ReflectionExtensions
	{
		public static T GetTypedValue<TObj, T>(this PropertyInfo propertyInfo, TObj obj)
		{
			return (T)propertyInfo.GetValue(obj);
		}

		public static T GetTypedValue<TObj, T>(this MemberInfo memberInfo, TObj obj)
		{
			return memberInfo.MemberType == MemberTypes.Property ? (T)((PropertyInfo)memberInfo).GetValue(obj)
																 : (T)((FieldInfo)memberInfo).GetValue(obj);
		}
	}
}
