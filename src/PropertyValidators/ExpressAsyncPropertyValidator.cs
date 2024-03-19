using FluentValidation;
using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace ExpressValidator
{
	internal class ExpressAsyncPropertyValidator<T> : ExpressPropertyValidatorBase<T>, IExpressPropertyValidator<T>
	{
		public ExpressAsyncPropertyValidator(PropertyInfo propertyInfo) : base(propertyInfo)
		{
			_ruleBuilderInitial = RuleFor(i => i).MustAsync((_, __) => Task.FromResult(true));
		}

		public bool IsAsync => true;

		public (bool IsValid, List<ValidationFailure> Failures) Validate<TObj>(TObj obj)
		{
			throw new InvalidOperationException();
		}

		public Task<(bool IsValid, List<ValidationFailure> Failures)> ValidateAsync<TObj>(TObj obj, CancellationToken token = default)
		{
			return ValidateExAsync(GetPropertyValue(obj), token);
		}
	}
}
