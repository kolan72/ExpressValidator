using FluentValidation;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using System.Threading.Tasks;

namespace ExpressValidator.Tests
{
	internal partial class ExpressAsyncPropertyValidatorTests
	{
		[Test]
		[TestCase(true, null, true)]
		[TestCase(false, null, true)]
		[TestCase(true, "t", false)]
		[TestCase(false, "t", false)]
		public async Task Should_OnlyNullValidation_Be_Corect_ForNull_And_NotNull(bool single, string valueToTest, bool isValid)
		{
			var validator = new ExpressAsyncPropertyValidator<string>(null);
			if (single)
				validator.SetValidation(o => o.Null());
			else
				validator.SetValidation(o => o.Null().Null());

			var res = await validator.ValidateExAsync(valueToTest);
			ClassicAssert.AreEqual(isValid, res.IsValid);
		}

		[Test]
		[TestCase(true, null, true)]
		[TestCase(false, null, true)]
		[TestCase(true, "t", false)]
		[TestCase(false, "t", false)]
		public async Task Should_OnlyEmptyValidation_Be_Corect_ForNull_And_NotNull(bool single, string valueToTest, bool isValid)
		{
			var validator = new ExpressAsyncPropertyValidator<string>(null);
			if (single)
				validator.SetValidation(o => o.Empty());
			else
				validator.SetValidation(o => o.Empty().Empty());

			var res = await validator.ValidateExAsync(valueToTest);
			ClassicAssert.AreEqual(isValid, res.IsValid);
		}

		[Test]
		[TestCase(null, true)]
		[TestCase("t", false)]
		public async Task Should_NullAndEmptyValidation_Be_Corect_ForNull_And_NotNull(string valueToTest, bool isValid)
		{
			var validator = new ExpressAsyncPropertyValidator<string>(null);
			validator.SetValidation(o => o.Empty().Null().Null().Empty());
			var res = await validator.ValidateExAsync(valueToTest);
			ClassicAssert.AreEqual(isValid, res.IsValid);
		}
	}
}
