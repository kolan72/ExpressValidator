using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace ExpressValidator
{
	internal class ExpressAsyncPropertyValidator<T> : ExpressPropertyValidatorBase<T>, IExpressPropertyValidator<T>
	{
		public ExpressAsyncPropertyValidator(PropertyInfo propertyInfo) : base(propertyInfo)
		{
			_typeValidator = new TypeAsyncValidator<T>();
		}

		public bool IsAsync => true;

		public (bool IsValid, List<ValidationFailure> Failures) Validate<TObj>(TObj obj)
		{
			throw new InvalidOperationException();
		}
	}
}
