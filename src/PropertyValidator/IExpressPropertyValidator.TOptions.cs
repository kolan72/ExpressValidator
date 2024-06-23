using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressValidator
{
	internal interface IExpressPropertyValidator<TObj, TOptions, T> : IObjectValidator<TObj, TOptions>
	{
		void SetValidation(Action<TOptions, IRuleBuilderOptions<T, T>> action);
	}
}
