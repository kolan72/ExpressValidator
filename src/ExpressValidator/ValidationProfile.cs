namespace ExpressValidator
{
	public abstract class ValidationProfile<T>
	{
		protected readonly ExpressValidatorBuilder<T> _validatorBuilder;

		protected ValidationProfile(OnFirstPropertyValidatorFailed onFirstPropertyValidatorFailed = OnFirstPropertyValidatorFailed.Continue)
		{
			_validatorBuilder = new ExpressValidatorBuilder<T>(onFirstPropertyValidatorFailed);
		}

		public IExpressValidator<T> CreateValidator()
		{
			Configure(_validatorBuilder);
			return _validatorBuilder.Build();
		}

		public abstract void Configure(ExpressValidatorBuilder<T> expressValidatorBuilder);
	}
}
