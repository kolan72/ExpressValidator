using FluentValidation;
using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace ExpressValidator
{
	internal abstract class ExpressPropertyValidatorBase<T>
	{
		protected readonly string _propName;
		protected TypeValidatorBase<T> _typeValidator;

		protected ExpressPropertyValidatorBase(PropertyInfo propertyInfo)
		{
			PropInfo = propertyInfo;
			_propName = propertyInfo?.Name ?? string.Empty;
		}

		public void SetValidation(Action<IRuleBuilderOptions<T, T>> action)
		{
			_typeValidator.SetValidation(action, _propName);
		}

		public Task<(bool IsValid, List<ValidationFailure> Failures)> ValidateAsync<TObj>(TObj obj, CancellationToken token = default)
		{
			return _typeValidator.ValidateExAsync(GetPropertyValue(obj), token);
		}

		protected T GetPropertyValue<TObj>(TObj obj)
		{
			return PropInfo.GetTypedValue<TObj, T>(obj);
		}

		protected PropertyInfo PropInfo { get; set; }
	}
}
