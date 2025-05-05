using ExpressValidator.QuickValidation;
using FluentValidation;
using NUnit.Framework;
using System;

namespace ExpressValidator.Tests
{
	internal class QuickValidatorTests
	{
		[Test]
		public void Should_Fail_WithExpectedPropertyName_When_ValidationFails_ForPrimitiveType_UsingOverload_WithPropertyName()
		{
			const int valueToTest = 5;
			var result = QuickValidator.Validate(valueToTest,
									(opt) => opt.GreaterThan(10)
												.GreaterThan(15),
									nameof(valueToTest));
			Assert.That(result.IsValid, Is.False);
			Assert.That(result.Errors.Count, Is.EqualTo(2));
			Assert.That(result.Errors[0].PropertyName, Is.EqualTo(nameof(valueToTest)));
		}

		[Test]
		[TestCase(PropertyNameMode.Default)]
		[TestCase(PropertyNameMode.TypeName)]
		public void Should_Fail_WithOverriddenPropertyName_When_ValidationFails_ForPrimitiveType_UsingOverload_WithPropertyNameMode(PropertyNameMode mode)
		{
			const int valueToTest = 5;
			const string propName = "MyPropName";
			var result = QuickValidator.Validate(valueToTest,
									(opt) => opt
												.OverridePropertyName(propName)
												.GreaterThan(10)
												.GreaterThan(15),
									mode);
			Assert.That(result.IsValid, Is.False);
			Assert.That(result.Errors.Count, Is.EqualTo(2));
			Assert.That(result.Errors[0].PropertyName, Is.EqualTo(propName));
			Assert.That(result.Errors[1].PropertyName, Is.EqualTo(propName));
		}

		[Test]
		[TestCase(PropertyNameMode.Default)]
		[TestCase(PropertyNameMode.TypeName)]
		public void Should_Fail_WithExpectedPropertyName_When_ValidationFails_ForPrimitiveType_UsingOverload_WithPropertyNameMode(PropertyNameMode mode)
		{
			const int valueToTest = 5;
			var result = QuickValidator.Validate(valueToTest,
									(opt) => opt.GreaterThan(10)
												.GreaterThan(15),
									mode);
			Assert.That(result.IsValid, Is.False);
			Assert.That(result.Errors.Count, Is.EqualTo(2));
			if (mode == PropertyNameMode.Default)
			{
				Assert.That(result.Errors[0].PropertyName, Is.EqualTo("Input"));
			}
			else
			{
				Assert.That(result.Errors[0].PropertyName, Is.EqualTo(typeof(int).Name));
			}
		}

		[Test]
		public void Should_Fail_WithExpectedPropertyName_When_ValidationFails_ForNonPrimitiveType_UsingOverload_WithPropertyName()
		{
			var objToQuick = new ObjWithTwoPublicProps() { I = -1, PercentValue1 = 101 };
			var rule = GetRule();

			var result = QuickValidator.Validate(objToQuick,
														rule,
														nameof(objToQuick));

			Assert.That(result.IsValid, Is.False);
			Assert.That(result.Errors.Count, Is.EqualTo(2));
			Assert.That(result.Errors[0].PropertyName, Is.EqualTo(nameof(objToQuick)+"." + nameof(ObjWithTwoPublicProps.I)));
		}

		[Test]
		[TestCase(PropertyNameMode.Default)]
		[TestCase(PropertyNameMode.TypeName)]
		public void Should_Fail_WithExpectedPropertyName_When_ValidationFails_ForNonPrimitiveType_UsingOverload_WithPropertyNameMode(PropertyNameMode mode)
		{
			var objToQuick = new ObjWithTwoPublicProps() { I = -1, PercentValue1 = 101 };
			var rule = GetRule();

			var result = QuickValidator.Validate(objToQuick,
														rule,
														mode);

			Assert.That(result.IsValid, Is.False);
			Assert.That(result.Errors.Count, Is.EqualTo(2));
			if (mode == PropertyNameMode.Default)
			{
				Assert.That(result.Errors[0].PropertyName, Is.EqualTo("Input." + nameof(ObjWithTwoPublicProps.I)));
			}
			else
			{
				Assert.That(result.Errors[0].PropertyName, Is.EqualTo(nameof(ObjWithTwoPublicProps) + "." + nameof(ObjWithTwoPublicProps.I)));
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

		private static Action<IRuleBuilderOptions<ObjWithTwoPublicProps, ObjWithTwoPublicProps>> GetRule()
		{
			return (opt) =>
							opt
							.ChildRules((v) => v.RuleFor(o => o.I)
								.GreaterThan(0))
							.ChildRules((v) => v.RuleFor(o => o.PercentValue1)
								.InclusiveBetween(0, 100));
		}
	}
}
