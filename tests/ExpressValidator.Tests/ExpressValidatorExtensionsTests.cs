using NUnit.Framework;
using FluentValidation;
using ExpressValidator.Extensions;
using System.Threading.Tasks;

namespace ExpressValidator.Tests
{
	internal class ExpressValidatorExtensionsTests
	{
		[Test]
		[TestCase(true)]
		[TestCase(false)]
		public void Should_BuildAndValidate_Work(bool isValid)
		{
			int i = isValid ? 1 : -1;
			var objToValidate = new ObjWithTwoPublicProps() { I = i };

			var result = new ExpressValidatorBuilder<ObjWithTwoPublicProps>()
			   .AddProperty((o) => o.I)
			   .WithValidation((opt) => opt.GreaterThan(0))
			   .BuildAndValidate(objToValidate);

			Assert.That(result.IsValid, Is.EqualTo(isValid));
		}

		[Test]
		[TestCase(true)]
		[TestCase(false)]
		public void Should_BuildAndValidate_With_TOptions_Param_Work(bool isValid)
		{
			var objWithTwoPublicPropsOptions = new ObjWithTwoPublicPropsOptions()
			{
				IGreaterThanValue = isValid ? -1 : 1,
			};

			var objToValidate = new ObjWithTwoPublicProps() { I = 0 };

			var result = new ExpressValidatorBuilder<ObjWithTwoPublicProps, ObjWithTwoPublicPropsOptions>()
			   .AddProperty((o) => o.I)
			   .WithValidation((to, opt) => opt.GreaterThan(to.IGreaterThanValue))
			   .BuildAndValidate(objToValidate, objWithTwoPublicPropsOptions);

			Assert.That(result.IsValid, Is.EqualTo(isValid));
		}

		[Test]
		[TestCase(true)]
		[TestCase(false)]
		public async Task Should_BuildAndValidateAsync_Work(bool isValid)
		{
			int i = isValid ? 1 : -1;
			var objToValidate = new ObjWithTwoPublicProps() { I = i };

			var result = await new ExpressValidatorBuilder<ObjWithTwoPublicProps>()
			   .AddProperty((o) => o.I)
			   .WithAsyncValidation((opt) => opt.GreaterThan(0).MustAsync(async(_, __) => { await Task.Delay(1); return true; }))
			   .BuildAndValidateAsync(objToValidate);

			Assert.That(result.IsValid, Is.EqualTo(isValid));
		}
	}
}
