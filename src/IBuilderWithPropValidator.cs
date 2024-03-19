using FluentValidation;
using System;

namespace ExpressValidator
{
	/// <summary>
	/// Represents a contract for an object that holds <see cref="ExpressValidatorBuilder{TObj}"/> along with the current property to be validated.
	/// This interface is primarily for internal use by ExpressValidator.
	/// </summary>
	/// <typeparam name="TObj"></typeparam>
	/// <typeparam name="T"></typeparam>
	public interface IBuilderWithPropValidator<TObj, T>
	{
		/// <summary>
		/// Allows validators to be added in the IRuleBuilderOptions&lt;T, T&gt; property rule builder.
		/// </summary>
		/// <param name="action">Action to add validators</param>
		/// <returns>&lt;ExpressValidatorBuilder&lt;TObj&gt;&gt;</returns>
		ExpressValidatorBuilder<TObj> WithValidation(Action<IRuleBuilderOptions<T, T>> action);

		/// <summary>
		/// Allows validators to be added to the IRuleBuilderOptions&lt;T, T&gt; property rule builder with possible use with async validators.
		/// </summary>
		/// <param name="action">Action to add validators</param>
		/// <returns>&lt;ExpressValidatorBuilder&lt;TObj&gt;&gt;</returns>
		ExpressValidatorBuilder<TObj> WithAsyncValidation(Action<IRuleBuilderOptions<T, T>> action);
	}
}
