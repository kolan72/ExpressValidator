using FluentValidation;
using NUnit.Framework;

namespace ExpressValidator.Tests
{
	public class NotNullValidationMessageProviderTests
	{
		[Test]
		public void Should_GetMessage_Returns_CorrectMessage_For_Null_Instance()
		{
			const string propName = "TestPropName";
			var notNullMsgProvider = new NotNullValidationMessageProvider<string>(propName);
			var res = notNullMsgProvider.GetMessage(new ValidationContext<string>(null));
			Assert.That(res.Contains(propName), Is.True);
		}
	}
}
