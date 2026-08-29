namespace ExpressValidator
{
	/// <summary>
	/// Static entry point for creating ExpressValidator builders.
	/// Provides a fluent API: <c>ExpressValidator.For&lt;TObj&gt;().AddProperty(...).WithValidation(...).Validate(obj)</c>.
	/// </summary>
	public static class ExpressValidator
	{
		/// <summary>
		/// Creates an <see cref="ExpressValidatorBuilder{TObj}"/> for validating objects of type <typeparamref name="TObj"/>.
		/// </summary>
		/// <typeparam name="TObj">The type of object to validate.</typeparam>
		/// <returns>A new <see cref="ExpressValidatorBuilder{TObj}"/>.</returns>
		public static ExpressValidatorBuilder<TObj> For<TObj>()
			=> new ExpressValidatorBuilder<TObj>();

		/// <summary>
		/// Creates an <see cref="ExpressValidatorBuilder{TObj}"/> for validating objects of type <typeparamref name="TObj"/>
		/// with the specified validation mode.
		/// </summary>
		/// <typeparam name="TObj">The type of object to validate.</typeparam>
		/// <param name="validationMode">Specifies whether to stop or continue on the first property validation failure.</param>
		/// <returns>A new <see cref="ExpressValidatorBuilder{TObj}"/>.</returns>
		public static ExpressValidatorBuilder<TObj> For<TObj>(OnFirstPropertyValidatorFailed validationMode)
			=> new ExpressValidatorBuilder<TObj>(validationMode);

		/// <summary>
		/// Creates an <see cref="ExpressValidatorBuilder{TObj, TOptions}"/> for validating objects of type <typeparamref name="TObj"/>
		/// with configurable options of type <typeparamref name="TOptions"/>.
		/// </summary>
		/// <typeparam name="TObj">The type of object to validate.</typeparam>
		/// <typeparam name="TOptions">The type of options used for parameterized validation rules.</typeparam>
		/// <returns>A new <see cref="ExpressValidatorBuilder{TObj, TOptions}"/>.</returns>
		public static ExpressValidatorBuilder<TObj, TOptions> For<TObj, TOptions>()
			=> new ExpressValidatorBuilder<TObj, TOptions>();

		/// <summary>
		/// Creates an <see cref="ExpressValidatorBuilder{TObj, TOptions}"/> for validating objects of type <typeparamref name="TObj"/>
		/// with configurable options of type <typeparamref name="TOptions"/> and the specified validation mode.
		/// </summary>
		/// <typeparam name="TObj">The type of object to validate.</typeparam>
		/// <typeparam name="TOptions">The type of options used for parameterized validation rules.</typeparam>
		/// <param name="validationMode">Specifies whether to stop or continue on the first property validation failure.</param>
		/// <returns>A new <see cref="ExpressValidatorBuilder{TObj, TOptions}"/>.</returns>
		public static ExpressValidatorBuilder<TObj, TOptions> For<TObj, TOptions>(OnFirstPropertyValidatorFailed validationMode)
			=> new ExpressValidatorBuilder<TObj, TOptions>(validationMode);
	}
}
