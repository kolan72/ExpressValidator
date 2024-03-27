using FluentValidation;

namespace ExpressValidator
{
	internal class TypeValidator<T> : TypeValidatorBase<T>
	{
		public TypeValidator()
		{
			_ruleBuilderInitial = RuleFor(i => i).Must((_) => true);
		}
	}
}
