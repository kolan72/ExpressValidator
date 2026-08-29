namespace ExpressValidator
{
	internal interface IObjectValidator<in TObj> : IObjectValidatorBase<TObj>
	{
		void Initialize();
	}
}
