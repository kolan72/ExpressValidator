using FluentValidation.Results;
using System.Collections.Generic;
using System.Reflection;

namespace ExpressValidator
{
	internal class ExpressPropertyValidator<T> : ExpressPropertyValidatorBase<T>, IExpressPropertyValidator<T>
	{
		public ExpressPropertyValidator(PropertyInfo propertyInfo) : base(propertyInfo)
		{
			_typeValidator = new TypeValidator<T>();
		}

		public bool IsAsync => false;

		public (bool IsValid, List<ValidationFailure> Failures) Validate<TObj>(TObj obj)
		{
			return _typeValidator.ValidateEx(GetPropertyValue(obj));
		}
	}
}
