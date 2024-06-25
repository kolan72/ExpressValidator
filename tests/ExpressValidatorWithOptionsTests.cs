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
	internal class ExpressValidatorWithOptonsTests
	{
		[Test]
		public void Should_Work_When_IsValid()
		{
			var options = new ObjWithTwoPublicPropsOptions() { IGreaterThanValue = 0, SMaximumLengthValue = 1};
			var result = new ExpressValidatorBuilder<ObjWithTwoPublicProps, ObjWithTwoPublicPropsOptions>()
						   .AddProperty(o => o.I)
						   .WithValidation((to, p) => p.GreaterThan(to.IGreaterThanValue))
						   .AddProperty(o => o.S)
						   .WithValidation((to, p)=> p.MaximumLength(to.SMaximumLengthValue))
						   .Build(options)
						   .Validate(new ObjWithTwoPublicProps() { I = 1, S = "b"});
			ClassicAssert.AreEqual(true, result.IsValid);
		}

		[Test]
		public void Should_Validation_Result_Change_When_Options_Change()
		{
			var options1 = new ObjWithTwoPublicPropsOptions() { IGreaterThanValue = 0, SMaximumLengthValue = 1 };
			var builder = new ExpressValidatorBuilder<ObjWithTwoPublicProps, ObjWithTwoPublicPropsOptions>()
						   .AddProperty(o => o.I)
						   .WithValidation((to, p) => p.GreaterThan(to.IGreaterThanValue))
						   .AddProperty(o => o.S)
						   .WithValidation((to, p) => p.MaximumLength(to.SMaximumLengthValue));

			var result1 = builder.Build(options1)
								.Validate(new ObjWithTwoPublicProps() { I = 1, S = "b" });
			ClassicAssert.AreEqual(true, result1.IsValid);

			var options2 = new ObjWithTwoPublicPropsOptions() { IGreaterThanValue = 2, SMaximumLengthValue = 2 };
			var result2 = builder.Build(options2)
								.Validate(new ObjWithTwoPublicProps() { I = 1, S = "abc" });
			ClassicAssert.AreEqual(false, result2.IsValid);
			ClassicAssert.AreEqual(2, result2.Errors.Count);

			var options3 = new ObjWithTwoPublicPropsOptions() { IGreaterThanValue = 3, SMaximumLengthValue = 3 };
			var result3 = builder.Build(options3)
								.Validate(new ObjWithTwoPublicProps() { I = 2, S = "abcd" });
			ClassicAssert.AreEqual(false, result3.IsValid);
			ClassicAssert.AreEqual(2, result3.Errors.Count);
		}

		[Test]
		public void Should_IsValid_Equals_True_WhenNoValidators()
		{
			var options = new ObjWithTwoPublicPropsOptions() { IGreaterThanValue = 0, SMaximumLengthValue = 1 };
			var result = new ExpressValidatorBuilder<ObjWithTwoPublicProps, ObjWithTwoPublicPropsOptions>()
							.Build(options)
							.Validate(new ObjWithTwoPublicProps());
			Assert.That(result.IsValid, Is.True);
		}
	}
}
