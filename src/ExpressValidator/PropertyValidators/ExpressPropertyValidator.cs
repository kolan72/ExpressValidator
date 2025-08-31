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

		private readonly Action<T> _onSuccessValidation;

		public ExpressPropertyValidator(Func<TObj, T> propertyFunc, string propName, TypeValidatorBase<T> typeValidator, Action<T> onSuccessValidation = null)
		{
			_propertyFunc = propertyFunc;
			_propName = propName;
			_typeValidator = typeValidator;
			_onSuccessValidation = onSuccessValidation;
		}

		public void SetValidation(Action<IRuleBuilderOptions<T, T>> action)
		{
			_typeValidator.SetValidation(action, _propName);
		}

		public async Task<(bool IsValid, List<ValidationFailure> Failures)> ValidateAsync(TObj obj, CancellationToken token = default)
		{
			if (_onSuccessValidation != null)
			{
				var value = _propertyFunc(obj);
				var res = await _typeValidator.ValidateExAsync(value, token);
				if (res.IsValid)
				{
					_onSuccessValidation(value);
				}
				return res;
			}
			else
			{
				return await _typeValidator.ValidateExAsync(_propertyFunc(obj), token);
			}
		}

		public (bool IsValid, List<ValidationFailure> Failures) Validate(TObj obj)
		{
			if (IsAsync)
			{
				throw new InvalidOperationException();
			}
			if (_onSuccessValidation != null)
			{
				var value = _propertyFunc(obj);
				var res = _typeValidator.ValidateEx(value);
				if (res.IsValid)
				{
					_onSuccessValidation(value);
				}
				return res;
			}
			else
			{
				return _typeValidator.ValidateEx(_propertyFunc(obj));
			}
		}

		public bool IsAsync => _typeValidator.IsAsync == true;
	}
}
