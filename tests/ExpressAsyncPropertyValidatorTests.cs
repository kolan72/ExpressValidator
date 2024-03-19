using NUnit.Framework;
using System.Threading.Tasks;
using FluentValidation;
using System.Reflection;
using NUnit.Framework.Legacy;
using System;

namespace ExpressValidator.Tests
{
	internal partial class ExpressAsyncPropertyValidatorTests
	{
		[Test]
		[TestCase("t", true)]
		[TestCase("tt", false)]
		public async Task Should_ValidateAsync_ForUsualRules_Work(string whatToTest, bool result)
		{
			PropertyInfoParser.TryParse<ObjWithNullable, string>(o => o.Value, out PropertyInfo propertyInfo);

			var validator = new ExpressAsyncPropertyValidator<string>(propertyInfo);
			validator.SetValidation(o => o.MaximumLength(1));
			var res = await validator.ValidateAsync(new ObjWithNullable() {  Value = whatToTest});
			ClassicAssert.AreEqual(result, res.IsValid);
			if (!result)
			{
				ClassicAssert.AreEqual(1, res.Failures.Count);
			}
		 }

		[Test]
		public void Should_Validate_Throw()
		{
			PropertyInfoParser.TryParse<ObjWithNullable, string>(o => o.Value, out PropertyInfo propertyInfo);

			var validator = new ExpressAsyncPropertyValidator<string>(propertyInfo);
			validator.SetValidation(o => o.MaximumLength(1));
			Assert.Throws<InvalidOperationException>(() => validator.Validate(new ObjWithNullable() { Value = "t" })) ;
		}
	}
}
