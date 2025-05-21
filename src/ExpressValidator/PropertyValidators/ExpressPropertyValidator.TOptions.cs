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

		public ExpressPropertyValidator(Func<TObj, T> propertyFunc, string propName, bool isAsync)
		{
			_propertyFunc = propertyFunc;
			_propName = propName;
			IsAsync = isAsync;
		}

		public void SetValidation(Action<TOptions, IRuleBuilderOptions<T, T>> action)
		{
			_actionWithOptions = action;
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
