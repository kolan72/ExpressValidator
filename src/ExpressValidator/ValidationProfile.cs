namespace ExpressValidator
{
#pragma warning disable S1694 // An abstract class should have both abstract and concrete methods
	public abstract class ValidationProfile<T>
#pragma warning restore S1694 // An abstract class should have both abstract and concrete methods
	{
		public abstract void Configure(ExpressValidatorBuilder<T> expressValidatorBuilder);
	}
}
