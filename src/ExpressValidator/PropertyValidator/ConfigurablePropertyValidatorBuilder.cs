using FluentValidation;
using FluentValidation.Results;
using System;
using System.Collections.Generic;

namespace ExpressValidator
{
	public class ConfigurablePropertyValidatorBuilder<T, TProperty, TOptions> : IConfigurablePropertyBuilder<T, TProperty, TOptions>, IConfiguredPropertyBuilder<T, TProperty, TOptions>
	{
		private Action<ExpressValidatorBuilder<TProperty, TOptions>> _configure;
		private Func<ValidationContext<T>, TProperty, ValidationResult, string> _func;

		private readonly List<AppendedArg<TOptions>> _appendedArgs = new List<AppendedArg<TOptions>>();

		internal ConfigurablePropertyValidatorBuilder(){}

		internal ConfigurablePropertyValidatorBuilder(Action<ExpressValidatorBuilder<TProperty, TOptions>> configure)
		{
			_configure = configure;
		}

		public ConfigurablePropertyValidator<T, TProperty, TOptions> Build(TOptions options)
		{
			return new ConfigurablePropertyValidator<T, TProperty, TOptions>(_configure, options, _func, _appendedArgs);
		}

		public IWithMessageTemplate<T, TProperty, TOptions> WithMessageTemplate(Func<ValidationContext<T>, TProperty, ValidationResult, string> func)
		{
			_func = func;
			return this;
		}

		public IConfiguredPropertyBuilder<T, TProperty, TOptions> WithTemplateArgument(string argName, Func<TOptions, object> f)
		{
			_appendedArgs.Add(new AppendedArg<TOptions>() { ArgName = argName, Selector = f });
			return this;
		}

		public IConfigurablePropertyBuilder<T, TProperty, TOptions> Configure(Action<ExpressValidatorBuilder<TProperty, TOptions>> configure)
		{
			_configure = configure;
			return this;
		}
	}
}
