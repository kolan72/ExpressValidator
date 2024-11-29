using FluentValidation;
using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ExpressValidator
{
	internal class ExpressPropertyValidator<TObj, T> : IExpressPropertyValidator<TObj, T>
	{
		private readonly string _propName;
		private readonly TypeValidatorBase<T> _typeValidator;
		private readonly Func<TObj, T> _propertyFunc;

		public ExpressPropertyValidator(Func<TObj, T> propertyFunc, string propName, TypeValidatorBase<T> typeValidator)
		{
			_propertyFunc = propertyFunc;
			_propName = propName;
			_typeValidator = typeValidator;
		}

		public void SetValidation(Action<IRuleBuilderOptions<T, T>> action)
		{
			_typeValidator.SetValidation(action, _propName);
		}

		public Task<(bool IsValid, List<ValidationFailure> Failures)> ValidateAsync(TObj obj, CancellationToken token = default)
		{
			return _typeValidator.ValidateExAsync(_propertyFunc(obj), token);
		}

		public (bool IsValid, List<ValidationFailure> Failures) Validate(TObj obj)
		{
			if (IsAsync)
			{
				throw new InvalidOperationException();
			}
			return _typeValidator.ValidateEx(_propertyFunc(obj));
		}

		public bool IsAsync => _typeValidator.IsAsync == true;
	}
}
