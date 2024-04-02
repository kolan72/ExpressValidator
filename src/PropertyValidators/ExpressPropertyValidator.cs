using System.Reflection;

namespace ExpressValidator
{
	internal class ExpressPropertyValidator<T> : ExpressPropertyValidatorBase<T>
	{
		public ExpressPropertyValidator(PropertyInfo propertyInfo) : base(propertyInfo)
		{
			_typeValidator = new TypeValidator<T>();
		}
	}
}
