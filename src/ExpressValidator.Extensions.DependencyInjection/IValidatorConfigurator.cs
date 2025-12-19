namespace ExpressValidator.Extensions.DependencyInjection
{
	internal interface IValidatorConfigurator<T>
	{
		IExpressValidator<T> Build();
	}
}
