using ExpressValidator.Extensions.DependencyInjection;
using ExpressValidator;
using FluentValidation;

namespace ConfiguratorDemo
{
	public class GuessValidatorConfigurator : ValidatorConfigurator<ObjToValidate>
	{
		public override void Configure(ExpressValidatorBuilder<ObjToValidate> expressValidatorBuilder)
			=> expressValidatorBuilder
				.AddProperty(o => o.I)
				.WithValidation((o) => o.GreaterThan(5));
	}
}
