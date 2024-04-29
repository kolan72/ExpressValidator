using FluentValidation;
using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace ExpressValidator
{
	internal class ExpressPropertyValidator<TObj, T> : IExpressPropertyValidator<TObj, T>
	{
		protected readonly string _propName;
		protected TypeValidatorBase<T> _typeValidator;

		public ExpressPropertyValidator(MemberInfo memberInfo, TypeValidatorBase<T> typeValidator)
		{
			ValidatingInfo = memberInfo;
			_propName = ValidatingInfo?.Name ?? string.Empty;
			_typeValidator = typeValidator;
		}

		public void SetValidation(Action<IRuleBuilderOptions<T, T>> action)
		{
			_typeValidator.SetValidation(action, _propName);
		}

		public Task<(bool IsValid, List<ValidationFailure> Failures)> ValidateAsync(TObj obj, CancellationToken token = default)
		{
			return _typeValidator.ValidateExAsync(ValidatingInfo.GetTypedValue<TObj, T>(obj), token);
		}

		public (bool IsValid, List<ValidationFailure> Failures) Validate(TObj obj)
		{
			if (IsAsync)
			{
				throw new InvalidOperationException();
			}
			return _typeValidator.ValidateEx(ValidatingInfo.GetTypedValue<TObj, T>(obj));
		}

		protected MemberInfo ValidatingInfo { get; set; }

		public bool IsAsync => _typeValidator.IsAsync == true;
	}
}
