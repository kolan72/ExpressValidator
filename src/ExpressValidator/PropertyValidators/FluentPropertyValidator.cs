using FluentValidation;
using System;

namespace ExpressValidator
{
	internal class FluentPropertyValidator<TObj, T> : AbstractValidator<TObj>
    {
        public FluentPropertyValidator(Func<TObj, T> propertyFunc, string propName, TypeValidatorBase<T> typeValidator)
        {
           RuleFor((obj) => propertyFunc(obj)).NotNull().SetValidator(typeValidator).OverridePropertyName(propName);
        }
    }
}
