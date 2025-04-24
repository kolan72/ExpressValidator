using FluentValidation;
using FluentValidation.Results;
using NUnit.Framework;
using System.Linq;

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
		[TestCase(true)]
		[TestCase(false)]
		public void Should_FailValidation_WhenInputIsInvalid_HasCorrectPropertyName(bool withPropertyName)
		{
			const int valueToTest = 5;
			ValidationResult result = null;
			if (withPropertyName)
			{
				result = QuickValidator.Validate(valueToTest,
									(opt) => opt.GreaterThan(10),
									nameof(valueToTest));
			}
			else
			{
				result = QuickValidator.Validate(valueToTest,
								(opt) => opt.GreaterThan(10));
			}

			Assert.That(result.IsValid, Is.False);
			Assert.That(result.Errors.FirstOrDefault()?.PropertyName, Is.EqualTo(withPropertyName ? nameof(valueToTest) : "input value"));
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
