using System.Reflection;

namespace ExpressValidator
{
	internal class ExpressAsyncPropertyValidator<T> : ExpressPropertyValidatorBase<T>
	{
		public ExpressAsyncPropertyValidator(PropertyInfo propertyInfo) : base(propertyInfo)
		{
			_typeValidator = new TypeAsyncValidator<T>();
		}
	}
}
