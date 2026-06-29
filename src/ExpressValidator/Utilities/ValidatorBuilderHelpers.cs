using System.Linq;
using System.Reflection;

namespace ExpressValidator
{
	internal static class ValidatorBuilderHelpers
	{
		public static string GetIndexerPropName(ParameterInfo[] parameters)
			=> $"this[{(parameters.FirstOrDefault()?.Name ?? "index")}]";
	}
}
