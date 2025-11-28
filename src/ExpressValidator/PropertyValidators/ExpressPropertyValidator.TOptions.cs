using FluentValidation;
using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ExpressValidator
{
	internal class ExpressPropertyValidator<TObj, TOptions, T> : IExpressPropertyValidator<TObj, TOptions, T>
	{
		private readonly string _propName;
		private readonly Func<TObj, T> _propertyFunc;
		private TypeValidatorBase<T> _typeValidator;

		private Action<TOptions, IRuleBuilderOptions<T, T>> _actionWithOptions;

		private Action<IRuleBuilderOptions<T, T>> _action;

		private readonly Action<T> _onSuccessValidation;

		public ExpressPropertyValidator(Func<TObj, T> propertyFunc, string propName, bool isAsync, Action<T> onSuccessValidation = null)
		{
			_propertyFunc = propertyFunc;
			_propName = propName;
			IsAsync = isAsync;
			_onSuccessValidation = onSuccessValidation;
		}

		public void SetValidation(Action<TOptions, IRuleBuilderOptions<T, T>> action)
		{
			_actionWithOptions = action;
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

		public void ApplyOptions(TOptions options)
		{
			_action =_actionWithOptions.Apply(options);
			SetTypeValidator();
		}

		private void SetTypeValidator()
		{
			if (IsAsync)
			{
				_typeValidator = new TypeAsyncValidator<T>();
			}
			else
			{
				_typeValidator = new TypeValidator<T>();
			}
			_typeValidator.SetValidation(_action, _propName);
		}

		public bool IsAsync { get; }
	}
}
