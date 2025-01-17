using FluentValidation;
using System;

namespace ExpressValidator
{
	/// <summary>
	/// Represents a contract for an object that holds <see cref="ExpressValidatorBuilder{TObj,TOptions}"/> along with the current property to be validated.
	/// This interface is primarily for internal use by ExpressValidator.
	/// </summary>
	/// <typeparam name="TObj">A type of object to validate.</typeparam>
	/// <typeparam name="TOptions">A type of the options to use when creating an object that implements the <see cref="IExpressValidator{TObj}"/>.</typeparam>
	/// <typeparam name="T">A type of the current property.</typeparam>
	public interface IBuilderWithPropValidator<TObj, TOptions, T>
	{
		/// <summary>
		/// Allows validators to be added in the IRuleBuilderOptions&lt;T, T&gt; property rule builder.
		/// </summary>
		/// <param name="action">Action to add validators</param>
		/// <returns>&lt;ExpressValidatorBuilder&lt;TObj&gt;&gt;</returns>
		ExpressValidatorBuilder<TObj, TOptions> WithValidation(Action<TOptions, IRuleBuilderOptions<T, T>> action);

		/// <summary>
		/// Allows validators to be added to the IRuleBuilderOptions&lt;T, T&gt; property rule builder with possible use with async validators.
		/// </summary>
		/// <param name="action">Action to add validators</param>
		/// <returns>&lt;ExpressValidatorBuilder&lt;TObj&gt;&gt;</returns>
		ExpressValidatorBuilder<TObj, TOptions> WithAsyncValidation(Action<TOptions, IRuleBuilderOptions<T, T>> action);
	}
}
