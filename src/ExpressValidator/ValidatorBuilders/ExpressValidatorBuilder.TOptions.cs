using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace ExpressValidator
{
	/// <summary>
	/// Represents the class that creates an object that implements <see cref="IExpressValidatorBuilder{TObj}"/>by using the collection of <typeparamref name="TObj"/> properties to validate and <typeparamref name="TOptions"/>, along with validation rules.
	/// </summary>
	/// <typeparam name="TObj">A type of object to validate.</typeparam>
	/// <typeparam name="TOptions">A type of the options to use when creating an object that implements the <see cref="IExpressValidator{TObj}"/>.</typeparam>
	public class ExpressValidatorBuilder<TObj, TOptions> : IExpressValidatorBuilder<TObj, TOptions>
	{
		private readonly OnFirstPropertyValidatorFailed _validationMode;
		private readonly List<IObjectValidator<TObj, TOptions>> _objectValidators = new List<IObjectValidator<TObj, TOptions>>();

		public ExpressValidatorBuilder(OnFirstPropertyValidatorFailed validationMode = OnFirstPropertyValidatorFailed.Continue)
		{
			_validationMode = validationMode;
		}

		/// <summary>
		/// Adds property to validate.
		/// </summary>
		/// <typeparam name="T">A type of <typeparamref name="TObj"/> object property.</typeparam>
		/// <param name="func">An expression to get property.</param>
		/// <returns></returns>
		public IBuilderWithPropValidator<TObj, TOptions, T> AddProperty<T>(Expression<Func<TObj, T>> func)
		{
			if (MemberInfoParser.TryParse(func, MemberTypes.Property, out var prop))
			{
				return new BuilderWithPropValidator<TObj, TOptions, T>(this, prop);
			}
			else if (MemberInfoParser.TryParseMethodCallExpression(func, out ParameterInfo[] parameters))
			{
				return new BuilderWithPropValidator<TObj, TOptions, T>(this, func.Compile(), ValidatorBuilderHelpers.GetIndexerPropName(parameters));
			}
			else
			{
				throw new ArgumentException("Can not get property from expression.");
			}
		}

		// <summary>
		// Add field to validate.
		// </summary>
		// <typeparam name="T">A type of <typeparamref name="TObj"/> object field.</typeparam>
		// <param name="func">An expression to get field.</param>
		// <returns></returns>
		public IBuilderWithPropValidator<TObj, TOptions, T> AddField<T>(Expression<Func<TObj, T>> func)
		{
			var memInfo = MemberInfoParser.ParseField(func);
			return new BuilderWithPropValidator<TObj, TOptions, T>(this, memInfo);
		}

		/// <summary>
		/// Adds a property or field to validate.
		/// Use this when the member kind (property vs field) is not known at the call site,
		/// or to simplify call sites that previously had to choose between <see cref="AddProperty{T}"/> and <see cref="AddField{T}"/>.
		/// Throws <see cref="ArgumentException"/> if the expression does not resolve to a property or field.
		/// </summary>
		/// <typeparam name="T">A type of <typeparamref name="TObj"/> object property or field.</typeparam>
		/// <param name="func">An expression to get the property or field.</param>
		/// <returns></returns>
		public IBuilderWithPropValidator<TObj, TOptions, T> AddMember<T>(Expression<Func<TObj, T>> func)
		{
			var memInfo = MemberInfoParser.ParseMember(func);
			return new BuilderWithPropValidator<TObj, TOptions, T>(this, memInfo);
		}

		/// <summary>
		/// Add Func for object to get value to validate.
		/// </summary>
		/// <typeparam name="T">A type of value.</typeparam>
		/// <param name="func">Func for object.</param>
		/// <param name="propName">A name of the property if the validation failed.</param>
		/// <param name="onSuccessValidation">Specifies a method to execute when validation succeeds.</param>
		/// <returns></returns>
		public IBuilderWithPropValidator<TObj, TOptions, T> AddFunc<T>(Func<TObj, T> func, string propName, Action<T> onSuccessValidation = null)
		{
			return new BuilderWithPropValidator<TObj, TOptions, T>(this, func, propName, onSuccessValidation);
		}

		/// <summary>
		/// Builds the <see cref="IExpressValidator{TObj}"/>.
		/// </summary>
		/// <returns></returns>
		public IExpressValidator<TObj> Build(TOptions options)
		{
			foreach (var validator in _objectValidators)
			{
				validator.ApplyOptions(options);
			}

			return new ExpressValidator<TObj>(_objectValidators, _validationMode);
		}

		internal void AddValidator(IObjectValidator<TObj, TOptions> objectValidator)
		{
			_objectValidators.Add(objectValidator);
		}
	}
}
