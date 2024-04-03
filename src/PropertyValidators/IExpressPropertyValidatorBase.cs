using FluentValidation;
using System;

namespace ExpressValidator
{
	internal interface IExpressPropertyValidatorBase<T>
	{
		void SetValidation(Action<IRuleBuilderOptions<T, T>> action);
	}
}
