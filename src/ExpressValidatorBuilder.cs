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
		private readonly List<IObjectValidator> _objectValidators = new List<IObjectValidator>();

		public ExpressValidatorBuilder(OnFirstPropertyValidatorFailed validationMode = OnFirstPropertyValidatorFailed.Continue)
		{
			_validationMode = validationMode;
		}

		/// <summary>
		/// Adds property to validate.
		/// </summary>
		/// <typeparam name="T">A type of property <typeparamref name="TObj"/>.</typeparam>
		/// <param name="func">An expression to get property.</param>
		/// <returns></returns>
		public IBuilderWithPropValidator<TObj, T> AddProperty<T>(Expression<Func<TObj, T>> func)
		{
			if (!MemberInfoParser.TryParse(func, MemberTypes.Property, out MemberInfo memInfo))
			{
				throw new ArgumentException("Can not get property from expression.");
			}
			return new BuilderWithPropValidator<TObj, T>(this, memInfo);
		}

		/// <summary>
		/// Add field to validate.
		/// </summary>
		/// <typeparam name="T">A type of field <typeparamref name="TObj"/>.</typeparam>
		/// <param name="func">An expression to get field.</param>
		/// <returns></returns>
		public IBuilderWithPropValidator<TObj, T> AddField<T>(Expression<Func<TObj, T>> func)
		{
			if (!MemberInfoParser.TryParse(func, MemberTypes.Field, out MemberInfo memInfo))
			{
				throw new ArgumentException("Can not get property from expression.");
			}
			return new BuilderWithPropValidator<TObj, T>(this, memInfo);
		}

		/// <summary>
		/// Builds the <see cref="IExpressValidator{TObj}"/>.
		/// </summary>
		/// <returns></returns>
		public IExpressValidator<TObj> Build()
		{
			return new ExpressValidator<TObj>(_objectValidators, _validationMode);
		}

		internal void AddValidator(IObjectValidator objectValidator)
		{
			_objectValidators.Add(objectValidator);
		}
	}
}
