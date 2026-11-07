using FluentValidation.Results;
using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace ExpressValidator
{
	/// <summary>
	/// Represents a set of rules for configuring and validating objects of type <typeparamref name="TObj"/>.
	/// Returned by <see cref="ExpressValidator.For{TObj}()"/>, it exposes methods to add members to validate
	/// along with validation operations, without exposing the underlying builder's <c>Build</c> operation.
	/// </summary>
	/// <typeparam name="TObj">A type of object to validate.</typeparam>
	public interface IExpressValidatorRules<TObj>
	{
		/// <summary>
		/// Adds property to validate.
		/// </summary>
		/// <typeparam name="T">A type of <typeparamref name="TObj"/> object property.</typeparam>
		/// <param name="func">An expression to get property.</param>
		/// <returns>The <see cref="IExpressValidatorRuleStep{TObj, T}"/> to continue the configuration.</returns>
		IExpressValidatorRuleStep<TObj, T> AddProperty<T>(Expression<Func<TObj, T>> func);

		/// <summary>
		/// Add field to validate.
		/// </summary>
		/// <typeparam name="T">A type of <typeparamref name="TObj"/> object field.</typeparam>
		/// <param name="func">An expression to get field.</param>
		/// <returns>The <see cref="IExpressValidatorRuleStep{TObj, T}"/> to continue the configuration.</returns>
		IExpressValidatorRuleStep<TObj, T> AddField<T>(Expression<Func<TObj, T>> func);

		/// <summary>
		/// Adds a property or field to validate.
		/// Use this when the member kind (property vs field) is not known at the call site,
		/// or to simplify call sites that previously had to choose between <see cref="AddProperty{T}"/> and <see cref="AddField{T}"/>.
		/// Throws <see cref="ArgumentException"/> if the expression does not resolve to a property or field.
		/// </summary>
		/// <typeparam name="T">A type of <typeparamref name="TObj"/> object property or field.</typeparam>
		/// <param name="func">An expression to get the property or field.</param>
		/// <returns>The <see cref="IExpressValidatorRuleStep{TObj, T}"/> to continue the configuration.</returns>
		IExpressValidatorRuleStep<TObj, T> AddMember<T>(Expression<Func<TObj, T>> func);

		/// <summary>
		/// Add Func for object to get value to validate.
		/// </summary>
		/// <typeparam name="T">A type of value.</typeparam>
		/// <param name="func">Func for object.</param>
		/// <param name="propName">A name of the property if the validation failed.</param>
		/// <param name="onSuccessValidation">Specifies a method to execute when validation succeeds.</param>
		/// <returns>The <see cref="IExpressValidatorRuleStep{TObj, T}"/> to continue the configuration.</returns>
		IExpressValidatorRuleStep<TObj, T> AddFunc<T>(Func<TObj, T> func, string propName, Action<T> onSuccessValidation = null);

		/// <summary>
		/// Builds a validator and validates the given object instance.
		/// </summary>
		/// <param name="obj">An object instance to validate.</param>
		/// <returns>A ValidationResult object containing any validation failures.</returns>
		ValidationResult Validate(TObj obj);

		/// <summary>
		/// Builds a validator and validates the given object instance asynchronously.
		/// </summary>
		/// <param name="obj">An object instance to validate.</param>
		/// <param name="token">A cancellation token to cancel validation.</param>
		/// <returns>A ValidationResult object containing any validation failures.</returns>
		Task<ValidationResult> ValidateAsync(TObj obj, CancellationToken token = default);
	}
}
