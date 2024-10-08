namespace ExpressValidator
{
	public interface IExpressValidatorBuilder<TObj, TOptions>
	{
		IExpressValidator<TObj> Build(TOptions options);
	}
}