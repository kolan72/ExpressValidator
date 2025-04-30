using ExpressValidator.QuickValidation;
using FluentValidation;
using FluentValidation.Results;
using NUnit.Framework;
using System;

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
			Assert.That(result.Errors[0].PropertyName, Is.EqualTo(withPropertyName ? nameof(valueToTest) : "input"));
		}

		[Test]
		[TestCase(true)]
		[TestCase(false)]
		public void Should_FailValidation_WhenNonPrimitiveInputIsInvalid_HasCorrectPropertyName(bool withPropertyName)
		{
			var objToQuick = new ObjWithTwoPublicProps() { I = -1, PercentValue1 = 101 };
			var rule = GetRule();
			ValidationResult result = null;
			if (withPropertyName)
			{
				result = QuickValidator.Validate(objToQuick,
														rule,
														nameof(objToQuick));
			}
			else
			{
				result = QuickValidator.Validate(objToQuick,
													rule);
			}
			Assert.That(result.IsValid, Is.False);
			Assert.That(result.Errors[0].PropertyName, Is.EqualTo(withPropertyName ? nameof(objToQuick) + "." + nameof(objToQuick.I)
																					: "input." + nameof(objToQuick.I)));

			Assert.That(result.Errors[1].PropertyName, Is.EqualTo(withPropertyName ? nameof(objToQuick) + "." + nameof(objToQuick.PercentValue1)
																		: "input." + nameof(objToQuick.PercentValue1)));

			static Action<IRuleBuilderOptions<ObjWithTwoPublicProps, ObjWithTwoPublicProps>> GetRule()
			{
				return (opt) =>
																			opt
																			.ChildRules((v) => v.RuleFor(o => o.I)
																				.GreaterThan(0))
																			.ChildRules((v) => v.RuleFor(o => o.PercentValue1)
																				.InclusiveBetween(0, 100));
			}
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
