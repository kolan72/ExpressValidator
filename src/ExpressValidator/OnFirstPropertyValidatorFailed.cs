namespace ExpressValidator
{
	/// <summary>
	/// Specifies how a property validator collection will validate an object if the current validator fails.
	/// </summary>
	public enum OnFirstPropertyValidatorFailed
	{
		/// <summary>
		/// Execution continues to the next property validator.
		/// </summary>
		Continue,
		/// <summary>
		/// Validation will be stopped.
		/// </summary>
		Break
	}
}
