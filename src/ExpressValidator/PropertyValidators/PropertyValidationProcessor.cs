using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

namespace ExpressValidator
{
	internal class PropertyValidationProcessor<TObj, T>
	{
		private readonly TypeValidatorBase<T> _typeValidator;
		private readonly Func<TObj, T> _propertyFunc;
		private readonly bool _isAsync;
		private readonly Action<T> _onSuccessValidation;

		public PropertyValidationProcessor(Func<TObj, T> propertyFunc, TypeValidatorBase<T> typeValidator, Action<T> onSuccessValidation)
        {
			_propertyFunc = propertyFunc;
			_typeValidator = typeValidator;
			_isAsync = _typeValidator.IsAsync == true;
			_onSuccessValidation = onSuccessValidation;
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
			if (_isAsync)
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
	}
}
