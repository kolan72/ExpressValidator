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
			return WithValidationByRules(action, GetExpressPropertyValidator);
		}

		public ExpressValidatorBuilder<TObj> WithAsyncValidation(Action<IRuleBuilderOptions<T, T>> action)
		{
			return WithValidationByRules(action, GetExpressAsyncPropertyValidator);
		}

		private ExpressValidatorBuilder<TObj> WithValidationByRules(Action<IRuleBuilderOptions<T, T>> action, Func<IExpressPropertyValidator<T>> propertyValidatorProvider)
		{
			PropertyValidator = propertyValidatorProvider();
			PropertyValidator.SetValidation(action);
			ExpressValidatorBuilder.AddValidator(PropertyValidator);
			return ExpressValidatorBuilder;
		}

		private IExpressPropertyValidator<T> GetExpressAsyncPropertyValidator()
		{
			return new ExpressAsyncPropertyValidator<T>(_propertyInfo);
		}

		private IExpressPropertyValidator<T> GetExpressPropertyValidator()
		{
			return new ExpressPropertyValidator<T>(_propertyInfo);
		}

		internal ExpressValidatorBuilder<TObj> ExpressValidatorBuilder { get; }
		internal IExpressPropertyValidator<T> PropertyValidator { get; private set; }
	}
}
