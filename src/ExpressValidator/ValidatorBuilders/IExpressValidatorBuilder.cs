namespace ExpressValidator
{
	/// <summary>
	/// Represents the interface for a builder to create an instance of an object that implements <see cref="IExpressValidator{TOb}"/>.
	/// </summary>
	/// <typeparam name="TObj">A type of and object to validate.</typeparam>
	public interface IExpressValidatorBuilder<TObj>
	{
		/// <summary>
		/// Creates an instance of an object that implements the <see cref="IExpressValidator{TOb}"/>.
		/// </summary>
		/// <returns><see cref="IExpressValidator{TOb}"/>.</returns>
		IExpressValidator<TObj> Build();
	}
}