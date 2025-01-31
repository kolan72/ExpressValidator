using NUnit.Framework;
using FluentValidation;
using ExpressValidator.Extensions;

namespace ExpressValidator.Tests
{
	internal class ExpressValidatorExtensionsTests
	{
		[Test]
		[TestCase(true)]
		[TestCase(false)]
		public void Should_BuildAndValidate_Work(bool isValid)
		{
			int i = isValid ? 1 : -1;
			var objToValidate = new ObjWithTwoPublicProps() { I = i };

			var result = new ExpressValidatorBuilder<ObjWithTwoPublicProps>()
			   .AddProperty((o) => o.I)
			   .WithValidation((opt) => opt.GreaterThan(0))
			   .BuildAndValidate(objToValidate);

			Assert.That(result.IsValid, Is.EqualTo(isValid));
		}
	}
}
