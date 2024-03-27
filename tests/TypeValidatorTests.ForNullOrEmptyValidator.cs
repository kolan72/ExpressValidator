using FluentValidation;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace ExpressValidator.Tests
{
	internal partial class TypeValidatorTests
    {
		[Test]
		[TestCase(true, null, true)]
		[TestCase(false, null, true)]
		[TestCase(true, "t", false)]
		[TestCase(false, "t", false)]
		public void Should_OnlyNullValidation_Be_Corect_ForNull_And_NotNull(bool single, string valueToTest, bool isValid)
		{
			var validator = new TypeValidator<string>();
			if (single)
				validator.SetValidation(o => o.Null(), "someprop");
			else
				validator.SetValidation(o => o.Null().Null(), "someprop");

			var res = validator.ValidateEx(valueToTest);
			ClassicAssert.AreEqual(isValid, res.IsValid);
		}

		[Test]
		[TestCase(true, null, true)]
		[TestCase(false, null, true)]
		[TestCase(true, "t", false)]
		[TestCase(false, "t", false)]
		public void Should_OnlyEmptyValidation_Be_Corect_ForNull_And_NotNull(bool single, string valueToTest, bool isValid)
		{
			var validator = new TypeValidator<string>();
			if (single)
				validator.SetValidation(o => o.Empty(), "someprop");
			else
				validator.SetValidation(o => o.Empty().Empty(), "someprop");

			var res = validator.ValidateEx(valueToTest);
			ClassicAssert.AreEqual(isValid, res.IsValid);
		}

		[Test]
		[TestCase(null, true)]
		[TestCase("t", false)]
		public void Should_NullAndEmptyValidation_Be_Corect_ForNull_And_NotNull(string valueToTest, bool isValid)
		{
			var validator = new TypeValidator<string>();
			validator.SetValidation(o => o.Empty().Null().Null().Empty(), "someprop");
			var (IsValid, _) = validator.ValidateEx(valueToTest);
			ClassicAssert.AreEqual(isValid, IsValid);
		}

		[Test]
		[TestCase(true)]
		[TestCase(false)]
		public void Should_NotOnlyNullValidation_Be_NotValid_ForNullValue(bool single)
		{
			var validator = new TypeValidator<string>();
			if (single)
				validator.SetValidation(o => o.Null().MinimumLength(1), "someprop");
			else
				validator.SetValidation(o => o.Null().Null().MinimumLength(1), "someprop");

			var res = validator.ValidateEx(null);
			ClassicAssert.AreEqual(false, res.IsValid);
		}
	}
}
