using FluentValidation;
using FluentValidation.Validators;
using System;

namespace ExpressValidator
{
	internal class NotNullValidationMessageProvider<T> : NotNullValidator<object, T>
	{
		private readonly string _propName;
		public NotNullValidationMessageProvider(string propName)
		{
			_propName = propName;
		}

		public string GetMessage() => GetDefaultMessageTemplate(null);

#pragma warning disable S1133 // Deprecated code should be removed
		[Obsolete("This method is obsolete")]
#pragma warning restore S1133 // Deprecated code should be removed
		public string GetMessage(ValidationContext<T> context)
		{
			return context.MessageFormatter.AppendPropertyName(_propName).BuildMessage(GetDefaultMessageTemplate(null));
		}
	}

	internal static class NullFallbackMessageProvider
	{
		public static string GetMessage<T>(string propName, ValidationContext<T> context)
		{
			var validator = new NotNullValidationMessageProvider<T>(propName);
			return context.MessageFormatter.AppendPropertyName(propName).BuildMessage(validator.GetMessage());
		}
	}
}
