using FluentValidation;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using System.Reflection;
using System.Threading.Tasks;

namespace ExpressValidator.Tests
{
	internal partial class ExpressPropertyValidatorTests
	{
		[Test]
		[TestCase("t", true)]
		[TestCase("tt", false)]
		public void Should_Validate_ForUsualRules_Work(string whatToTest, bool result)
		{
			PropertyInfoParser.TryParse<ObjWithNullable, string>(o => o.Value, out PropertyInfo propertyInfo);

			var validator = new ExpressPropertyValidator<string>(propertyInfo);
			validator.SetValidation(o => o.MaximumLength(1));
			var res = validator.Validate(new ObjWithNullable() { Value = whatToTest });
			ClassicAssert.AreEqual(result, res.IsValid);
			if (!result)
			{
				ClassicAssert.AreEqual(1, res.Failures.Count);
			}
		}

		[Test]
		[TestCase("t", true)]
		[TestCase("tt", false)]
		public async Task Should_ValidateAsync_ForUsualRules_Work(string whatToTest, bool result)
		{
			PropertyInfoParser.TryParse<ObjWithNullable, string>(o => o.Value, out PropertyInfo propertyInfo);

			var validator = new ExpressPropertyValidator<string>(propertyInfo);
			validator.SetValidation(o => o.MaximumLength(1));
			var res = await validator.ValidateAsync(new ObjWithNullable() { Value = whatToTest });
			ClassicAssert.AreEqual(result, res.IsValid);
			if (!result)
			{
				ClassicAssert.AreEqual(1, res.Failures.Count);
			}
		}
	}
}
