namespace ExpressValidator.QuickValidation
{
    /// <summary>
    /// Determines how the property name is set when quick validation fails.
    /// </summary>
    public enum PropertyNameMode
    {
        /// <summary>
        /// Use the literal string "Input" as the property name.
        /// </summary>
        Default,

        /// <summary>
        /// Use the type's name (typeof(T).Name) of the validated object as the property name.
        /// </summary>
        TypeName
    }
}
