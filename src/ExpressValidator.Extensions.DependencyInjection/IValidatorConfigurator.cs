namespace ExpressValidator.Extensions.DependencyInjection
{
	internal interface IValidatorConfigurator<T>
	{
		IExpressValidator<T> Build();
	}

	internal interface IValidatorConfigurator<T, TOptions>
	{
		IExpressValidator<T> Build(TOptions options);
		string ConfigSectionPath { get; }
	}

	public abstract class ValidatorConfigurator<T, TOptions> : ValidationProfile<T, TOptions>, IValidatorConfigurator<T, TOptions>
	{
		private readonly ExpressValidatorBuilder<T, TOptions> _validatorBuilder;
		protected ValidatorConfigurator(ExpressValidatorOptions expressValidatorOptions = null)
		{
			expressValidatorOptions = expressValidatorOptions ?? new ExpressValidatorOptions() { OnFirstPropertyValidatorFailed = OnFirstPropertyValidatorFailed.Continue };
			_validatorBuilder = new ExpressValidatorBuilder<T, TOptions>(expressValidatorOptions.OnFirstPropertyValidatorFailed);
		}

		public abstract string ConfigSectionPath { get; }

		IExpressValidator<T> IValidatorConfigurator<T, TOptions>.Build(TOptions options)
		{
			Configure(_validatorBuilder);
			return _validatorBuilder.Build(options);
		}
	}
}
