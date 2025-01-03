namespace ExpressValidator
{
	/// <summary>
	/// Represents the interface for a builder that uses <typeparamref name="TOptions"/> to create an instance of an object that implements <see cref="IExpressValidator{TOb}"/>.
	/// </summary>
	/// <typeparam name="TObj">A type of object to validate.</typeparam>
	/// <typeparam name="TOptions">A type of and options for use when creating an object that implements the <see cref="IExpressValidator{TObj}"/>.</typeparam>
	public interface IExpressValidatorBuilder<TObj, TOptions>
	{
		/// <summary>
		/// Creates an instance of an object that implements the <see cref="IExpressValidator{TOb}"/>.
		/// </summary>
		/// <param name="options">Options for use by <see cref="ExpressValidatorBuilder{T, TOptions}"/> to build a validator.</param>
		/// <returns><see cref="IExpressValidator{TOb}"/>.</returns>
		IExpressValidator<TObj> Build(TOptions options);
	}
}