namespace ExpressValidator
{
	public interface IExpressValidatorBuilder<TObj>
	{
		IExpressValidator<TObj> Build();
	}
}