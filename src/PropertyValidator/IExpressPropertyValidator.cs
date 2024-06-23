using FluentValidation;
using System;

namespace ExpressValidator
{
	internal interface IExpressPropertyValidator<TObj, T> : IObjectValidator<TObj>
	{
		void SetValidation(Action<IRuleBuilderOptions<T, T>> action);
	}
}
