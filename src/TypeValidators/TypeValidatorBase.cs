using FluentValidation;
using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ExpressValidator
{
	internal abstract class TypeValidatorBase<T> : AbstractValidator<T>
	{
		protected IRuleBuilderOptions<T, T> _ruleBuilderInitial;
		protected NotNullValidationMessageProvider<T> _nullMessageProvider;

		protected IValidationRule<T> _rule;
		protected string _propName;

		protected TypeValidatorBase()
		{
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
			if (context.InstanceToValidate == null)
			{
				result.Errors.Add(new ValidationFailure(_propName, _nullMessageProvider.GetMessage(context)));
				return false;
			}
			return true;
		}

		public void SetValidation(Action<IRuleBuilderOptions<T, T>> action, string propName)
		{
			action(_ruleBuilderInitial);

			_propName = propName;

			if (_rule.PropertyName is null)
			{
				_ruleBuilderInitial = _ruleBuilderInitial.OverridePropertyName(_propName);
			}

			_nullMessageProvider = new NotNullValidationMessageProvider<T>(_propName);

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

		public (bool IsValid, List<ValidationFailure> Failures) ValidateEx(T value)
		{
			if (ShouldValidate(value))
			{
				var validRes = Validate(value);
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

		internal abstract bool? IsAsync { get; }

		protected bool ShouldValidate(T value) => value != null || !HasOnlyNullOrEmptyValidators;

		private bool HasOnlyNullOrEmptyValidators { get; set; }

		private bool AllValidatorsAreNullOrEmpty()
		{
			return _rule.Components.Skip(1).All(r => r.Validator.Name == "NullValidator" || r.Validator.Name == "EmptyValidator");
		}
	}
}
