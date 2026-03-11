using NUnit.Framework;

namespace ExpressValidator.Tests.Net8
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
			customProfile.Configure(new ExpressValidatorBuilder<string>());

			// Assert
			Assert.That(customProfile.CustomConfigurationWasApplied, Is.True);
		}
	}

	internal class CustomValidatorProfile : ValidationProfile<string>
	{
		public bool CustomConfigurationWasApplied { get; private set; }

		public override void Configure(ExpressValidatorBuilder<string> expressValidatorBuilder)
		{
			CustomConfigurationWasApplied = true;
		}
	}
}
