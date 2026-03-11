using FluentValidation;
using NUnit.Framework;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace ExpressValidator.Tests
{
	internal class PropertyValidationProcessorTests
	{
		[Test]
		[TestCase("t", true, false)]
		[TestCase("t", true, true)]
		[TestCase("tt", false, null)]
		public void Should_Validate_When_IsAsync_False(string whatToTest, bool result, bool? useHandler)
		{
			string handlerResult = null;
			void successHandler(string s) { handlerResult = s; }

			MemberInfoParser.TryParse<ObjWithNullable, string>(o => o.Value, MemberTypes.Property, out MemberInfo propertyInfo);

			var validator = new TypeValidator<string>();
			validator.SetValidation(o => o.MaximumLength(1), propertyInfo.Name);

			var processor = new PropertyValidationProcessor<ObjWithNullable, string>(o => o.Value, validator, useHandler == true ? successHandler : null);

			var (IsValid, Failures) = processor.Validate(new ObjWithNullable() { Value = whatToTest });
			Assert.That (IsValid, Is.EqualTo(result));
			if (!result)
			{
				Assert.That(Failures.Count, Is.EqualTo(1));
			}
			else
			{
				Assert.That(handlerResult, Is.EqualTo(useHandler == true ? whatToTest : null));
			}
		}

		[Test]
		[TestCase("t")]
		[TestCase("tt")]
		public void Should_Throw_On_Validate_When_IsAsync_True(string whatToTest)
		{
			MemberInfoParser.TryParse<ObjWithNullable, string>(o => o.Value, MemberTypes.Property, out MemberInfo propertyInfo);

			var validator = new TypeAsyncValidator<string>();
			validator.SetValidation(o => o.MaximumLength(1), propertyInfo.Name);

			var processor = new PropertyValidationProcessor<ObjWithNullable, string>(o => o.Value, validator, null);

			Assert.Throws<InvalidOperationException>(() => processor.Validate(new ObjWithNullable() { Value = whatToTest }));
		}

		[Test]
		[TestCase("t", true, false, true)]
		[TestCase("t", true, true, true)]
		[TestCase("tt", false, null, true)]
		[TestCase("t", true, false, false)]
		[TestCase("t", true, true, false)]
		[TestCase("tt", false, null, false)]
		public async Task Should_ValidateAsync_Do_Not_Depend_On_TypeValidator_Sync_Type(string whatToTest, bool result, bool? useHandler, bool isAsync)
		{
			string handlerResult = null;
			void successHandler(string s) { handlerResult = s; }

			MemberInfoParser.TryParse<ObjWithNullable, string>(o => o.Value, MemberTypes.Property, out MemberInfo propertyInfo);

			TypeValidatorBase<string> validator = isAsync ? new TypeAsyncValidator<string>() : new TypeValidator<string>();
			validator.SetValidation(o => o.MaximumLength(1), propertyInfo.Name);

			var processor = new PropertyValidationProcessor<ObjWithNullable, string>(o => o.Value, validator, useHandler == true ? successHandler : null);

			var (IsValid, Failures) = await processor.ValidateAsync(new ObjWithNullable() { Value = whatToTest });
			Assert.That(IsValid, Is.EqualTo(result));
			if (!result)
			{
				Assert.That(Failures.Count, Is.EqualTo(1));
			}
			else
			{
				Assert.That(handlerResult, Is.EqualTo(useHandler == true ? whatToTest : null));
			}
		}
	}
}
