using FluentValidation.Results;

namespace ExpressValidator.Extensions
{
	public static class ExpressValidatorExtensions
	{
		/// <summary>
		/// Builds an <see cref="IExpressValidatorBuilder{TObj}"/> to create a validator and validate an <paramref name="obj"/> object using the created validator.
		/// </summary>
		/// <typeparam name="TObj">A type of object to validate.</typeparam>
		/// <param name="validatorBuilder"><see cref="IExpressValidatorBuilder{TObj}"/></param>
		/// <param name="obj">An object instance to validate.</param>
		/// <returns><see cref="ValidationResult"/></returns>
		public static ValidationResult BuildAndValidate<TObj>(this IExpressValidatorBuilder<TObj> validatorBuilder, TObj obj)
		{
			return validatorBuilder.Build().Validate(obj);
		}
	}
}
