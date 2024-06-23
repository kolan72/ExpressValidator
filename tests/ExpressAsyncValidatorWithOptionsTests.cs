using FluentValidation;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using System.Threading.Tasks;

namespace ExpressValidator.Tests
{
	internal class ExpressAsyncValidatorWithOptionsTests
	{
		[Test]
		public async Task Should_Work_When_IsValid()
		{
			var options = new ObjWithTwoPublicPropsOptions() { IGreaterThanValue = 0, SMaximumLengthValue = 1 };
			var result = await new ExpressValidatorBuilder<ObjWithTwoPublicProps, ObjWithTwoPublicPropsOptions>()
						   .AddProperty(o => o.I)
						   .WithAsyncValidation((to, p) => p.GreaterThan(to.IGreaterThanValue).MustAsync(async (_, __) => { await Task.Delay(1); return true;}))
						   .AddProperty(o => o.S)
						   .WithAsyncValidation((to, p) => p.MaximumLength(to.SMaximumLengthValue).MustAsync(async (_, __) => { await Task.Delay(1); return true;}))
						   .Build(options)
						   .ValidateAsync(new ObjWithTwoPublicProps() { I = 1, S = "b" });
			ClassicAssert.AreEqual(true, result.IsValid);
		}

		[Test]
		public async Task Should_Validation_Result_Change_When_Options_Change()
		{
			var options1 = new ObjWithTwoPublicPropsOptions() { IGreaterThanValue = 0, SMaximumLengthValue = 1 };
			var builder = new ExpressValidatorBuilder<ObjWithTwoPublicProps, ObjWithTwoPublicPropsOptions>()
						   .AddProperty(o => o.I)
						   .WithAsyncValidation((to, p) => p.GreaterThan(to.IGreaterThanValue).MustAsync(async (_, __) => { await Task.Delay(1); return true; }))
						   .AddProperty(o => o.S)
						   .WithAsyncValidation((to, p) => p.MaximumLength(to.SMaximumLengthValue).MustAsync(async (_, __) => { await Task.Delay(1); return true; }));

			var result1 = await builder.Build(options1)
								.ValidateAsync(new ObjWithTwoPublicProps() { I = 1, S = "b" });
			ClassicAssert.AreEqual(true, result1.IsValid);

			var options2 = new ObjWithTwoPublicPropsOptions() { IGreaterThanValue = 2, SMaximumLengthValue = 2 };
			var result2 = await builder.Build(options2)
								.ValidateAsync(new ObjWithTwoPublicProps() { I = 1, S = "abc" });
			ClassicAssert.AreEqual(false, result2.IsValid);
			ClassicAssert.AreEqual(2, result2.Errors.Count);

			var options3 = new ObjWithTwoPublicPropsOptions() { IGreaterThanValue = 3, SMaximumLengthValue = 3 };
			var result3 = await builder.Build(options3)
								.ValidateAsync(new ObjWithTwoPublicProps() { I = 2, S = "abcd" });
			ClassicAssert.AreEqual(false, result3.IsValid);
			ClassicAssert.AreEqual(2, result3.Errors.Count);
		}

		[Test]
		[TestCase(true)]
		[TestCase(false)]
		public async Task Should_Work_When_TheSamePropertyValidators_In_A_Row(bool isValid)
		{
			var options = new ObjWithTwoPublicPropsOptions() { IGreaterThanValue = 1, IGreaterThanValue2 = 2 };
			int i = isValid ? 3 : -1;
			var validator =  new ExpressValidatorBuilder<ObjWithTwoPublicProps, ObjWithTwoPublicPropsOptions>()
							.AddProperty(o => o.I)
							.WithValidation((topt, p) => p.GreaterThan(topt.IGreaterThanValue))
							.AddProperty(o => o.I)
							.WithValidation((topt, p) => p.GreaterThan(topt.IGreaterThanValue2))
							.Build(options);

			var result = await validator.ValidateAsync(new ObjWithTwoPublicProps() { I = i });
			if (isValid)
			{
				Assert.That(result.IsValid, Is.True);
			}
			else
			{
				Assert.That(result.Errors.Count, Is.EqualTo(2));
				Assert.That(result.IsValid, Is.False);
			}
		}
	}
}
