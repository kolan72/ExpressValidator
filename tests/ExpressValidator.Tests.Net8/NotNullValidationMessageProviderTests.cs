using FluentValidation;
using NUnit.Framework;
using System;

namespace ExpressValidator.Tests.Net8
{
	public class NotNullValidationMessageProviderTests
	{
#pragma warning disable S1133 // Deprecated code should be removed
		[Obsolete("This test is obsolete")]
#pragma warning restore S1133 // Deprecated code should be removed
		[Test]
		public void Should_GetMessage_Returns_CorrectMessage_For_Null_Instance()
		{
			const string propName = "TestPropName";
			var notNullMsgProvider = new NotNullValidationMessageProvider<string>(propName);
			var res = notNullMsgProvider.GetMessage(new ValidationContext<string>(null));
			Assert.That(res.Contains(propName), Is.True);
		}

		[Test]
		public void Should_NullFallbackMessageProvider_Returns_CorrectMessage_For_Null_Instance()
		{
			const string propName = "TestPropName";
			var res = NullFallbackMessageProvider.GetMessage(propName, new ValidationContext<string>(null));
			Assert.That(res.Contains(propName), Is.True);
		}
	}
}
