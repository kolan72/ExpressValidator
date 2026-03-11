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
		private PropertyValidationProcessor<TObj, T> _validationProcessor;

		public ExpressPropertyValidator(Func<TObj, T> propertyFunc, string propName, TypeValidatorBase<T> typeValidator, Action<T> onSuccessValidation = null)
		{
			_propertyFunc = propertyFunc;
			_propName = propName;
			_typeValidator = typeValidator;
			IsAsync = _typeValidator.IsAsync == true;
			_onSuccessValidation = onSuccessValidation;
		}

		public void SetValidation(Action<IRuleBuilderOptions<T, T>> action)
		{
			_typeValidator.SetValidation(action, _propName);
			_validationProcessor = new PropertyValidationProcessor<TObj, T>(_propertyFunc, _typeValidator, _onSuccessValidation);
		}

		public Task<(bool IsValid, List<ValidationFailure> Failures)> ValidateAsync(TObj obj, CancellationToken token = default)
		{
			return _validationProcessor.ValidateAsync(obj, token);
		}

		public (bool IsValid, List<ValidationFailure> Failures) Validate(TObj obj)
		{
			return _validationProcessor.Validate(obj);
		}

		public bool IsAsync { get; }
	}
}
