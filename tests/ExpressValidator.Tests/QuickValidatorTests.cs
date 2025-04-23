using FluentValidation;
using NUnit.Framework;

namespace ExpressValidator.Tests
{
	internal class QuickValidatorTests
	{
		[Test]
		public void Should_Fail_Validation_When_NotValid()
		{
			const int valueToTest = 5;
			var result = QuickValidator.Validate(valueToTest,
									(opt) => opt.GreaterThan(10)
												.GreaterThan(15));
			Assert.That(result.IsValid, Is.False);
			Assert.That(result.Errors.Count, Is.EqualTo(2));
		}

		[Test]
		public void Should_Pass_Validation_When_Valid()
		{
			const int valueToTest = 25;
			var result = QuickValidator.Validate(valueToTest,
									(opt) => opt.GreaterThan(10)
												.InclusiveBetween(15, 25));
			Assert.That(result.IsValid, Is.True);
		}
	}
}
