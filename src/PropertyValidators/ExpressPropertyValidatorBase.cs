using System.Reflection;

namespace ExpressValidator
{
	internal abstract class ExpressPropertyValidatorBase<T>
	{
		protected readonly string _propName;
		protected TypeValidatorBase<T> _typeValidator;

		protected ExpressPropertyValidatorBase(PropertyInfo propertyInfo)
		{
			PropInfo = propertyInfo;
			_propName = propertyInfo?.Name ?? string.Empty;
		}

		protected T GetPropertyValue<TObj>(TObj obj)
		{
			return PropInfo.GetTypedValue<TObj, T>(obj);
		}

		protected PropertyInfo PropInfo { get; set; }
	}
}
