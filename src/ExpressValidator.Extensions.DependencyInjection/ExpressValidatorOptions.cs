namespace ExpressValidator.Extensions.DependencyInjection
{
	/// <summary>
	/// Options to create <see cref="ExpressValidatorBuilder{T, TOptions}"/>.
	/// </summary>
	public class ExpressValidatorOptions
	{
		public OnFirstPropertyValidatorFailed OnFirstPropertyValidatorFailed { get; set; }
	}
}
