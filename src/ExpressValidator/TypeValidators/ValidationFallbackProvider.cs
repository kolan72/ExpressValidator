using FluentValidation;
using FluentValidation.Results;

namespace ExpressValidator
{
	internal static class ValidationFallbackProvider
	{
		/// <summary>
		/// Generates a ValidationResult representing a failure due to a null instance.
		/// </summary>
		public static ValidationResult GetNullFailure<T>()
		{
			var typeName = TypeTraits<T>.TypeName;

			var message = NullFallbackMessageProvider.GetMessage(
				typeName,
				new ValidationContext<string>(null)
			);

			var failures = new[] { new ValidationFailure(typeName, message) };
			return new ValidationResult(
				failures
			);
		}
	}
}
