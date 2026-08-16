using ExpressValidator;
using ExpressValidator.Extensions.DependencyInjection;
using FluentValidation;

namespace ConfiguratorWithOptionsDemo
{
	/// <summary>
	/// Validator configurator that uses options to configure validation rules dynamically.
	/// The options are automatically bound from the configuration section specified in ConfigSectionPath.
	/// When the configuration changes, the validator is automatically rebuilt.
	/// </summary>
	public class GuessValidatorConfigurator : ValidatorConfigurator<ObjToValidate, GuessValidationOptions>
	{
		// ConfigSectionPath property tells the DI system which configuration section to bind
		public override string ConfigSectionPath => "GuessValidation";

		public override void Configure(ExpressValidatorBuilder<ObjToValidate, GuessValidationOptions> expressValidatorBuilder)
		{
			// Use options to configure validation rules
			expressValidatorBuilder
				.AddProperty(o => o.I)
				.WithValidation((val, options) => options
					.GreaterThanOrEqualTo(val.MinValue)
					.WithMessage($"Value must be at least {val.MinValue}")
					.LessThanOrEqualTo(val.MaxValue)
					.WithMessage($"Value must be at most {val.MaxValue}"));
		}
	}

	/// <summary>
	/// Options class that will be bound from the "GuessValidation" configuration section.
	/// </summary>
	public class GuessValidationOptions
	{
		public int MinValue { get; set; }
		public int MaxValue { get; set; }
	}
}
