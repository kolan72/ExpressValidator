using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace ExpressValidator
{
	/// <summary>
	/// Builder class that holds the collection of <typeparamref name="TObj"/> properties to be validated, along with validation rules.
	/// </summary>
	/// <typeparam name="TObj">A type of object to validate.</typeparam>
	public class ExpressValidatorBuilder<TObj>
	{
		private readonly OnFirstPropertyValidatorFailed _validationMode;
		private readonly List<IObjectValidator<TObj>> _objectValidators = new List<IObjectValidator<TObj>>();

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
		public IBuilderWithPropValidator<TObj, T> AddProperty<T>(Expression<Func<TObj, T>> func)
		{
			var memInfo = MemberInfoParser.ParseProperty(func);
			return new BuilderWithPropValidator<TObj, T>(this, memInfo);
		}

		/// <summary>
		/// Add field to validate.
		/// </summary>
		/// <typeparam name="T">A type of <typeparamref name="TObj"/> object field.</typeparam>
		/// <param name="func">An expression to get field.</param>
		/// <returns></returns>
		public IBuilderWithPropValidator<TObj, T> AddField<T>(Expression<Func<TObj, T>> func)
		{
			var memInfo = MemberInfoParser.ParseField(func);
			return new BuilderWithPropValidator<TObj, T>(this, memInfo);
		}

		/// <summary>
		/// Add Func for object to get value to validate.
		/// </summary>
		/// <typeparam name="T">A type of value.</typeparam>
		/// <param name="func">Func for object</param>
		/// <param name="propName">A name of the property if the validation failed.</param>
		/// <returns></returns>
		public IBuilderWithPropValidator<TObj, T> AddFunc<T>(Func<TObj, T> func, string propName)
		{
			return new BuilderWithPropValidator<TObj, T>(this, func, propName);
		}

		/// <summary>
		/// Builds the <see cref="IExpressValidator{TObj}"/>.
		/// </summary>
		/// <returns></returns>
		public IExpressValidator<TObj> Build()
		{
			return new ExpressValidator<TObj>(_objectValidators, _validationMode);
		}

		internal void AddValidator(IObjectValidator<TObj> objectValidator)
		{
			_objectValidators.Add(objectValidator);
		}
	}
}
