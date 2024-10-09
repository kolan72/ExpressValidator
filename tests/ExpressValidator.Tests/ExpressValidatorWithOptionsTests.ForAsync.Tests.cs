using FluentValidation;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace ExpressValidator.Tests
{
	internal partial class ExpressValidatorWithOptonsTests
	{
		[Test]
		public void Should_Validate_Throw_For_AsyncRule_ForProperty()
		{
			var builder = new ExpressValidatorBuilder<ObjWithTwoPublicProps, ObjWithTwoPublicPropsOptions>()
						   .AddProperty(o => o.I)
						   .WithAsyncValidation((_, p) => p.MustAsync(async (_, __) => { await Task.Delay(1); return true; }))
						   .Build(_objWithTwoPublicPropsOptions);
			var exc = Assert.Throws<InvalidOperationException>(() => builder.Validate(new ObjWithTwoPublicProps() { I = 1, S = "b" }));
			Assert.That(exc.Message, Does.StartWith("Object validator has a property or field with asynchronous validation rules."));
		}

		[Test]
		public void Should_Validate_Throw_For_AsyncRule_ForField()
		{
			var builder = new ExpressValidatorBuilder<ObjWithTwoPublicProps, ObjWithTwoPublicPropsOptions>()
						   .AddField(o => o._sField)
						   .WithAsyncValidation((_, f) => f.MustAsync(async (_, __) => { await Task.Delay(1); return true; }))
						   .Build(_objWithTwoPublicPropsOptions);
			var exc = Assert.Throws<InvalidOperationException>(() => builder.Validate(new ObjWithTwoPublicProps() { I = 1, _sField = "b" }));
			Assert.That(exc.Message, Does.StartWith("Object validator has a property or field with asynchronous validation rules."));
		}

		[Test]
		public void Should_Validate_Throw_For_AsyncRule_ForFunc()
		{
			var builder = new ExpressValidatorBuilder<ObjWithTwoPublicProps, ObjWithTwoPublicPropsOptions>()
						   .AddFunc(o => o.PercentValue1 + o.PercentValue2, "percentSum")
						   .WithAsyncValidation((_, f) => f.MustAsync(async (_, __) => { await Task.Delay(1); return true; }))
						   .Build(_objWithTwoPublicPropsOptions);
			var exc = Assert.Throws<InvalidOperationException>(() => builder.Validate(new ObjWithTwoPublicProps() { I = 1, _sField = "b" }));
			Assert.That(exc.Message, Does.StartWith("Object validator has a property or field with asynchronous validation rules."));
		}

		[Test]
		public async Task Should_ValidateAsync_Work_For_AsyncRules()
		{
			var result = await new ExpressValidatorBuilder<ObjWithTwoPublicProps, ObjWithTwoPublicPropsOptions>()
			   .AddProperty(o => o.I)
			   .WithAsyncValidation((_, p) => p.MustAsync(async (_, __) => { await Task.Delay(1); return true; }))
			   .AddField(o => o._sField)
			   .WithAsyncValidation((_, p) => p.MustAsync(async (_, __) => { await Task.Delay(1); return true; }))
			   .AddFunc(o => o.PercentValue1 + o.PercentValue2, "percentSum")
			   .WithAsyncValidation((_, p) => p.MustAsync(async (_, __) => { await Task.Delay(1); return true; }))
			   .Build(_objWithTwoPublicPropsOptions)
			   .ValidateAsync(new ObjWithTwoPublicProps() { I = 1, _sField = "" });

			Assert.That(result.IsValid, Is.EqualTo(true));
		}
	}
}
