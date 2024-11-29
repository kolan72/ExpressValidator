using System.Reflection;

namespace ExpressValidator
{
	internal static class ReflectionExtensions
	{
		public static T GetTypedValue<TObj, T>(this MemberInfo memberInfo, TObj obj)
		{
			return memberInfo.MemberType == MemberTypes.Property ? (T)((PropertyInfo)memberInfo).GetGetMethod().Invoke(obj, null)
																 : (T)((FieldInfo)memberInfo).GetValue(obj);
		}
	}
}
