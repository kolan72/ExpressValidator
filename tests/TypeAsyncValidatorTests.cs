using NUnit.Framework;
using System.Threading.Tasks;
using FluentValidation;
using System.Reflection;
using NUnit.Framework.Legacy;
using System;

namespace ExpressValidator.Tests
{
	internal partial class TypeAsyncValidatorTests
	{
		[Test]
		[TestCase("t", true)]
		[TestCase("tt", false)]
		public async Task Should_ValidateAsync_ForUsualRules_Work(string whatToTest, bool result)
		{
			PropertyInfoParser.TryParse<ObjWithNullable, string>(o => o.Value, out PropertyInfo propertyInfo);

			var validator = new TypeAsyncValidator<string>();
			validator.SetValidation(o => o.MaximumLength(1), propertyInfo.Name);
			var res = await validator.ValidateAsync(whatToTest);
			ClassicAssert.AreEqual(result, res.IsValid);
			if (!result)
			{
				ClassicAssert.AreEqual(1, res.Errors.Count);
			}
		 }

		[Test]
		public void Should_Validate_Throw()
		{
			PropertyInfoParser.TryParse<ObjWithNullable, string>(o => o.Value, out PropertyInfo propertyInfo);

			var validator = new TypeAsyncValidator<string>();
			validator.SetValidation(o => o.MaximumLength(1), propertyInfo.Name);
			Assert.Throws<AsyncValidatorInvokedSynchronouslyException>(() => validator.Validate("t")) ;
		}
	}
}
