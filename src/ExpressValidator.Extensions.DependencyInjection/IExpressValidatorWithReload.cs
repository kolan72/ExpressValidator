namespace ExpressValidator.Extensions.DependencyInjection
{
	/// <summary>
	/// Interface for dynamically rebuild <see cref="IExpressValidatorBuilder{TObj, TOptions}"/> to get new validator whenever options are changed.
	/// </summary>
	/// <typeparam name="TObj"></typeparam>
	public interface IExpressValidatorWithReload<TObj> : IExpressValidator<TObj> { }
}
