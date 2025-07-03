using ExpressValidator.Extensions;
using FluentValidation;
using FluentValidation.Results;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ExpressValidator.QuickValidation
{
	/// <summary>
	/// Provides methods for quick validation.
	/// </summary>
	public static class QuickValidator
	{
		private const string FALLBACK_PROP_NAME = "Input";

		/// <summary>
		/// Validates the given object instance using <paramref name="action"/>.
		/// </summary>
		/// <typeparam name="T">The type of the object to validate.</typeparam>
		/// <param name="obj">The object to validate.</param>
		/// <param name="action">Action to add validators.</param>
		/// <param name="propName">The name of the property if the validation fails.
		/// If <see langword="null"/>, "Input" will be used.</param>
		/// <param name="onSuccessValidation">Specifies a method to execute when validation succeeds.</param>
		/// <returns></returns>
		public static ValidationResult Validate<T>(T obj, Action<IRuleBuilderOptions<T, T>> action, string propName, Action<T> onSuccessValidation = null)
		{
			return ValidateInner<T>(obj, action, propName ?? FALLBACK_PROP_NAME, onSuccessValidation);
		}

		/// <summary>
		/// Validates the given object instance using <paramref name="action"/>.
		/// </summary>
		/// <typeparam name="T">The type of the object to validate.</typeparam>
		/// <param name="obj">The object to validate.</param>
		/// <param name="action">Action to add validators.</param>
		/// <param name="mode"><see cref="PropertyNameMode"/>.
		/// If <see cref="PropertyNameMode.Default"/>, "Input" will be used.</param>
		/// <param name="onSuccessValidation">Specifies a method to execute when validation succeeds.</param>
		/// <returns></returns>
		public static ValidationResult Validate<T>(T obj, Action<IRuleBuilderOptions<T, T>> action, PropertyNameMode mode = PropertyNameMode.Default, Action<T> onSuccessValidation = null)
		{
			return ValidateInner<T>(obj, action, GetPropName<T>(mode), onSuccessValidation);
		}

		private static ValidationResult ValidateInner<T>(T obj, Action<IRuleBuilderOptions<T, T>> action, string propName, Action<T> onSuccessValidation = null)
		{
			var eb = new ExpressValidatorBuilder<Unit>();
			return eb.AddFunc((_) => obj,
								propName,
								onSuccessValidation)
					.WithValidation(action)
					.BuildAndValidate(Unit.Default);
		}

		/// <summary>
		/// Asynchronously validates the given object instance using <paramref name="action"/>.
		/// </summary>
		/// <typeparam name="T">The type of the object to validate.</typeparam>
		/// <param name="obj">The object to validate.</param>
		/// <param name="action">Action to add validators.</param>
		/// <param name="propName">The name of the property if the validation fails.
		/// If <see langword="null"/>, "Input" will be used.</param>
		/// <param name="onSuccessValidation">Specifies a method to execute when validation succeeds.</param>
		/// <param name="token">>A cancellation token to cancel validation.</param>
		/// <returns></returns>
		public static Task<ValidationResult> ValidateAsync<T>(T obj, Action<IRuleBuilderOptions<T, T>> action, string propName, Action<T> onSuccessValidation = null, CancellationToken token = default)
		{
			return ValidateInnerAsync(obj, action, propName ?? FALLBACK_PROP_NAME, onSuccessValidation, token);
		}

		/// <summary>
		/// Asynchronously validates the given object instance using <paramref name="action"/>.
		/// </summary>
		/// <typeparam name="T">The type of the object to validate.</typeparam>
		/// <param name="obj">The object to validate.</param>
		/// <param name="action">Action to add validators.</param>
		/// <param name="mode"><see cref="PropertyNameMode"/>.
		/// If <see cref="PropertyNameMode.Default"/>, "Input" will be used.</param>
		/// <param name="onSuccessValidation">Specifies a method to execute when validation succeeds.</param>
		/// <param name="token">>A cancellation token to cancel validation.</param>
		/// <returns></returns>
		public static Task<ValidationResult> ValidateAsync<T>(T obj, Action<IRuleBuilderOptions<T, T>> action, PropertyNameMode mode = PropertyNameMode.Default, Action<T> onSuccessValidation = null, CancellationToken token = default)
		{
			return ValidateInnerAsync(obj, action, GetPropName<T>(mode), onSuccessValidation, token);
		}

		private static Task<ValidationResult> ValidateInnerAsync<T>(T obj, Action<IRuleBuilderOptions<T, T>> action, string propName, Action<T> onSuccessValidation = null, CancellationToken token = default)
		{
			var eb = new ExpressValidatorBuilder<Unit>();
			return eb.AddFunc((_) => obj,
								propName,
								onSuccessValidation)
					.WithAsyncValidation(action)
					.BuildAndValidateAsync(Unit.Default, token);
		}

		private static string GetPropName<T>(PropertyNameMode mode) => mode == PropertyNameMode.Default ? FALLBACK_PROP_NAME : typeof(T).Name;
	}
}
