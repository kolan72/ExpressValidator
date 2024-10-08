using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ExpressValidator
{
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
			var memInfo = MemberInfoParser.ParseProperty(func);
			return new BuilderWithPropValidator<TObj, TOptions, T>(this, memInfo);
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
		/// Add Func for object to get value to validate.
		/// </summary>
		/// <typeparam name="T">A type of value.</typeparam>
		/// <param name="func">Func for object.</param>
		/// <param name="propName">A name of the property if the validation failed.</param>
		/// <returns></returns>
		public IBuilderWithPropValidator<TObj, TOptions, T> AddFunc<T>(Func<TObj, T> func, string propName)
		{
			return new BuilderWithPropValidator<TObj, TOptions, T>(this, func, propName);
		}

		internal TOptions Options { get; set; }

		/// <summary>
		/// Builds the <see cref="IExpressValidator{TObj}"/>.
		/// </summary>
		/// <returns></returns>
		public IExpressValidator<TObj> Build(TOptions options)
		{
			return new ExpressValidator<TObj, TOptions>(options, _objectValidators, _validationMode);
		}

		internal void AddValidator(IObjectValidator<TObj, TOptions> objectValidator)
		{
			_objectValidators.Add(objectValidator);
		}
	}
}
