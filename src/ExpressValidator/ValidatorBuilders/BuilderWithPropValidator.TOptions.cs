using FluentValidation;
using System;
using System.Reflection;

namespace ExpressValidator
{
	public class BuilderWithPropValidator<TObj, TOptions, T> : IBuilderWithPropValidator<TObj, TOptions, T>
	{
		private readonly string _propName;
		private readonly Func<TObj, T> _propertyFunc;

		internal BuilderWithPropValidator(ExpressValidatorBuilder<TObj, TOptions> expressValidatorBuilder, MemberInfo memberInfo)
										: this(expressValidatorBuilder, memberInfo.GetTypedValue<TObj, T>, memberInfo?.Name ?? string.Empty)
		{ }

		internal BuilderWithPropValidator(ExpressValidatorBuilder<TObj, TOptions> expressValidatorBuilder, Func<TObj, T> propertyFunc, string propName)
		{
			ExpressValidatorBuilder = expressValidatorBuilder;
			_propertyFunc = propertyFunc;
			_propName = propName;
		}

		public ExpressValidatorBuilder<TObj, TOptions> WithAsyncValidation(Action<TOptions, IRuleBuilderOptions<T, T>> action)
		{
			return WithValidationByRules(action, new ExpressPropertyValidator<TObj, TOptions, T>(_propertyFunc, _propName, true));
		}

		public ExpressValidatorBuilder<TObj, TOptions> WithValidation(Action<TOptions, IRuleBuilderOptions<T, T>> action)
		{
			return WithValidationByRules(action, new ExpressPropertyValidator<TObj, TOptions, T>(_propertyFunc, _propName, false));
		}

		private ExpressValidatorBuilder<TObj, TOptions> WithValidationByRules(Action<TOptions, IRuleBuilderOptions<T, T>> action, IExpressPropertyValidator<TObj, TOptions, T> expressPropertyValidator)
		{
			var propertyValidator = expressPropertyValidator;
			propertyValidator.SetValidation(action);
			ExpressValidatorBuilder.AddValidator(propertyValidator);
			return ExpressValidatorBuilder;
		}

		private ExpressValidatorBuilder<TObj, TOptions> ExpressValidatorBuilder { get; }
	}
}
