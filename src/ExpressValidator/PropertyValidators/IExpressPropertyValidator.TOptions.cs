using FluentValidation;
using System;

namespace ExpressValidator
{
	internal interface IExpressPropertyValidator<in TObj, TOptions, T> : IObjectValidator<TObj, TOptions>
	{
		void SetValidation(Action<TOptions, IRuleBuilderOptions<T, T>> action);
	}
}
