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
	/// Represents the class that creates an object that implements <see cref="IExpressValidatorBuilder{TObj}"/>
	/// by using the collection of <typeparamref name="TObj"/> properties to validate,
	/// along with validation rules.
	/// </summary>
	/// <typeparam name="TObj">A type of object to validate.</typeparam>
	public class ExpressValidatorBuilder<TObj> : IExpressValidatorBuilder<TObj>, IExpressValidatorFacade<TObj>
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
			=> AddPropertyCore<T>(func);

		/// <summary>
		/// Add field to validate.
		/// </summary>
		/// <typeparam name="T">A type of <typeparamref name="TObj"/> object field.</typeparam>
		/// <param name="func">An expression to get field.</param>
		/// <returns></returns>
		public IBuilderWithPropValidator<TObj, T> AddField<T>(Expression<Func<TObj, T>> func)
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
		public IBuilderWithPropValidator<TObj, T> AddMember<T>(Expression<Func<TObj, T>> func)
			=> AddMemberCore<T>(func);

		/// <summary>
		/// Add Func for object to get value to validate.
		/// </summary>
		/// <typeparam name="T">A type of value.</typeparam>
		/// <param name="func">Func for object</param>
		/// <param name="propName">A name of the property if the validation failed.</param>
		/// <param name="onSuccessValidation">Specifies a method to execute when validation succeeds.</param>
		/// <returns></returns>
		public IBuilderWithPropValidator<TObj, T> AddFunc<T>(Func<TObj, T> func, string propName, Action<T> onSuccessValidation = null)
			=> AddFuncCore(func, propName, onSuccessValidation);

		/// <summary>
		/// Builds the <see cref="IExpressValidator{TObj}"/>.
		/// </summary>
		/// <returns></returns>
		public IExpressValidator<TObj> Build()
		{
			foreach (var validator in _objectValidators)
			{
				validator.Initialize();
			}
			return new ExpressValidator<TObj>(_objectValidators, _validationMode);
		}

		private BuilderWithPropValidator<TObj, T> AddPropertyCore<T>(Expression<Func<TObj, T>> func)
		{
			if (MemberInfoParser.TryParse(func, MemberTypes.Property, out var prop))
			{
				return new BuilderWithPropValidator<TObj, T>(this, prop);
			}
			else if (MemberInfoParser.TryParseMethodCallExpression(func, out ParameterInfo[] parameters))
			{
				return new BuilderWithPropValidator<TObj, T>(this, func.Compile(), ValidatorBuilderHelpers.GetIndexerPropName(parameters));
			}
			else
			{
				throw new ArgumentException("Can not get property from expression.");
			}
		}

		private BuilderWithPropValidator<TObj, T> AddFieldCore<T>(Expression<Func<TObj, T>> func)
		{
			var memInfo = MemberInfoParser.ParseField(func);
			return new BuilderWithPropValidator<TObj, T>(this, memInfo);
		}

		private BuilderWithPropValidator<TObj, T> AddMemberCore<T>(Expression<Func<TObj, T>> func)
		{
			if (MemberInfoParser.TryParse(func, out MemberInfo memInfo))
			{
				return new BuilderWithPropValidator<TObj, T>(this, memInfo);
			}
			else if (MemberInfoParser.TryParseMethodCallExpression(func, out ParameterInfo[] parameters))
			{
				return new BuilderWithPropValidator<TObj, T>(this, func.Compile(), ValidatorBuilderHelpers.GetIndexerPropName(parameters));
			}
			else
			{
				throw new ArgumentException("Expression must refer to a property or field.");
			}
		}

		private BuilderWithPropValidator<TObj, T> AddFuncCore<T>(Func<TObj, T> func, string propName, Action<T> onSuccessValidation)
		{
			return new BuilderWithPropValidator<TObj, T>(this, func, propName, onSuccessValidation);
		}

		IFacadeBuilderWithPropValidator<TObj, T> IExpressValidatorFacade<TObj>.AddProperty<T>(Expression<Func<TObj, T>> func)
			=> AddPropertyCore<T>(func);

		IFacadeBuilderWithPropValidator<TObj, T> IExpressValidatorFacade<TObj>.AddField<T>(Expression<Func<TObj, T>> func)
			=> AddFieldCore<T>(func);

		IFacadeBuilderWithPropValidator<TObj, T> IExpressValidatorFacade<TObj>.AddMember<T>(Expression<Func<TObj, T>> func)
			=> AddMemberCore<T>(func);

		IFacadeBuilderWithPropValidator<TObj, T> IExpressValidatorFacade<TObj>.AddFunc<T>(Func<TObj, T> func, string propName, Action<T> onSuccessValidation)
			=> AddFuncCore(func, propName, onSuccessValidation);

		ValidationResult IExpressValidatorFacade<TObj>.Validate(TObj obj)
			=> Build().Validate(obj);

		Task<ValidationResult> IExpressValidatorFacade<TObj>.ValidateAsync(TObj obj, CancellationToken token)
			=> Build().ValidateAsync(obj, token);

		internal void AddValidator(IObjectValidator<TObj> objectValidator)
		{
			_objectValidators.Add(objectValidator);
		}
	}
}
