using FluentValidation;
using System;

namespace ExpressValidator
{
	internal interface IExpressPropertyValidator<TObj, TOptions, T> : IObjectValidator<TObj, TOptions>
	{
		void SetValidation(Action<TOptions, IRuleBuilderOptions<T, T>> action);
	}
}
