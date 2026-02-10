using NUnit.Framework;

namespace ExpressValidator.Tests
{
	[TestFixture]
	internal class ValidationProfileTests
	{
		[Test]
		public void Should_AllowDerivedClassesToConfigureValidator_WhenCreatingValidator()
		{
			// Arrange
			var customProfile = new CustomValidatorProfile();

			// Act
			var validator = customProfile.CreateValidator();

			// Assert
			Assert.That(validator, Is.Not.Null);
			Assert.That(customProfile.CustomConfigurationWasApplied, Is.True);
		}
	}

	internal class CustomValidatorProfile : ValidationProfile<string>
	{
		public bool CustomConfigurationWasApplied { get; private set; }

		public CustomValidatorProfile(OnFirstPropertyValidatorFailed option = OnFirstPropertyValidatorFailed.Continue)
			: base(option)
		{
		}

		public override void Configure(ExpressValidatorBuilder<string> expressValidatorBuilder)
		{
			CustomConfigurationWasApplied = true;
		}
	}
}
