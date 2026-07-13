using FluentValidation;
using FluentValidation.Results;
using System;

namespace ExpressValidator
{
	public interface IPropertyValidatorBuilder<T, TProperty, TOptions>
	{
		ConfigurablePropertyValidator<T, TProperty, TOptions> Build(TOptions options);
		IConfigurablePropertyBuilder<T, TProperty, TOptions> Configure(Action<ExpressValidatorBuilder<TProperty, TOptions>> configure);
	}

	public interface IConfigurablePropertyBuilder<T, TProperty, TOptions> : IPropertyValidatorBuilder<T, TProperty, TOptions>
	{
		IWithMessageTemplate<T, TProperty, TOptions> WithMessageTemplate(Func<ValidationContext<T>, TProperty, ValidationResult, string> func);
	}

	public interface IWithMessageTemplate<T, TProperty, TOptions>
	{
		IConfiguredPropertyBuilder<T, TProperty, TOptions> WithTemplateArgument(string argName, Func<TOptions, object> f);
	}

	public interface IConfiguredPropertyBuilder<T, TProperty, TOptions> :
																	IPropertyValidatorBuilder<T, TProperty, TOptions>,
																	IWithMessageTemplate<T, TProperty, TOptions>
	{ }
}
