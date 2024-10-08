using FluentValidation;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using System.Threading.Tasks;

namespace ExpressValidator.Tests
{
	internal partial class TypeAsyncValidatorTests
	{
		[Test]
		[TestCase(true, null, true)]
		[TestCase(false, null, true)]
		[TestCase(true, "t", false)]
		[TestCase(false, "t", false)]
		public async Task Should_OnlyNullValidation_Be_Corect_ForNull_And_NotNull(bool single, string valueToTest, bool isValid)
		{
			var validator = new TypeAsyncValidator<string>();
			if (single)
				validator.SetValidation(o => o.Null(), "someprop");
			else
				validator.SetValidation(o => o.Null().Null(), "someprop");

			var (IsValid, _) = await validator.ValidateExAsync(valueToTest);
			ClassicAssert.AreEqual(isValid, IsValid);
		}

		[Test]
		[TestCase(true, null, true)]
		[TestCase(false, null, true)]
		[TestCase(true, 1, false)]
		[TestCase(false, 1, false)]
		public async Task Should_OnlyNullValidation_Be_Corect_ForNull_And_NotNull_NullableValue(bool single, int? valueToTest, bool isValid)
		{
			var validator = new TypeAsyncValidator<int?>();
			if (single)
				validator.SetValidation(o => o.Null(), "someprop");
			else
				validator.SetValidation(o => o.Null().Null(), "someprop");

			var (IsValid, _) = await validator.ValidateExAsync(valueToTest);
			ClassicAssert.AreEqual(isValid, IsValid);
		}

		[Test]
		[TestCase(true, null, true)]
		[TestCase(false, null, true)]
		[TestCase(true, "t", false)]
		[TestCase(false, "t", false)]
		public async Task Should_OnlyEmptyValidation_Be_Corect_ForNull_And_NotNull(bool single, string valueToTest, bool isValid)
		{
			var validator = new TypeAsyncValidator<string>();
			if (single)
				validator.SetValidation(o => o.Empty(), string.Empty);
			else
				validator.SetValidation(o => o.Empty().Empty(), string.Empty);

			var (IsValid, _) = await validator.ValidateExAsync(valueToTest);
			ClassicAssert.AreEqual(isValid, IsValid);
		}

		[Test]
		[TestCase(true, null, true)]
		[TestCase(false, null, true)]
		[TestCase(true, 1, false)]
		[TestCase(false, 1, false)]
		public async Task Should_OnlyEmptyValidation_Be_Corect_ForNull_And_NotNull_For_NullableValue(bool single, int? valueToTest, bool isValid)
		{
			var validator = new TypeAsyncValidator<int?>();
			if (single)
				validator.SetValidation(o => o.Empty(), string.Empty);
			else
				validator.SetValidation(o => o.Empty().Empty(), string.Empty);

			var (IsValid, _) = await validator.ValidateExAsync(valueToTest);
			ClassicAssert.AreEqual(isValid, IsValid);
		}

		[Test]
		[TestCase(null, true)]
		[TestCase("t", false)]
		public async Task Should_NullAndEmptyValidation_Be_Corect_ForNull_And_NotNull(string valueToTest, bool isValid)
		{
			var validator = new TypeAsyncValidator<string>();
			validator.SetValidation(o => o.Empty().Null().Null().Empty(), "someprop");
			var (IsValid, _) = await validator.ValidateExAsync(valueToTest);
			ClassicAssert.AreEqual(isValid, IsValid);
		}

		[Test]
		[TestCase(null, true)]
		[TestCase(1, false)]
		public async Task Should_NullAndEmptyValidation_Be_Corect_ForNull_And_NotNull_For_NullableValue(int? valueToTest, bool isValid)
		{
			var validator = new TypeAsyncValidator<int?>();
			validator.SetValidation(o => o.Empty().Null().Null().Empty(), "someprop");
			var (IsValid, _) = await validator.ValidateExAsync(valueToTest);
			ClassicAssert.AreEqual(isValid, IsValid);
		}
	}
}
