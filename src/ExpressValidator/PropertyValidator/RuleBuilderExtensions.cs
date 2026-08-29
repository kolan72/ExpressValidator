using FluentValidation;
using System;

namespace ExpressValidator
{
	public static class RuleBuilderExtensions
	{
		public static IRuleBuilderOptions<T, TProperty> SetExpressValidator<T, TProperty, TOptions>(
			this IRuleBuilder<T, TProperty> validator,
			Action<IPropertyValidatorBuilder<T, TProperty, TOptions>> func,
			TOptions options)
		{
			var builder = new ConfigurablePropertyValidatorBuilder<T, TProperty, TOptions>();
			func(builder);
			var result = builder.Build(options);
			return validator.SetValidator(result);
		}
	}
}
