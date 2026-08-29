namespace ExpressValidator.Extensions.DependencyInjection
{
    public abstract class ValidatorConfigurator<T> : ValidationProfile<T>, IValidatorConfigurator<T>
	{
		private readonly ExpressValidatorBuilder<T> _validatorBuilder;
		protected ValidatorConfigurator(ExpressValidatorOptions expressValidatorOptions = null)
		{
			expressValidatorOptions = expressValidatorOptions ?? new ExpressValidatorOptions() { OnFirstPropertyValidatorFailed = OnFirstPropertyValidatorFailed.Continue };
			_validatorBuilder = new ExpressValidatorBuilder<T>(expressValidatorOptions.OnFirstPropertyValidatorFailed);
		}

		IExpressValidator<T> IValidatorConfigurator<T>.Build()
		{
			Configure(_validatorBuilder);
			return _validatorBuilder.Build();
		}
	}
}
