using FluentValidation;
using FluentValidation.Results;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace ExpressValidator.Tests
{
	internal class ExpressValidatorNullObjectTests
	{
		private static readonly string NullErrorMessageForClass = NullFallbackMessageProvider.GetMessage(typeof(Contact).Name,
																								   new ValidationContext<string>(null));

		private static readonly string NullErrorMessageForStruct = NullFallbackMessageProvider.GetMessage(typeof(ContactStruct?).Name,
																									new ValidationContext<string>(null));

		[Test]
		[TestCase(true)]
		[TestCase(false)]
		public async Task Should_NotThrow_When_Class_To_Validate_Is_Null(bool isAsync)
		{
			var validator = new ExpressValidatorBuilder<Contact>()
						   .AddProperty(o => o.Name)
						   .WithValidation(o => o.NotEmpty()
											.MaximumLength(100))
						   .AddProperty(o => o.Email)
						   .WithValidation(o => o.NotEmpty()
											.EmailAddress())
						   .Build();

			ValidationResult result = null;
			if (isAsync)
			{
				result = await validator.ValidateAsync(null);
			}
			else
			{
#pragma warning disable S6966 // Awaitable method should be used
				result = validator.Validate(null);
#pragma warning restore S6966 // Awaitable method should be used
			}

			Assert.That(result.IsValid, Is.False);
			Assert.That(result.Errors.Count, Is.EqualTo(1));
			Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(NullErrorMessageForClass));

			if (isAsync)
			{
				Assert.ThrowsAsync<ArgumentNullException>(async () => await new ContactValidator().ValidateAsync((Contact)null));
			}
			else
			{
				Assert.Throws<ArgumentNullException>(() => new ContactValidator().Validate((Contact)null));
			}
		}

		[Test]
		[TestCase(true)]
		[TestCase(false)]
		public async Task Should_NotThrow_When_Nullable_Struct_Is_Null(bool isAsync)
		{
			var validator = new ExpressValidatorBuilder<ContactStruct?>()
							.AddProperty(o => o.Value.Name)
							.WithValidation(o => o.NotEmpty()
											.MaximumLength(100))
							.AddProperty(o => o.Value.Email)
							.WithValidation(o => o.NotEmpty()
											.EmailAddress())
							.Build();

			ValidationResult result = null;
			if (isAsync)
			{
				result = await validator.ValidateAsync(null);
			}
			else
			{
#pragma warning disable S6966 // Awaitable method should be used
				result = validator.Validate(null);
#pragma warning restore S6966 // Awaitable method should be used
			}

			Assert.That(result.IsValid, Is.False);
			Assert.That(result.Errors.Count, Is.EqualTo(1));
			Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(NullErrorMessageForStruct));

			if (isAsync)
			{
				Assert.ThrowsAsync<ArgumentNullException>(async () => await new ContactNullableStructValidator().ValidateAsync((ContactStruct?)null));
			}
			else
			{
				Assert.Throws<ArgumentNullException>(() => new ContactNullableStructValidator().Validate((ContactStruct?)null));
			}
		}

		[Test]
		public async Task Should_NotThrow_When_Class_To_Validate_Is_Null_And_WithAsyncValidation_Is_Used()
		{
			var validator = new ExpressValidatorBuilder<Contact>()
							.AddProperty(o => o.Name)
							.WithValidation(o => o.NotEmpty()
											.MaximumLength(100))
							.AddProperty(o => o.Email)
							.WithValidation(o => o.NotEmpty()
											.EmailAddress())
							.AddProperty(o => o.K)
							.WithAsyncValidation(o => o.MustAsync(async (_, __) => { await Task.Delay(1); return true; }))
							.Build();

			var result = await validator.ValidateAsync(null);

			Assert.That(result.IsValid, Is.False);
			Assert.That(result.Errors.Count, Is.EqualTo(1));
			Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(NullErrorMessageForClass));

			Assert.ThrowsAsync<ArgumentNullException>(async () => await new ContactValidator().ValidateAsync((Contact)null));
		}

		[Test]
		public async Task Should_NotThrow_When_Struct_To_Validate_Is_Null_And_WithAsyncValidation_Is_Used()
		{
			var validator = new ExpressValidatorBuilder<ContactStruct?>()
						.AddProperty(o => o.Value.Email)
						.WithValidation(o => o.NotEmpty()
										.EmailAddress())
						.AddProperty(o => o.Value.Name)
							.WithAsyncValidation(o => o.MustAsync(async (_, __) => { await Task.Delay(1); return true; }))
						.Build();

			var result = await validator.ValidateAsync(null);

			Assert.That(result.IsValid, Is.False);
			Assert.That(result.Errors.Count, Is.EqualTo(1));
			Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(NullErrorMessageForStruct));

			Assert.ThrowsAsync<ArgumentNullException>(async () => await new ContactNullableStructValidator().ValidateAsync((ContactStruct?)null));
		}
	}
}
