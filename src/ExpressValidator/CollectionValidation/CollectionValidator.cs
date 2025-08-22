using FluentValidation.Results;
using System;
using System.Collections.Generic;
using ExpressValidator.Extensions;

namespace ExpressValidator.CollectionValidation
{
	public static class CollectionValidator
	{
		private const string FALLBACK_PROP_NAME = "Input collection";

		public static ValidationResult Validate<T>( IEnumerable<T> collection,
													Action<ExpressValidatorBuilder<T>> configure,
													string propName,
													OnFirstPropertyValidatorFailed onFirstPropertyValidatorFailed = OnFirstPropertyValidatorFailed.Continue,
													Action<IEnumerable<T>> onSuccessValidation = null)
		{
			return ValidateInner(collection, configure, propName ?? FALLBACK_PROP_NAME, onFirstPropertyValidatorFailed, onSuccessValidation);
		}

		private static ValidationResult ValidateInner<T>(IEnumerable<T> collection,
														 Action<ExpressValidatorBuilder<T>> configure,
														 string propName,
														 OnFirstPropertyValidatorFailed onFirstPropertyValidatorFailed = OnFirstPropertyValidatorFailed.Continue,
														 Action<IEnumerable<T>> onSuccessValidation = null)
		{
			var eb = new ExpressValidatorBuilder<Unit>();

			var validator = new CollectionValidator<T>(configure, onFirstPropertyValidatorFailed);

			return eb.AddFunc((_) => collection,
								propName,
								onSuccessValidation)
					.WithValidation(o => o.SetValidator(validator))
					.BuildAndValidate(Unit.Default);
		}
	}
}
