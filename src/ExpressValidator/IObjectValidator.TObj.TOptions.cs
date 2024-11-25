namespace ExpressValidator
{
	internal interface IObjectValidator<in TObj, in TOptions> : IObjectValidator<TObj>
	{
		void ApplyOptions(TOptions options);
	}
}