using FluentValidation;
using FluentValidation.Validators;

namespace ExpressValidator
{
	internal class NotNullValidationMessageProvider<T> : NotNullValidator<object, T>
	{
		private readonly string _propName;
		public NotNullValidationMessageProvider(string propName)
		{
			_propName = propName;
		}

		public string GetMessage(ValidationContext<T> context)
		{
			return context.MessageFormatter.AppendPropertyName(_propName).BuildMessage(GetDefaultMessageTemplate(null));
		}
	}
}
