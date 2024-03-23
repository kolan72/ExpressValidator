using FluentValidation;
using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace ExpressValidator
{
	internal abstract class ExpressPropertyValidatorBase<T> : AbstractValidator<T>
	{
		protected  IRuleBuilderOptions<T, T> _ruleBuilderInitial;
		protected readonly NotNullValidationMessageProvider<T> _nullMessageProvider;

		protected IValidationRule<T> _rule;
		protected readonly string _propName;

		protected ExpressPropertyValidatorBase(PropertyInfo propertyInfo)
		{
			PropInfo = propertyInfo;
			_propName = propertyInfo?.Name ?? string.Empty;
			_nullMessageProvider = new NotNullValidationMessageProvider<T>(_propName);
		}

		protected override void OnRuleAdded(IValidationRule<T> rule)
		{
			_rule = rule;
		}

		/// <summary>
		///  Workaround for "Cannot pass null model to Validate." ArgumentNullException, based on <see href="https://github.com/FluentValidation/FluentValidation/issues/2069"/>
		/// </summary>
		/// <param name="context"></param>
		/// <param name="result"></param>
		/// <returns></returns>
		protected override bool PreValidate(ValidationContext<T> context, ValidationResult result)
		{
			if ((typeof(T).IsClass || typeof(T).IsInterface) && context.InstanceToValidate == null)
			{
				result.Errors.Add(new ValidationFailure(_propName, _nullMessageProvider.GetMessage(context)));
				return false;
			}
			return true;
		}

		public void SetValidation(Action<IRuleBuilderOptions<T, T>> action)
		{
			action(_ruleBuilderInitial);

			if(_rule.PropertyName is null)
			{
				_ruleBuilderInitial = _ruleBuilderInitial.OverridePropertyName(_propName);
			}

			HasOnlyNullOrEmptyValidators = AllValidatorsAreNullOrEmpty();
		}

		public async Task<(bool IsValid, List<ValidationFailure> Failures)> ValidateExAsync(T value, CancellationToken token = default)
		{
			if (ShouldValidate(value))
			{
				var validRes = await ValidateAsync(value, token);
				if (validRes.IsValid)
					return (true, null);
				else
					return (false, validRes.Errors);
			}
			else
			{
				return (true, null);
			}
		}

		protected T GetPropertyValue<TObj>(TObj obj)
		{
			return PropInfo.GetTypedValue<TObj, T>(obj);
		}

		protected PropertyInfo PropInfo { get; set; }

		private bool HasOnlyNullOrEmptyValidators { get; set; }

		protected bool ShouldValidate(T value) => value != null || !HasOnlyNullOrEmptyValidators;

		private bool AllValidatorsAreNullOrEmpty()
		{
			return _rule.Components.Skip(1).All(r => r.Validator.Name == "NullValidator" || r.Validator.Name == "EmptyValidator");
		}
	}
}
