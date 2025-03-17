using FluentValidation.Results;
using System.Threading;
using System.Threading.Tasks;

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

		/// <summary>
		/// Builds an <see cref="IExpressValidatorBuilder{TObj, TOptions}"/> that uses <paramref name="options"/> to create a validator and use a validator for <paramref name="obj"/> validation.
		/// </summary>
		/// <typeparam name="TObj">A type of object to validate.</typeparam>
		/// <typeparam name="TOptions">A type of options to validate.</typeparam>
		/// <param name="validatorBuilder"><see cref="IExpressValidatorBuilder{TObj, TOptions}"/></param>
		/// <param name="obj">An object instance to validate.</param>
		/// <param name="options">Options for builder</param>
		/// <returns><see cref="ValidationResult"/></returns>
		public static ValidationResult BuildAndValidate<TObj, TOptions>(this IExpressValidatorBuilder<TObj, TOptions> validatorBuilder, TObj obj, TOptions options)
		{
			return validatorBuilder.Build(options).Validate(obj);
		}

		/// <summary>
		///  Builds an <see cref="IExpressValidatorBuilder{TObj}"/> to create a validator and asynchronously validate an <paramref name="obj"/> object using the created validator.
		/// </summary>
		/// <typeparam name="TObj">A type of object to validate.</typeparam>
		/// <param name="validatorBuilder"><see cref="IExpressValidatorBuilder{TObj}"/></param>
		/// <param name="obj">An object instance to validate.</param>
		/// <param name="token">A cancellation token to cancel validation.</param>
		/// <returns>A task that wraps <see cref="ValidationResult"/>.</returns>
		public static Task<ValidationResult> BuildAndValidateAsync<TObj>(this IExpressValidatorBuilder<TObj> validatorBuilder, TObj obj, CancellationToken token = default)
		{
			return validatorBuilder.Build().ValidateAsync(obj, token);
		}
	}
}
