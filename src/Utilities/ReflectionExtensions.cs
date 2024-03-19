using System.Reflection;

namespace ExpressValidator
{
	internal static class ReflectionExtensions
	{
		public static T GetTypedValue<TObj, T>(this PropertyInfo propertyInfo, TObj obj)
		{
			return (T)propertyInfo.GetValue(obj);
		}
	}
}
