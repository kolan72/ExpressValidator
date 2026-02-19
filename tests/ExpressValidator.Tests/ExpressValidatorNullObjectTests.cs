using FluentValidation;
using NUnit.Framework;
using System;

namespace ExpressValidator.Tests
{
	internal class ExpressValidatorNullObjectTests
	{
		[Test]
		public void Should_NotThrow_When_Class_To_Validate_Is_Null()
		{
			var result = new ExpressValidatorBuilder<Contact>()
						   .AddProperty(o => o.Name)
						   .WithValidation(o => o.NotEmpty()
											.MaximumLength(100))
						   .AddProperty(o => o.Email)
						   .WithValidation(o => o.NotEmpty()
											.EmailAddress())
						   .Build()
						   .Validate(null);

			var em = NullFallbackMessageProvider.GetMessage(
				typeof(Contact).Name,
				new ValidationContext<string>(null));

			Assert.That(result.IsValid, Is.False);
			Assert.That(result.Errors.Count, Is.EqualTo(1));
			Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(em));

			Assert.Throws<ArgumentNullException>(() => new ContactValidator().Validate((Contact)null));
		}

		[Test]
		public void Should_NotThrow_When_Nullable_Struct_Is_Null()
		{
			var result = new ExpressValidatorBuilder<ContactStruct?>()
							.AddProperty(o => o.Value.Name)
							.WithValidation(o => o.NotEmpty()
											.MaximumLength(100))
							.AddProperty(o => o.Value.Email)
							.WithValidation(o => o.NotEmpty()
											.EmailAddress())
							.Build()
							.Validate(null);

			var em = NullFallbackMessageProvider.GetMessage(
				typeof(ContactStruct?).Name,
				new ValidationContext<string>(null));

			Assert.That(result.IsValid, Is.False);
			Assert.That(result.Errors.Count, Is.EqualTo(1));
			Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(em));

			Assert.Throws<ArgumentNullException>(() => new ContactNullableStructValidator().Validate((ContactStruct?)null));
		}
	}
}
