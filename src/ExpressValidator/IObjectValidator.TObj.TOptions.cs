namespace ExpressValidator
{
	internal interface IObjectValidator<TObj, in TOptions> : IObjectValidator<TObj>
	{
		void ApplyOptions(TOptions options);
	}
}