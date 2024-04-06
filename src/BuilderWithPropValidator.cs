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
		private readonly PropertyInfo _propertyInfo;

		internal BuilderWithPropValidator(ExpressValidatorBuilder<TObj> expressValidatorBuilder, PropertyInfo propertyInfo)
		{
			ExpressValidatorBuilder = expressValidatorBuilder;
			_propertyInfo = propertyInfo;
		}

		public ExpressValidatorBuilder<TObj> WithValidation(Action<IRuleBuilderOptions<T, T>> action)
		{
			return WithValidationByRules(action, new ExpressPropertyValidator<T>(_propertyInfo, new TypeValidator<T>()));
		}

		public ExpressValidatorBuilder<TObj> WithAsyncValidation(Action<IRuleBuilderOptions<T, T>> action)
		{
			return WithValidationByRules(action, new ExpressPropertyValidator<T>(_propertyInfo, new TypeAsyncValidator<T>()));
		}

		private ExpressValidatorBuilder<TObj> WithValidationByRules(Action<IRuleBuilderOptions<T, T>> action, IExpressPropertyValidator<T> expressPropertyValidator)
		{
			var propertyValidator = expressPropertyValidator;
			propertyValidator.SetValidation(action);
			ExpressValidatorBuilder.AddValidator(propertyValidator);
			return ExpressValidatorBuilder;
		}

		private ExpressValidatorBuilder<TObj> ExpressValidatorBuilder { get; }
	}
}
