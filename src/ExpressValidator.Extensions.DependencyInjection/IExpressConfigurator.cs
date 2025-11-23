namespace ExpressValidator.Extensions.DependencyInjection
{
	internal interface IExpressConfigurator<T>
	{
		IExpressValidator<T> Build();
	}
}
