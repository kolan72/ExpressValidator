using FluentValidation;
using System;
using System.Collections.Generic;

namespace ExpressValidator.CollectionValidation
{
	public class CollectionValidator<T> : AbstractValidator<IEnumerable<T>>
	{
        public CollectionValidator(Action<ExpressValidatorBuilder<T>> configure, OnFirstPropertyValidatorFailed onFirstPropertyValidatorFailed = OnFirstPropertyValidatorFailed.Continue)
        {
			RuleFor(x => x)
			 .NotEmpty().WithMessage("Collection cannot be empty.")
			 .Custom((collection, context) =>
			 {
				 if (collection == null) return;
				 var eb = new ExpressValidatorBuilder<T>(onFirstPropertyValidatorFailed);
				 configure(eb);

				 var validator = eb.Build();

				 foreach (var item in collection)
				 {
					 var result = validator.Validate(item);
					 if (!result.IsValid)
					 {
						 foreach (var error in result.Errors)
						 {
							 context.AddFailure(error);
						 }
					 }
				 }
			 });
		}
    }
}
