using FluentValidation;
using System;

namespace ExpressValidator
{
	public interface IExpressPropertyValidatorBase<T>
	{
		void SetValidation(Action<IRuleBuilderOptions<T, T>> action);
	}
}
