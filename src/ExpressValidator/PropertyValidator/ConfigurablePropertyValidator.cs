using FluentValidation;
using FluentValidation.Results;
using FluentValidation.Validators;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ExpressValidator
{
	public class ConfigurablePropertyValidator<T, TProperty, TOptions> : PropertyValidator<T, TProperty>
	{
		private readonly IExpressValidator<TProperty> _validator;
		private readonly Func<ValidationContext<T>, TProperty, ValidationResult, string> _templateFactory;
		private readonly List<AppendedArg<TOptions>> _appendedArgs;
		private readonly TOptions _options;
		private readonly bool _isTemplateFactorySet;

		internal ConfigurablePropertyValidator(
			Action<ExpressValidatorBuilder<TProperty, TOptions>> configure,
			TOptions options,
			Func<ValidationContext<T>, TProperty, ValidationResult, string> templateFactory,
			List<AppendedArg<TOptions>> appendedArgs)
		{
			var builder = new ExpressValidatorBuilder<TProperty, TOptions>();
			configure(builder);

			_validator = builder.Build(options);
			_options = options;

			_isTemplateFactorySet = !(templateFactory is null);

			_templateFactory = templateFactory ?? ((_, __, res) => string.Join(";", res.Errors.Select(e => e.ErrorMessage)));
			_appendedArgs = appendedArgs;
		}

		public override bool IsValid(ValidationContext<T> context, TProperty value)
		{
			var res = _validator.Validate(value);

			if (!res.IsValid)
			{
				if (_isTemplateFactorySet)
				{
					foreach (var apArg in _appendedArgs)
					{
						context.MessageFormatter.AppendArgument(apArg.ArgName, apArg.Selector(_options));
					}
				}
				_template = _templateFactory(context, value, res);
				return false;
			}
			return true;
		}

		private string _template;

		public override string Name => "DynamicPropertyValidator";

		protected override string GetDefaultMessageTemplate(string errorCode) => _template;
	}
}
