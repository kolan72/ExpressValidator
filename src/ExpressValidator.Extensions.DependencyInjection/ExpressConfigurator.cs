namespace ExpressValidator.Extensions.DependencyInjection
{
    public abstract class ExpressConfigurator<T> : IExpressConfigurator<T>
	{
		private readonly ExpressValidatorBuilder<T> _validatorBuilder;
		protected ExpressConfigurator(ExpressValidatorOptions expressValidatorOptions = null)
		{
			expressValidatorOptions = expressValidatorOptions ?? new ExpressValidatorOptions() { OnFirstPropertyValidatorFailed = OnFirstPropertyValidatorFailed.Continue };
			_validatorBuilder = new ExpressValidatorBuilder<T>(expressValidatorOptions.OnFirstPropertyValidatorFailed);
		}

		public abstract void Configure(ExpressValidatorBuilder<T> expressValidatorBuilder);

		IExpressValidator<T> IExpressConfigurator<T>.Build()
		{
			Configure(_validatorBuilder);
			return _validatorBuilder.Build();
		}
	}
}
