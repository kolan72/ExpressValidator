using FluentValidation;
using FluentValidation.Results;
using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace ExpressValidator.QuickValidation
{
	/// <summary>
	/// Represents a validation operation that can throw an exception on failure.
	/// </summary>
	/// <typeparam name="T">The type of the object being validated.</typeparam>
	public struct RequireValidator<T>
	{
		private readonly T _obj;
		private readonly string _expression;
		private readonly Action<IRuleBuilderOptions<T, T>> _action;

		internal RequireValidator(T obj, string expression, Action<IRuleBuilderOptions<T, T>> action)
		{
			_obj = obj;
			_expression = expression;
			_action = action;
		}

		/// <summary>
		/// Validates the object and throws <see cref="InvalidOperationException"/> if validation fails.
		/// </summary>
		/// <param name="message">Custom exception message. If null, a default message is generated.</param>
		/// <param name="propName">The property name to use. If null, the captured expression is used.</param>
		public void Raise(string message = null, string propName = null)
		{
			var actualPropName = propName ?? _expression ?? "Input";
			var result = QuickValidator.Validate(_obj, _action, actualPropName);

			if (result.IsValid == false)
			{
				throw new InvalidOperationException(message ?? BuildDefaultMessage(result, actualPropName));
			}
		}

		/// <summary>
		/// Validates the object and throws an exception created by the factory if validation fails.
		/// </summary>
		/// <param name="exceptionFactory">A function that creates the exception to throw.</param>
		/// <param name="propName">The property name to use. If null, the captured expression is used.</param>
		public void Raise(Func<ValidationResult, string, Exception> exceptionFactory, string propName = null)
		{
			var actualPropName = propName ?? _expression ?? "Input";
			var result = QuickValidator.Validate(_obj, _action, actualPropName);

			if (result.IsValid == false)
			{
				throw exceptionFactory(result, actualPropName);
			}
		}

		/// <summary>
		/// Validates the object and throws an exception of the specified type if validation fails.
		/// </summary>
		/// <typeparam name="TException">The exception type. Must have a constructor accepting a string message.</typeparam>
		/// <param name="message">Custom exception message. If null, a default message is generated.</param>
		/// <param name="propName">The property name to use. If null, the captured expression is used.</param>
		public void Raise<TException>(string message = null, string propName = null) where TException : Exception
		{
			var actualPropName = propName ?? _expression ?? "Input";
			var result = QuickValidator.Validate(_obj, _action, actualPropName);

			if (result.IsValid == false)
			{
				var exceptionMessage = message ?? BuildDefaultMessage(result, actualPropName);
				throw (TException)Activator.CreateInstance(typeof(TException), exceptionMessage);
			}
		}

		/// <summary>
		/// Asynchronously validates the object and throws <see cref="InvalidOperationException"/> if validation fails.
		/// </summary>
		/// <param name="message">Custom exception message. If null, a default message is generated.</param>
		/// <param name="propName">The property name to use. If null, the captured expression is used.</param>
		/// <param name="token">A cancellation token.</param>
		public async Task RaiseAsync(string message = null, string propName = null, CancellationToken token = default)
		{
			var actualPropName = propName ?? _expression ?? "Input";
			var result = await QuickValidator.ValidateAsync(_obj, _action, actualPropName, token: token);

			if (result.IsValid == false)
			{
				throw new InvalidOperationException(message ?? BuildDefaultMessage(result, actualPropName));
			}
		}

		/// <summary>
		/// Asynchronously validates the object and throws an exception created by the factory if validation fails.
		/// </summary>
		/// <param name="exceptionFactory">A function that creates the exception to throw.</param>
		/// <param name="propName">The property name to use. If null, the captured expression is used.</param>
		/// <param name="token">A cancellation token.</param>
		public async Task RaiseAsync(Func<ValidationResult, string, Exception> exceptionFactory, string propName = null, CancellationToken token = default)
		{
			var actualPropName = propName ?? _expression ?? "Input";
			var result = await QuickValidator.ValidateAsync(_obj, _action, actualPropName, token: token);

			if (result.IsValid == false)
			{
				throw exceptionFactory(result, actualPropName);
			}
		}

		/// <summary>
		/// Asynchronously validates the object and throws an exception of the specified type if validation fails.
		/// </summary>
		/// <typeparam name="TException">The exception type. Must have a constructor accepting a string message.</typeparam>
		/// <param name="message">Custom exception message. If null, a default message is generated.</param>
		/// <param name="propName">The property name to use. If null, the captured expression is used.</param>
		/// <param name="token">A cancellation token.</param>
		public async Task RaiseAsync<TException>(string message = null, string propName = null, CancellationToken token = default) where TException : Exception
		{
			var actualPropName = propName ?? _expression ?? "Input";
			var result = await QuickValidator.ValidateAsync(_obj, _action, actualPropName, token: token);

			if (result.IsValid == false)
			{
				var exceptionMessage = message ?? BuildDefaultMessage(result, actualPropName);
				throw (TException)Activator.CreateInstance(typeof(TException), exceptionMessage);
			}
		}

		private static string BuildDefaultMessage(ValidationResult result, string propName)
		{
			var errors = string.Join("; ", result.Errors);
			return $"Validation of '{propName}' failed: {errors}";
		}
	}

	/// <summary>
	/// Extension methods to start require-style validation on any object.
	/// </summary>
	public static class RequireExtensions
	{
		/// <summary>
		/// Starts a require-style validation chain on the specified object.
		/// </summary>
		/// <typeparam name="T">The type of the object to validate.</typeparam>
		/// <param name="obj">The object to validate.</param>
		/// <param name="action">Action to add validators.</param>
		/// <param name="expression">The expression that was validated (automatically captured).</param>
		/// <returns>A <see cref="RequireValidator{T}"/> for calling Raise or RaiseAsync.</returns>
		public static RequireValidator<T> Require<T>(
			this T obj,
			Action<IRuleBuilderOptions<T, T>> action,
#if NET8_0_OR_GREATER
			[CallerArgumentExpression("obj")] string expression = null
#else
			string expression = null
#endif
			)
		{
			Action<IRuleBuilderOptions<T, T>> actWithName = (opt) => opt.WithName(expression);
			Action<IRuleBuilderOptions<T, T>> act = (opt) => { action(opt); actWithName(opt); };
			return new RequireValidator<T>(obj, expression, act);
		}
	}
}
