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
		private object _configurationSentinel;

		protected ValidatorConfigurator(ExpressValidatorOptions expressValidatorOptions = null)
		{
			expressValidatorOptions = expressValidatorOptions ?? new ExpressValidatorOptions() { OnFirstPropertyValidatorFailed = OnFirstPropertyValidatorFailed.Continue };
			_validatorBuilder = new ExpressValidatorBuilder<T, TOptions>(expressValidatorOptions.OnFirstPropertyValidatorFailed);
		}

		public abstract string ConfigSectionPath { get; }

		IExpressValidator<T> IValidatorConfigurator<T, TOptions>.Build(TOptions options)
		{
			// Ensure Configure is called only once using LazyInitializer
			System.Threading.LazyInitializer.EnsureInitialized(
				ref _configurationSentinel,
				() =>
				{
					Configure(_validatorBuilder);
					return new object();
				});

			return _validatorBuilder.Build(options);
		}
	}
}
