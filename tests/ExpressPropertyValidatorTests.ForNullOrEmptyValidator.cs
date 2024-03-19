using FluentValidation;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace ExpressValidator.Tests
{
	internal partial class ExpressPropertyValidatorTests
    {
		[Test]
		[TestCase(true, null, true)]
		[TestCase(false, null, true)]
		[TestCase(true, "t", false)]
		[TestCase(false, "t", false)]
		public void Should_OnlyNullValidation_Be_Corect_ForNull_And_NotNull(bool single, string valueToTest, bool isValid)
		{
			var validator = new ExpressPropertyValidator<string>(null);
			if (single)
				validator.SetValidation(o => o.Null());
			else
				validator.SetValidation(o => o.Null().Null());

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
			var validator = new ExpressPropertyValidator<string>(null);
			if (single)
				validator.SetValidation(o => o.Empty());
			else
				validator.SetValidation(o => o.Empty().Empty());

			var res = validator.ValidateEx(valueToTest);
			ClassicAssert.AreEqual(isValid, res.IsValid);
		}

		[Test]
		[TestCase(null, true)]
		[TestCase("t", false)]
		public void Should_NullAndEmptyValidation_Be_Corect_ForNull_And_NotNull(string valueToTest, bool isValid)
		{
			var validator = new ExpressPropertyValidator<string>(null);
			validator.SetValidation(o => o.Empty().Null().Null().Empty());
			var (IsValid, _) = validator.ValidateEx(valueToTest);
			ClassicAssert.AreEqual(isValid, IsValid);
		}

		[Test]
		[TestCase(true)]
		[TestCase(false)]
		public void Should_NotOnlyNullValidation_Be_NotValid_ForNullValue(bool single)
		{
			var validator = new ExpressPropertyValidator<string>(null);
			if (single)
				validator.SetValidation(o => o.Null().MinimumLength(1));
			else
				validator.SetValidation(o => o.Null().Null().MinimumLength(1));

			var res = validator.ValidateEx(null);
			ClassicAssert.AreEqual(false, res.IsValid);
		}
	}
}
