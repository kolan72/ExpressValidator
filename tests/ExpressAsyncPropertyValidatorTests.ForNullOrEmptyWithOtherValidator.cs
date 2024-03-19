using FluentValidation;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using System.Threading.Tasks;

namespace ExpressValidator.Tests
{
	internal partial class ExpressAsyncPropertyValidatorTests
	{
		[Test]
		[TestCase(true)]
		[TestCase(false)]
		public async Task Should_NotOnlyNullValidation_Be_NotValid_ForNullValue(bool single)
		{
			var validator = new ExpressAsyncPropertyValidator<string>(null);
			if (single)
				validator.SetValidation(o => o.Null().MinimumLength(1));
			else
				validator.SetValidation(o => o.Null().Null().MinimumLength(1));

			var (IsValid, Failures) = await validator.ValidateExAsync(null);
			ClassicAssert.AreEqual(false, IsValid);
			ClassicAssert.AreEqual(1, Failures.Count);
		}

		[Test]
		[TestCase(true)]
		[TestCase(false)]
		public async Task Should_NotOnlyEmptyValidation_Be_NotValid_ForNullValue(bool single)
		{
			var validator = new ExpressAsyncPropertyValidator<string>(null);
			if (single)
				validator.SetValidation(o => o.Empty().MinimumLength(1));
			else
				validator.SetValidation(o => o.Empty().Empty().MinimumLength(1));

			var (IsValid, Failures) = await validator.ValidateExAsync(null);
			ClassicAssert.AreEqual(false, IsValid);
			ClassicAssert.AreEqual(1, Failures.Count);
		}

		[Test]
		[TestCase(true)]
		[TestCase(false)]
		public async Task Should_NotOnlyEmptyValidation_Be_NotValid_ForNullValue_For_RefProperty_IfEmptyValidators_AreLast(bool single)
		{
			var validator = new ExpressPropertyValidator<string>(null);
			if (single)
				validator.SetValidation(o => o.MinimumLength(1).Empty());
			else
				validator.SetValidation(o => o.MinimumLength(1).Empty().Empty());

			var (IsValid, Failures) = await validator.ValidateExAsync(null);
			ClassicAssert.AreEqual(false, IsValid);
			ClassicAssert.AreEqual(1, Failures.Count);
		}

		[Test]
		[TestCase(true)]
		[TestCase(false)]
		public async Task Should_NotOnlyEmptyValidation_Be_NotValid_ForDefaultValue_For_ValueProperty(bool single)
		{
			var validator = new ExpressPropertyValidator<int>(null);
			if (single)
				validator.SetValidation(o => o.Empty().GreaterThan(1));
			else
				validator.SetValidation(o => o.Empty().Empty().GreaterThan(1));

			var (IsValid, Failures) = await validator.ValidateExAsync(0);
			ClassicAssert.AreEqual(false, IsValid);
			ClassicAssert.AreEqual(1, Failures.Count);
		}
	}
}
