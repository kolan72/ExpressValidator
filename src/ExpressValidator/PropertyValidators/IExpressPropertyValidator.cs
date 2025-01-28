using FluentValidation;
using System;

namespace ExpressValidator
{
	internal interface IExpressPropertyValidator<in TObj, T> : IObjectValidator<TObj>
	{
		void SetValidation(Action<IRuleBuilderOptions<T, T>> action);
	}
}
