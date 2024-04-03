using FluentValidation;
using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace ExpressValidator
{
	internal class ExpressPropertyValidator<T> : IExpressPropertyValidator<T>
	{
		protected readonly string _propName;
		protected TypeValidatorBase<T> _typeValidator;

		public ExpressPropertyValidator(PropertyInfo propertyInfo, TypeValidatorBase<T> typeValidator)
		{
			PropInfo = propertyInfo;
			_propName = propertyInfo?.Name ?? string.Empty;
			_typeValidator = typeValidator;
		}

		public void SetValidation(Action<IRuleBuilderOptions<T, T>> action)
		{
			_typeValidator.SetValidation(action, _propName);
		}

		public Task<(bool IsValid, List<ValidationFailure> Failures)> ValidateAsync<TObj>(TObj obj, CancellationToken token = default)
		{
			return _typeValidator.ValidateExAsync(PropInfo.GetTypedValue<TObj, T>(obj), token);
		}

		public (bool IsValid, List<ValidationFailure> Failures) Validate<TObj>(TObj obj)
		{
			if (IsAsync)
			{
				throw new InvalidOperationException();
			}
			return _typeValidator.ValidateEx(PropInfo.GetTypedValue<TObj, T>(obj));
		}

		protected PropertyInfo PropInfo { get; set; }

		public bool IsAsync => _typeValidator.IsAsync == true;
	}
}
