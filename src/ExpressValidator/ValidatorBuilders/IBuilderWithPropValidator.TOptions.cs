using FluentValidation;
using System;

namespace ExpressValidator
{
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
