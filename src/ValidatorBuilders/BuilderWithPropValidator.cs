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
		private readonly string _propName;
		private readonly Func<TObj, T> _propertyFunc;

		internal BuilderWithPropValidator(ExpressValidatorBuilder<TObj> expressValidatorBuilder, MemberInfo memberInfo)
										: this(expressValidatorBuilder, memberInfo.GetTypedValue<TObj, T>, memberInfo?.Name ?? string.Empty)
		{}

		internal BuilderWithPropValidator(ExpressValidatorBuilder<TObj> expressValidatorBuilder, Func<TObj, T> propertyFunc, string propName)
		{
			ExpressValidatorBuilder = expressValidatorBuilder;
			_propertyFunc = propertyFunc;
			_propName = propName;
		}

		public ExpressValidatorBuilder<TObj> WithValidation(Action<IRuleBuilderOptions<T, T>> action)
		{
			return WithValidationByRules(action, new ExpressPropertyValidator<TObj, T>(_propertyFunc, _propName, new TypeValidator<T>()));
		}

		public ExpressValidatorBuilder<TObj> WithAsyncValidation(Action<IRuleBuilderOptions<T, T>> action)
		{
			return WithValidationByRules(action, new ExpressPropertyValidator<TObj, T>(_propertyFunc, _propName, new TypeAsyncValidator<T>()));
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
