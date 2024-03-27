using FluentValidation;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using System.Threading.Tasks;

namespace ExpressValidator.Tests
{
	internal partial class TypeAsyncValidatorTests
	{
		[Test]
		[TestCase(true)]
		[TestCase(false)]
		public async Task Should_NotOnlyNullValidation_Be_NotValid_ForNullValue(bool single)
		{
			var validator = new TypeAsyncValidator<string>();
			if (single)
				validator.SetValidation(o => o.Null().MinimumLength(1), "someprop");
			else
				validator.SetValidation(o => o.Null().Null().MinimumLength(1), "someprop");

			var (IsValid, Failures) = await validator.ValidateExAsync(null);
			ClassicAssert.AreEqual(false, IsValid);
			ClassicAssert.AreEqual(1, Failures.Count);
		}

		[Test]
		[TestCase(true)]
		[TestCase(false)]
		public async Task Should_NotOnlyEmptyValidation_Be_NotValid_ForNullValue(bool single)
		{
			var validator = new TypeAsyncValidator<string>();
			if (single)
				validator.SetValidation(o => o.Empty().MinimumLength(1), "someprop");
			else
				validator.SetValidation(o => o.Empty().Empty().MinimumLength(1), "someprop");

			var (IsValid, Failures) = await validator.ValidateExAsync(null);
			ClassicAssert.AreEqual(false, IsValid);
			ClassicAssert.AreEqual(1, Failures.Count);
		}

		[Test]
		[TestCase(true)]
		[TestCase(false)]
		public async Task Should_NotOnlyEmptyValidation_Be_NotValid_ForNullValue_For_RefProperty_IfEmptyValidators_AreLast(bool single)
		{
			var validator = new TypeAsyncValidator<string>();
			if (single)
				validator.SetValidation(o => o.MinimumLength(1).Empty(), "someprop");
			else
				validator.SetValidation(o => o.MinimumLength(1).Empty().Empty(), "someprop");

			var (IsValid, Failures) = await validator.ValidateExAsync(null);
			ClassicAssert.AreEqual(false, IsValid);
			ClassicAssert.AreEqual(1, Failures.Count);
		}

		[Test]
		[TestCase(true)]
		[TestCase(false)]
		public async Task Should_NotOnlyEmptyValidation_Be_NotValid_ForDefaultValue_For_ValueProperty(bool single)
		{
			var validator = new TypeAsyncValidator<int>();
			if (single)
				validator.SetValidation(o => o.Empty().GreaterThan(1), "someprop");
			else
				validator.SetValidation(o => o.Empty().Empty().GreaterThan(1), "someprop");

			var (IsValid, Failures) = await validator.ValidateExAsync(0);
			ClassicAssert.AreEqual(false, IsValid);
			ClassicAssert.AreEqual(1, Failures.Count);
		}
	}
}
