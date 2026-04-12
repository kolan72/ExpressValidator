using ExpressValidator.QuickValidation;
using FluentValidation;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace ExpressValidator.Tests.Net8
{
	internal class RequireValidatorTests
	{
		#region Require Extension Method Tests

		[Test]
		public void Require_ShouldCaptureExpression_Automatically()
		{
			var myValue = 5;

			Assert.That(() => myValue.Require(v => v.GreaterThan(10).WithMessage("Test")).Raise(), Throws.InstanceOf<InvalidOperationException>());
		}

		#endregion

		#region Raise Tests

		[Test]
		public void Raise_ShouldNotThrow_When_ValidationSucceeds()
		{
			const int valueToTest = 25;

			Assert.DoesNotThrow(() => valueToTest.Require(v => v.GreaterThan(10)).Raise());
		}

		[Test]
		public void Raise_ShouldThrowInvalidOperationException_When_ValidationFails()
		{
			const int valueToTest = 5;

			var ex = Assert.Throws<InvalidOperationException>(() => valueToTest.Require(v => v.GreaterThan(10)).Raise());
			Assert.That(ex.Message, Does.Contain("failed"));
		}

		[Test]
		public void Raise_ShouldUseCustomMessage_When_Provided()
		{
			const int valueToTest = 5;

			var ex = Assert.Throws<InvalidOperationException>(() => valueToTest.Require(v => v.GreaterThan(10)).Raise("My custom message"));
			Assert.That(ex.Message, Is.EqualTo("My custom message"));
		}

		[Test]
		public void Raise_ShouldThrowCustomExceptionType_When_Specified()
		{
			const int valueToTest = 5;

			var ex = Assert.Throws<ArgumentException>(() => valueToTest.Require(v => v.GreaterThan(10)).Raise<ArgumentException>());
			Assert.That(ex.Message, Does.Contain("failed"));
		}

		[Test]
		public void Raise_ShouldThrowCustomExceptionType_WithCustomMessage()
		{
			const int valueToTest = 5;

			var ex = Assert.Throws<ArgumentException>(() => valueToTest.Require(v => v.GreaterThan(10)).Raise<ArgumentException>("Arg message"));
			Assert.That(ex.Message, Is.EqualTo("Arg message"));
		}

		[Test]
		public void Raise_ShouldUseExceptionFactory_When_Provided()
		{
			const int valueToTest = 5;

			var ex = Assert.Throws<CustomValidationException>(() => valueToTest.Require(v => v.GreaterThan(10))
				.Raise((result, propName) => new CustomValidationException($"Custom: {propName}", result)));
			Assert.That(ex.Message, Does.Contain("Custom:"));
			Assert.That(ex.Result, Is.Not.Null);
		}

		[Test]
		public void Raise_ShouldUseExplicitPropertyName_When_Provided()
		{
			const int valueToTest = 5;

			var ex = Assert.Throws<InvalidOperationException>(() => valueToTest.Require(v => v.GreaterThan(10)).Raise(propName: "ExplicitPropName"));
			Assert.That(ex.Message, Does.Contain("ExplicitPropName"));
		}

		[Test]
		public void Raise_ShouldUseCapturedExpression_When_NoPropertyNameProvided()
		{
			var myValue = 5;

			var ex = Assert.Throws<InvalidOperationException>(() => myValue.Require(v => v.GreaterThan(10)).Raise());
			Assert.That(ex.Message, Does.Contain("myValue"));
		}

		#endregion

		#region RaiseAsync Tests

		[Test]
		public async Task RaiseAsync_ShouldNotThrow_When_ValidationSucceeds()
		{
			const int valueToTest = 25;

			await valueToTest.Require(v => v.GreaterThan(10)).RaiseAsync();

			Assert.Pass();
		}

		[Test]
		public async Task RaiseAsync_ShouldThrowInvalidOperationException_When_ValidationFails()
		{
			const int valueToTest = 5;

			var ex = Assert.ThrowsAsync<InvalidOperationException>(async () => await valueToTest.Require(v => v.GreaterThan(10)).RaiseAsync());
			Assert.That(ex.Message, Does.Contain("failed"));
		}

		[Test]
		public async Task RaiseAsync_ShouldUseCustomMessage_When_Provided()
		{
			const int valueToTest = 5;

			var ex = Assert.ThrowsAsync<InvalidOperationException>(async () => await valueToTest.Require(v => v.GreaterThan(10)).RaiseAsync("Async custom message"));
			Assert.That(ex.Message, Is.EqualTo("Async custom message"));
		}

		[Test]
		public async Task RaiseAsync_ShouldUseCapturedExpression_When_NoPropertyNameProvided()
		{
			var myValue = 5;

			var ex = Assert.ThrowsAsync<InvalidOperationException>(async () => await myValue.Require(v => v.GreaterThan(10)).RaiseAsync());
			Assert.That(ex.Message, Does.Contain("myValue"));
		}

		#endregion

		#region Complex Object Tests

		[Test]
		public void Raise_ShouldWork_With_ComplexObjects()
		{
			var objToQuick = new ObjWithTwoPublicProps() { I = -1, PercentValue1 = 101 };

			var ex = Assert.Throws<InvalidOperationException>(() => objToQuick.Require(opt => opt
					.ChildRules(v => v.RuleFor(o => o.I).GreaterThan(0))
					.ChildRules(v => v.RuleFor(o => o.PercentValue1).InclusiveBetween(0, 100)))
				.Raise());
			Assert.That(ex.Message, Does.Contain("failed"));
		}

		[Test]
		public void Raise_ShouldUseWithExceptionType_With_ComplexObjects()
		{
			var objToQuick = new ObjWithTwoPublicProps() { I = -1, PercentValue1 = 101 };

			var ex = Assert.Throws<CustomValidationException>(() => objToQuick.Require(opt => opt
					.ChildRules(v => v.RuleFor(o => o.I).GreaterThan(0))
					.ChildRules(v => v.RuleFor(o => o.PercentValue1).InclusiveBetween(0, 100)))
				.Raise<CustomValidationException>());
			Assert.That(ex.Message, Does.Contain("failed"));
		}

		#endregion

		#region Helper Classes

		private class CustomValidationException : Exception
		{
			public FluentValidation.Results.ValidationResult Result { get; }

			public CustomValidationException(string message, FluentValidation.Results.ValidationResult result)
				: base(message)
			{
				Result = result;
			}

			public CustomValidationException(string message) : base(message)
			{
			}
		}

		#endregion
	}
}
