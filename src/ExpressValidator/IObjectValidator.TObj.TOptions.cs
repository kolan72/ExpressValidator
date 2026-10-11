namespace ExpressValidator
{
	internal interface IObjectValidator<in TObj, in TOptions> : IObjectValidatorBase<TObj>
	{
		void ApplyOptions(TOptions options);
	}
}