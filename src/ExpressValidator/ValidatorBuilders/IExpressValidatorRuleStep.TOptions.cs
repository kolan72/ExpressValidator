using FluentValidation;
using System;

namespace ExpressValidator
{
	/// <summary>
	/// Represents a contract for an object that belongs to the <see cref="IExpressValidatorRules{TObj, TOptions}"/> fluent chain
	/// and holds the current property to be validated.
	/// This interface is primarily for internal use by ExpressValidator.
	/// </summary>
	/// <typeparam name="TObj">A type of object to validate.</typeparam>
	/// <typeparam name="TOptions">A type of the options to use when creating an object that implements the <see cref="IExpressValidator{TObj}"/>.</typeparam>
	/// <typeparam name="T">A type of the current property.</typeparam>
	public interface IExpressValidatorRuleStep<TObj, TOptions, T>
	{
		/// <summary>
		/// Allows validators to be added in the rule builder for the current property.
		/// </summary>
		/// <param name="action">Action to add validators</param>
		/// <returns>The <see cref="IExpressValidatorRules{TObj, TOptions}"/> to continue the configuration.</returns>
		IExpressValidatorRules<TObj, TOptions> WithValidation(Action<TOptions, IRuleBuilderOptions<T, T>> action);

		/// <summary>
		/// Allows validators to be added to the rule builder for the current property with possible use with async validators.
		/// </summary>
		/// <param name="action">Action to add validators</param>
		/// <returns>The <see cref="IExpressValidatorRules{TObj, TOptions}"/> to continue the configuration.</returns>
		IExpressValidatorRules<TObj, TOptions> WithAsyncValidation(Action<TOptions, IRuleBuilderOptions<T, T>> action);
	}
}
