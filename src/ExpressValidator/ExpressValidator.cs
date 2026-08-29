using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ExpressValidator
{
	/// <summary>
	///  Defines a validator for an object.
	/// </summary>
	public class ExpressValidator<TObj> : IExpressValidator<TObj>
	{
		private readonly IReadOnlyList<IObjectValidatorBase<TObj>> _validators;
		private readonly OnFirstPropertyValidatorFailed _validationMode;
		// Computed once at construction; the validator list is immutable after Build().
		private readonly bool _hasAsyncValidators;

		internal ExpressValidator(IEnumerable<IObjectValidatorBase<TObj>> validators, OnFirstPropertyValidatorFailed validationMode)
		{
			_validators = validators as IReadOnlyList<IObjectValidatorBase<TObj>> ?? validators.ToList();
			_validationMode = validationMode;
			_hasAsyncValidators = _validators.Any(v => v.IsAsync);
		}

		public ValidationResult Validate(TObj obj)
		{
			if (_hasAsyncValidators)
			{
				throw new InvalidOperationException($"Object validator has a property or field with asynchronous validation rules. Please use {nameof(ValidateAsync)} method.");
			}
			if (TypeHelper<TObj>.IsNull(obj))
			{
				return ValidationFallbackProvider.GetNullFailure<TObj>();
			}
			return _validationMode == OnFirstPropertyValidatorFailed.Break ? ValidateWithBreak(obj) : ValidateWithContinue(obj);
		}

		public Task<ValidationResult> ValidateAsync(TObj obj, CancellationToken token = default)
		{
			if (TypeHelper<TObj>.IsNull(obj))
			{
				return Task.FromResult(ValidationFallbackProvider.GetNullFailure<TObj>());
			}
			return _validationMode == OnFirstPropertyValidatorFailed.Break ? ValidateWithBreakAsync(obj, token) : ValidateWithContinueAsync(obj, token);
		}

		private async Task<ValidationResult> ValidateWithBreakAsync(TObj obj, CancellationToken token)
		{
			foreach (var validator in _validators)
			{
				token.ThrowIfCancellationRequested();

				var (IsValid, Failures) = await validator.ValidateAsync(obj, token).ConfigureAwait(false);
				if (!IsValid)
				{
					return new ValidationResult(Failures);
				}
			}
			return new ValidationResult();
		}

		private async Task<ValidationResult> ValidateWithContinueAsync(TObj obj, CancellationToken token)
		{
			var currentFailures = new List<ValidationFailure>();
			foreach (var validator in _validators)
			{
				token.ThrowIfCancellationRequested();

				var (IsValid, Failures) = await validator.ValidateAsync(obj, token);
				if (!IsValid)
				{
					currentFailures.AddRange(Failures);
				}
			}
			return new ValidationResult(currentFailures);
		}

		private ValidationResult ValidateWithBreak(TObj obj)
		{
			foreach (var validator in _validators)
			{
				var (IsValid, Failures) = validator.Validate(obj);
				if (!IsValid)
				{
					return new ValidationResult(Failures);
				}
			}
			return new ValidationResult();
		}

		private ValidationResult ValidateWithContinue(TObj obj)
		{
			var currentFailures = new List<ValidationFailure>();
			foreach (var validator in _validators)
			{
				var (IsValid, Failures) = validator.Validate(obj);
				if (!IsValid)
				{
					currentFailures.AddRange(Failures);
				}
			}
			return new ValidationResult(currentFailures);
		}
	}
}
