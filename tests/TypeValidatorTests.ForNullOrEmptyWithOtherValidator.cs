using FluentValidation;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressValidator.Tests
{
	internal partial class TypeValidatorTests
	{
		[Test]
		[TestCase(true)]
		[TestCase(false)]
		public void Should_NotOnlyEmptyValidation_Be_NotValid_ForNullValue_For_RefProperty(bool single)
		{
			var validator = new TypeValidator<string>();
			if (single)
				validator.SetValidation(o => o.Empty().MinimumLength(1), "someprop");
			else
				validator.SetValidation(o => o.Empty().Empty().MinimumLength(1), "someprop");

			var (IsValid, Failures) = validator.ValidateEx(null);
			ClassicAssert.AreEqual(false, IsValid);
			ClassicAssert.AreEqual(1, Failures.Count);
		}

		[Test]
		[TestCase(true)]
		[TestCase(false)]
		public void Should_NotOnlyEmptyValidation_Be_NotValid_ForNullValue_For_RefProperty_IfEmptyValidators_AreLast(bool single)
		{
			var validator = new TypeValidator<string>();
			if (single)
				validator.SetValidation(o => o.MinimumLength(1).Empty(), "someprop");
			else
				validator.SetValidation(o => o.MinimumLength(1).Empty().Empty(), "someprop");

			var (IsValid, Failures) = validator.ValidateEx(null);
			ClassicAssert.AreEqual(false, IsValid);
			ClassicAssert.AreEqual(1, Failures.Count);
		}

		[Test]
		[TestCase(true)]
		[TestCase(false)]
		public void Should_NotOnlyNullValidation_Be_NotValid_ForNullValue_For_RefProperty(bool single)
		{
			var validator = new TypeValidator<string>();
			if (single)
				validator.SetValidation(o => o.Null().MinimumLength(1), "someprop");
			else
				validator.SetValidation(o => o.Null().Null().MinimumLength(1), "someprop");

			var (IsValid, Failures) = validator.ValidateEx(null);
			ClassicAssert.AreEqual(false, IsValid);
			ClassicAssert.AreEqual(1, Failures.Count);
		}

		[Test]
		[TestCase(true)]
		[TestCase(false)]
		public void Should_NotOnlyEmptyValidation_Be_NotValid_ForDefaultValue_For_ValueProperty(bool single)
		{
			var validator = new TypeValidator<int>();
			if (single)
				validator.SetValidation(o => o.Empty().GreaterThan(1), "someprop");
			else
				validator.SetValidation(o => o.Empty().Empty().GreaterThan(1), "someprop");

			var (IsValid, Failures) = validator.ValidateEx(0);
			ClassicAssert.AreEqual(false, IsValid);
			ClassicAssert.AreEqual(1, Failures.Count);
		}
	}
}
