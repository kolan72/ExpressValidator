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
	internal partial class ExpressPropertyValidatorTests
	{
		[Test]
		[TestCase(true)]
		[TestCase(false)]
		public void Should_NotOnlyEmptyValidation_Be_NotValid_ForNullValue_For_RefProperty(bool single)
		{
			var validator = new ExpressPropertyValidator<string>(null);
			if (single)
				validator.SetValidation(o => o.Empty().MinimumLength(1));
			else
				validator.SetValidation(o => o.Empty().Empty().MinimumLength(1));

			var (IsValid, Failures) = validator.ValidateEx(null);
			ClassicAssert.AreEqual(false, IsValid);
			ClassicAssert.AreEqual(1, Failures.Count);
		}

		[Test]
		[TestCase(true)]
		[TestCase(false)]
		public void Should_NotOnlyEmptyValidation_Be_NotValid_ForNullValue_For_RefProperty_IfEmptyValidators_AreLast(bool single)
		{
			var validator = new ExpressPropertyValidator<string>(null);
			if (single)
				validator.SetValidation(o => o.MinimumLength(1).Empty());
			else
				validator.SetValidation(o => o.MinimumLength(1).Empty().Empty());

			var (IsValid, Failures) = validator.ValidateEx(null);
			ClassicAssert.AreEqual(false, IsValid);
			ClassicAssert.AreEqual(1, Failures.Count);
		}

		[Test]
		[TestCase(true)]
		[TestCase(false)]
		public void Should_NotOnlyNullValidation_Be_NotValid_ForNullValue_For_RefProperty(bool single)
		{
			var validator = new ExpressPropertyValidator<string>(null);
			if (single)
				validator.SetValidation(o => o.Null().MinimumLength(1));
			else
				validator.SetValidation(o => o.Null().Null().MinimumLength(1));

			var (IsValid, Failures) = validator.ValidateEx(null);
			ClassicAssert.AreEqual(false, IsValid);
			ClassicAssert.AreEqual(1, Failures.Count);
		}

		[Test]
		[TestCase(true)]
		[TestCase(false)]
		public void Should_NotOnlyEmptyValidation_Be_NotValid_ForDefaultValue_For_ValueProperty(bool single)
		{
			var validator = new ExpressPropertyValidator<int>(null);
			if (single)
				validator.SetValidation(o => o.Empty().GreaterThan(1));
			else
				validator.SetValidation(o => o.Empty().Empty().GreaterThan(1));

			var (IsValid, Failures) = validator.ValidateEx(0);
			ClassicAssert.AreEqual(false, IsValid);
			ClassicAssert.AreEqual(1, Failures.Count);
		}
	}
}
