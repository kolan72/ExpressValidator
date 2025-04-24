using ExpressValidator.Extensions;
using FluentValidation;
using FluentValidation.Results;
using System;

namespace ExpressValidator
{
	/// <summary>
	/// Provides methods for quick validation.
	/// </summary>
	public static class QuickValidator
	{
		private const string FALLBACK_PROP_NAME = "input value";

		/// <summary>
		///  Validates the given object instance using <paramref name="action"/>.
		/// </summary>
		/// <typeparam name="T">The type of the object to validate.</typeparam>
		/// <param name="obj">The object to validate.</param>
		/// <param name="action">Action to add validators.</param>
		/// <param name="propName">The name of the property if the validation fails.
		/// If <see langword="null"/>, "input value" will be used.</param>
		/// <returns></returns>
		public static ValidationResult Validate<T>(T obj, Action<IRuleBuilderOptions<T, T>> action, string propName = null)
		{
			var eb = new ExpressValidatorBuilder<Unit>();
			return eb.AddFunc((_) => obj,
								GetPropName(propName))
					.WithValidation(action)
					.BuildAndValidate(Unit.Default);
		}

		private static string GetPropName(string propName = null) => propName ?? FALLBACK_PROP_NAME;
	}
}
