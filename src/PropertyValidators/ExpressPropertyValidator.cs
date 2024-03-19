using FluentValidation;
using FluentValidation.Results;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace ExpressValidator
{
	internal class ExpressPropertyValidator<T> : ExpressPropertyValidatorBase<T>, IExpressPropertyValidator<T>
	{
		public ExpressPropertyValidator(PropertyInfo propertyInfo) : base(propertyInfo)
		{
			_ruleBuilderInitial = RuleFor(i => i).Must((_) => true);
		}

		public bool IsAsync => false;

		public (bool IsValid, List<ValidationFailure> Failures) Validate<TObj>(TObj obj)
		{
			return ValidateEx(GetPropertyValue(obj));
		}

		public Task<(bool IsValid, List<ValidationFailure> Failures)> ValidateAsync<TObj>(TObj obj, CancellationToken token = default)
		{
			return ValidateExAsync(GetPropertyValue(obj), token);
		}

		public (bool IsValid, List<ValidationFailure> Failures) ValidateEx(T value)
		{
			if (ShouldValidate(value))
			{
				var validRes = base.Validate(value);
				if (validRes.IsValid)
					return (true, null);
				else
					return (false, validRes.Errors);
			}
			else
			{
				return (true, null);
			}
		}
	}
}
