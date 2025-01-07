namespace ExpressValidator.Extensions.DependencyInjection
{
	/// <summary>
	/// Options to create <see cref="ExpressValidatorBuilder{T, TOptions}"/>.
	/// </summary>
	public class ExpressValidatorOptions
	{
		/// <summary>
		/// Specifies how an ExpressValidatorBuilder will validate an object if the current validator fails.
		/// </summary>
		public OnFirstPropertyValidatorFailed OnFirstPropertyValidatorFailed { get; set; }
	}
}
