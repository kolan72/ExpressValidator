using FluentValidation;
using System;
using System.Reflection;

namespace ExpressValidator
{
	/// <summary>
	/// This class is primarily for internal use by ExpressValidator.
	/// </summary>
	public class BuilderWithPropValidator<TObj, T> : IBuilderWithPropValidator<TObj, T>
	{
		private readonly MemberInfo _validatingInfo;

		internal BuilderWithPropValidator(ExpressValidatorBuilder<TObj> expressValidatorBuilder, MemberInfo memberInfo)
		{
			ExpressValidatorBuilder = expressValidatorBuilder;
			_validatingInfo = memberInfo;
		}

		public ExpressValidatorBuilder<TObj> WithValidation(Action<IRuleBuilderOptions<T, T>> action)
		{
			return WithValidationByRules(action, new ExpressPropertyValidator<TObj, T>(_validatingInfo, new TypeValidator<T>()));
		}

		public ExpressValidatorBuilder<TObj> WithAsyncValidation(Action<IRuleBuilderOptions<T, T>> action)
		{
			return WithValidationByRules(action, new ExpressPropertyValidator<TObj, T>(_validatingInfo, new TypeAsyncValidator<T>()));
		}

		private ExpressValidatorBuilder<TObj> WithValidationByRules(Action<IRuleBuilderOptions<T, T>> action, IExpressPropertyValidator<TObj, T> expressPropertyValidator)
		{
			var propertyValidator = expressPropertyValidator;
			propertyValidator.SetValidation(action);
			ExpressValidatorBuilder.AddValidator(propertyValidator);
			return ExpressValidatorBuilder;
		}

		private ExpressValidatorBuilder<TObj> ExpressValidatorBuilder { get; }
	}
}
