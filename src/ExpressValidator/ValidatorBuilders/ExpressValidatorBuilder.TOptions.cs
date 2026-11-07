using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace ExpressValidator
{
	/// <summary>
	/// Represents the class that creates an object that implements <see cref="IExpressValidatorBuilder{TObj}"/>by using the collection of <typeparamref name="TObj"/> properties to validate and <typeparamref name="TOptions"/>, along with validation rules.
	/// </summary>
	/// <typeparam name="TObj">A type of object to validate.</typeparam>
	/// <typeparam name="TOptions">A type of the options to use when creating an object that implements the <see cref="IExpressValidator{TObj}"/>.</typeparam>
	public class ExpressValidatorBuilder<TObj, TOptions> : IExpressValidatorBuilder<TObj, TOptions>, IExpressValidatorRules<TObj, TOptions>
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
			=> AddPropertyCore<T>(func);

		// <summary>
		// Add field to validate.
		// </summary>
		// <typeparam name="T">A type of <typeparamref name="TObj"/> object field.</typeparam>
		// <param name="func">An expression to get field.</param>
		// <returns></returns>
		public IBuilderWithPropValidator<TObj, TOptions, T> AddField<T>(Expression<Func<TObj, T>> func)
			=> AddFieldCore<T>(func);

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
			=> AddMemberCore<T>(func);

		/// <summary>
		/// Add Func for object to get value to validate.
		/// </summary>
		/// <typeparam name="T">A type of value.</typeparam>
		/// <param name="func">Func for object.</param>
		/// <param name="propName">A name of the property if the validation failed.</param>
		/// <param name="onSuccessValidation">Specifies a method to execute when validation succeeds.</param>
		/// <returns></returns>
		public IBuilderWithPropValidator<TObj, TOptions, T> AddFunc<T>(Func<TObj, T> func, string propName, Action<T> onSuccessValidation = null)
			=> AddFuncCore(func, propName, onSuccessValidation);

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

		private BuilderWithPropValidator<TObj, TOptions, T> AddPropertyCore<T>(Expression<Func<TObj, T>> func)
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

		private BuilderWithPropValidator<TObj, TOptions, T> AddFieldCore<T>(Expression<Func<TObj, T>> func)
		{
			var memInfo = MemberInfoParser.ParseField(func);
			return new BuilderWithPropValidator<TObj, TOptions, T>(this, memInfo);
		}

		private BuilderWithPropValidator<TObj, TOptions, T> AddMemberCore<T>(Expression<Func<TObj, T>> func)
		{
			if (MemberInfoParser.TryParse(func, out MemberInfo memInfo))
			{
				return new BuilderWithPropValidator<TObj, TOptions, T>(this, memInfo);
			}
			else if (MemberInfoParser.TryParseMethodCallExpression(func, out ParameterInfo[] parameters))
			{
				return new BuilderWithPropValidator<TObj, TOptions, T>(this, func.Compile(), ValidatorBuilderHelpers.GetIndexerPropName(parameters));
			}
			else
			{
				throw new ArgumentException("Expression must refer to a property or field.");
			}
		}

		private BuilderWithPropValidator<TObj, TOptions, T> AddFuncCore<T>(Func<TObj, T> func, string propName, Action<T> onSuccessValidation)
		{
			return new BuilderWithPropValidator<TObj, TOptions, T>(this, func, propName, onSuccessValidation);
		}

		IExpressValidatorRuleStep<TObj, TOptions, T> IExpressValidatorRules<TObj, TOptions>.AddProperty<T>(Expression<Func<TObj, T>> func)
			=> AddPropertyCore<T>(func);

		IExpressValidatorRuleStep<TObj, TOptions, T> IExpressValidatorRules<TObj, TOptions>.AddField<T>(Expression<Func<TObj, T>> func)
			=> AddFieldCore<T>(func);

		IExpressValidatorRuleStep<TObj, TOptions, T> IExpressValidatorRules<TObj, TOptions>.AddMember<T>(Expression<Func<TObj, T>> func)
			=> AddMemberCore<T>(func);

		IExpressValidatorRuleStep<TObj, TOptions, T> IExpressValidatorRules<TObj, TOptions>.AddFunc<T>(Func<TObj, T> func, string propName, Action<T> onSuccessValidation)
			=> AddFuncCore(func, propName, onSuccessValidation);

		ValidationResult IExpressValidatorRules<TObj, TOptions>.Validate(TObj obj, TOptions options)
			=> Build(options).Validate(obj);

		Task<ValidationResult> IExpressValidatorRules<TObj, TOptions>.ValidateAsync(TObj obj, TOptions options, CancellationToken token)
			=> Build(options).ValidateAsync(obj, token);

		internal void AddValidator(IObjectValidator<TObj, TOptions> objectValidator)
		{
			_objectValidators.Add(objectValidator);
		}
	}
}
