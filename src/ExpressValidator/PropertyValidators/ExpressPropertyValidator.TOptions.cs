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

		private Action<TOptions, IRuleBuilderOptions<T, T>> _actionWithOptions;

		private Action<IRuleBuilderOptions<T, T>> _action;

		private readonly Action<T> _onSuccessValidation;

		private PropertyValidationProcessor<TObj, T> _validationProcessor;

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

		public Task<(bool IsValid, List<ValidationFailure> Failures)> ValidateAsync(TObj obj, CancellationToken token = default)
		{
			return _validationProcessor.ValidateAsync(obj, token);
		}

		public (bool IsValid, List<ValidationFailure> Failures) Validate(TObj obj)
		{
			return _validationProcessor.Validate(obj);
		}

		public void ApplyOptions(TOptions options)
		{
			_action =_actionWithOptions.Apply(options);
			SetTypeValidator();
		}

		private void SetTypeValidator()
		{
			TypeValidatorBase<T> typeValidator;

			if (IsAsync)
			{
				typeValidator = new TypeAsyncValidator<T>();
			}
			else
			{
				typeValidator = new TypeValidator<T>();
			}
			typeValidator.SetValidation(_action, _propName);
			_validationProcessor = new PropertyValidationProcessor<TObj, T>(_propertyFunc, typeValidator, _onSuccessValidation);
		}

		public bool IsAsync { get; }
	}
}
