﻿using ExpressValidator.QuickValidation;
using FluentValidation;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

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
		public async Task Should_Fail_WithExpectedPropertyName_When_AsyncValidationFails_ForPrimitiveType_UsingOverload_WithPropertyName()
		{
			const int valueToTest = 5;
			var result = await QuickValidator.ValidateAsync(valueToTest,
									(opt) => opt.GreaterThan(10)
												.GreaterThan(15)
												.MustAsync(async (_, __) => { await Task.Delay(1); return true; }),
									nameof(valueToTest));
			Assert.That(result.IsValid, Is.False);
			Assert.That(result.Errors.Count, Is.EqualTo(2));
			Assert.That(result.Errors[0].PropertyName, Is.EqualTo(nameof(valueToTest)));
		}

		[Test]
		public void Should_Fail_WithOverriddenPropertyName_When_ValidationFails_ForPrimitiveType_UsingOverload_WithPropertyName()
		{
			const int valueToTest = 5;
			const string propName = "MyPropName";
			var result = QuickValidator.Validate(valueToTest,
									(opt) => opt
												.OverridePropertyName(propName)
												.GreaterThan(10)
												.GreaterThan(15),
									nameof(valueToTest));
			Assert.That(result.IsValid, Is.False);
			Assert.That(result.Errors.Count, Is.EqualTo(2));
			Assert.That(result.Errors[0].PropertyName, Is.EqualTo(propName));
		}

		[Test]
		public async Task Should_Fail_WithOverriddenPropertyName_When_AsyncValidationFails_ForPrimitiveType_UsingOverload_WithPropertyName()
		{
			const int valueToTest = 5;
			const string propName = "MyPropName";
			var result = await QuickValidator.ValidateAsync(valueToTest,
									(opt) => opt
												.OverridePropertyName(propName)
												.GreaterThan(10)
												.GreaterThan(15)
												.MustAsync(async (_, __) => { await Task.Delay(1); return true; }),
									nameof(valueToTest));
			Assert.That(result.IsValid, Is.False);
			Assert.That(result.Errors.Count, Is.EqualTo(2));
			Assert.That(result.Errors[0].PropertyName, Is.EqualTo(propName));
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
		public async Task Should_Fail_WithOverriddenPropertyName_When_AsyncValidationFails_ForPrimitiveType_UsingOverload_WithPropertyNameMode(PropertyNameMode mode)
		{
			const int valueToTest = 5;
			const string propName = "MyPropName";
			var result = await QuickValidator.ValidateAsync(valueToTest,
									(opt) => opt
												.OverridePropertyName(propName)
												.GreaterThan(10)
												.GreaterThan(15)
												.MustAsync(async (_, __) => { await Task.Delay(1); return true; })
												,
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
		[TestCase(PropertyNameMode.Default)]
		[TestCase(PropertyNameMode.TypeName)]
		public async Task Should_Fail_WithExpectedPropertyName_When_AsyncValidationFails_ForPrimitiveType_UsingOverload_WithPropertyNameMode(PropertyNameMode mode)
		{
			const int valueToTest = 5;
			var result = await QuickValidator.ValidateAsync(valueToTest,
									(opt) => opt.GreaterThan(10)
												.GreaterThan(15)
												.MustAsync(async (_, __) => { await Task.Delay(1); return true; })
												,
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
			Assert.That(result.Errors[0].PropertyName, Is.EqualTo(nameof(objToQuick) + "." + nameof(ObjWithTwoPublicProps.I)));
		}

		[Test]
		public async Task Should_Fail_WithExpectedPropertyName_When_AsyncValidationFails_ForNonPrimitiveType_UsingOverload_WithPropertyName()
		{
			var objToQuick = new ObjWithTwoPublicProps() { I = -1, PercentValue1 = 101 };
			var rule = GetAsyncRule();

			var result = await QuickValidator.ValidateAsync(objToQuick,
														rule,
														nameof(objToQuick));

			Assert.That(result.IsValid, Is.False);
			Assert.That(result.Errors.Count, Is.EqualTo(2));
			Assert.That(result.Errors[0].PropertyName, Is.EqualTo(nameof(objToQuick) + "." + nameof(ObjWithTwoPublicProps.I)));
		}

		[Test]
		public void Should_Fail_WithOverriddenPropertyName_When_ValidationFails_ForNonPrimitiveType_UsingOverload_WithPropertyName()
		{
			var objToQuick = new ObjWithTwoPublicProps() { I = -1, PercentValue1 = 101 };
			var rule = GetRuleWithOverriddenPropertyName();

			var result = QuickValidator.Validate(objToQuick,
														rule,
														nameof(objToQuick));

			Assert.That(result.IsValid, Is.False);
			Assert.That(result.Errors.Count, Is.EqualTo(2));
			Assert.That(result.Errors[0].PropertyName, Is.EqualTo(nameof(objToQuick) + ".MyPropNameI"));
		}

		[Test]
		public async Task Should_Fail_WithOverriddenPropertyName_When_AsyncValidationFails_ForNonPrimitiveType_UsingOverload_WithPropertyName()
		{
			var objToQuick = new ObjWithTwoPublicProps() { I = -1, PercentValue1 = 101 };
			var rule = GetAsyncRuleWithOverriddenPropertyName();

			var result = await QuickValidator.ValidateAsync(objToQuick,
														rule,
														nameof(objToQuick));

			Assert.That(result.IsValid, Is.False);
			Assert.That(result.Errors.Count, Is.EqualTo(2));
			Assert.That(result.Errors[0].PropertyName, Is.EqualTo(nameof(objToQuick) + ".MyPropNameI"));
		}

		[Test]
		[TestCase(PropertyNameMode.Default)]
		[TestCase(PropertyNameMode.TypeName)]
		public void Should_Fail_WithOverriddenPropertyName_When_ValidationFails_ForNonPrimitiveType_UsingOverload_WithPropertyNameMode(PropertyNameMode mode)
		{
			var objToQuick = new ObjWithTwoPublicProps() { I = -1, PercentValue1 = 101 };
			var rule = GetRuleWithOverriddenPropertyName();

			var result = QuickValidator.Validate(objToQuick,
														rule,
														mode);

			Assert.That(result.IsValid, Is.False);
			Assert.That(result.Errors.Count, Is.EqualTo(2));
			if (mode == PropertyNameMode.Default)
			{
				Assert.That(result.Errors[0].PropertyName, Is.EqualTo("Input.MyPropNameI"));
			}
			else
			{
				Assert.That(result.Errors[0].PropertyName, Is.EqualTo(nameof(ObjWithTwoPublicProps) + ".MyPropNameI"));
			}
		}

		[Test]
		[TestCase(PropertyNameMode.Default)]
		[TestCase(PropertyNameMode.TypeName)]
		public async Task Should_Fail_WithOverriddenPropertyName_When_AsyncValidationFails_ForNonPrimitiveType_UsingOverload_WithPropertyNameMode(PropertyNameMode mode)
		{
			var objToQuick = new ObjWithTwoPublicProps() { I = -1, PercentValue1 = 101 };
			var rule = GetAsyncRuleWithOverriddenPropertyName();

			var result = await QuickValidator.ValidateAsync(objToQuick,
														rule,
														mode);

			Assert.That(result.IsValid, Is.False);
			Assert.That(result.Errors.Count, Is.EqualTo(2));
			if (mode == PropertyNameMode.Default)
			{
				Assert.That(result.Errors[0].PropertyName, Is.EqualTo("Input.MyPropNameI"));
			}
			else
			{
				Assert.That(result.Errors[0].PropertyName, Is.EqualTo(nameof(ObjWithTwoPublicProps) + ".MyPropNameI"));
			}
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
		[TestCase(PropertyNameMode.Default)]
		[TestCase(PropertyNameMode.TypeName)]
		public async Task Should_Fail_WithExpectedPropertyName_When_AsyncValidationFails_ForNonPrimitiveType_UsingOverload_WithPropertyNameMode(PropertyNameMode mode)
		{
			var objToQuick = new ObjWithTwoPublicProps() { I = -1, PercentValue1 = 101 };
			var rule = GetAsyncRule();

			var result = await QuickValidator.ValidateAsync(objToQuick,
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

		[Test]
		public async Task Should_Pass_AsyncValidation_When_Valid()
		{
			const int valueToTest = 25;
			var result = await QuickValidator.ValidateAsync(valueToTest,
									(opt) => opt.GreaterThan(10)
												.InclusiveBetween(15, 25));
			Assert.That(result.IsValid, Is.True);
		}

		[Test]
		[TestCase(true)]
		[TestCase(false)]
		public void Should_Call_OnSuccess_When_Validation_Succeeds(bool isValid)
		{
			int valueFromHandler = 0;
			int valueToTest;
			if (isValid)
			{
				valueToTest = 25;
			}
			else
			{
				valueToTest = 5;
			}

			var result = QuickValidator.Validate(valueToTest,
									(opt) => opt.GreaterThan(10),
									"vv",
									(v) => valueFromHandler = v);
			if (isValid)
			{
				Assert.That(result.IsValid, Is.True);
				Assert.That(valueFromHandler, Is.EqualTo(25));
			}
			else
			{
				Assert.That(result.IsValid, Is.False);
				Assert.That(valueFromHandler, Is.EqualTo(0));
			}
		}

		[Test]
		[TestCase(true)]
		[TestCase(false)]
		public async Task Should_Call_OnSuccess_When_ValidationAsync_Succeeds(bool isValid)
		{
			int valueFromHandler = 0;
			int valueToTest;
			if (isValid)
			{
				valueToTest = 25;
			}
			else
			{
				valueToTest = 5;
			}

			var result = await  QuickValidator.ValidateAsync(valueToTest,
									(opt) => opt.GreaterThan(10),
									"vv",
									(v) => valueFromHandler = v);
			if (isValid)
			{
				Assert.That(result.IsValid, Is.True);
				Assert.That(valueFromHandler, Is.EqualTo(25));
			}
			else
			{
				Assert.That(result.IsValid, Is.False);
				Assert.That(valueFromHandler, Is.EqualTo(0));
			}
		}

		[Test]
		[TestCase(true)]
		[TestCase(false)]
		public void Should_Call_OnSuccess_When_Validation_Succeeds_UsingOverload_WithPropertyNameMode(bool isValid)
		{
			int valueFromHandler = 0;
			int valueToTest;
			if (isValid)
			{
				valueToTest = 25;
			}
			else
			{
				valueToTest = 5;
			}

			var result = QuickValidator.Validate(valueToTest,
									(opt) => opt.GreaterThan(10),
									PropertyNameMode.TypeName,
									(v) => valueFromHandler = v);
			if (isValid)
			{
				Assert.That(result.IsValid, Is.True);
				Assert.That(valueFromHandler, Is.EqualTo(25));
			}
			else
			{
				Assert.That(result.IsValid, Is.False);
				Assert.That(valueFromHandler, Is.EqualTo(0));
			}
		}

		[Test]
		[TestCase(true)]
		[TestCase(false)]
		public async Task Should_Call_OnSuccess_When_AsyncValidation_Succeeds_UsingOverload_WithPropertyNameMode(bool isValid)
		{
			int valueFromHandler = 0;
			int valueToTest;
			if (isValid)
			{
				valueToTest = 25;
			}
			else
			{
				valueToTest = 5;
			}

			var result = await QuickValidator.ValidateAsync(valueToTest,
									(opt) => opt.GreaterThan(10),
									PropertyNameMode.TypeName,
									(v) => valueFromHandler = v);
			if (isValid)
			{
				Assert.That(result.IsValid, Is.True);
				Assert.That(valueFromHandler, Is.EqualTo(25));
			}
			else
			{
				Assert.That(result.IsValid, Is.False);
				Assert.That(valueFromHandler, Is.EqualTo(0));
			}
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

		private static Action<IRuleBuilderOptions<ObjWithTwoPublicProps, ObjWithTwoPublicProps>> GetAsyncRule()
		{
			return (opt) =>
							opt
							.ChildRules((v) => v.RuleFor(o => o.I)
								.GreaterThan(0))
								.MustAsync(async (_, __) => { await Task.Delay(1); return true; })
							.ChildRules((v) => v.RuleFor(o => o.PercentValue1)
								.InclusiveBetween(0, 100)
								.MustAsync(async (_, __) => { await Task.Delay(1); return true; })
								);
		}

		private static Action<IRuleBuilderOptions<ObjWithTwoPublicProps, ObjWithTwoPublicProps>> GetRuleWithOverriddenPropertyName()
		{
			return (opt) =>
							opt
							.ChildRules((v) => v.RuleFor(o => o.I)
								.GreaterThan(0).OverridePropertyName("MyPropNameI"))
							.ChildRules((v) => v.RuleFor(o => o.PercentValue1)
								.InclusiveBetween(0, 100));
		}

		private static Action<IRuleBuilderOptions<ObjWithTwoPublicProps, ObjWithTwoPublicProps>> GetAsyncRuleWithOverriddenPropertyName()
		{
			return (opt) =>
							opt
							.ChildRules((v) => v.RuleFor(o => o.I)
								.GreaterThan(0).OverridePropertyName("MyPropNameI"))
								.MustAsync(async (_, __) => { await Task.Delay(1); return true; })
							.ChildRules((v) => v.RuleFor(o => o.PercentValue1)
								.InclusiveBetween(0, 100)
								.MustAsync(async (_, __) => { await Task.Delay(1); return true; })
								);
		}
	}
}
