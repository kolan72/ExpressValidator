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
    internal partial class ExpressValidatorTests
    {
		[Test]
		public void Should_Validate_Throw_For_AsyncRule_ForProperty()
		{
			var builder = new ExpressValidatorBuilder<ObjWithTwoPublicProps>()
						   .AddProperty(o => o.I)
						   .WithAsyncValidation(o => o.MustAsync(async (_, __) => { await Task.Delay(1); return true; }))
						   .Build();
			var exc = Assert.Throws<InvalidOperationException>(() => builder.Validate(new ObjWithTwoPublicProps() { I = 1, S = "b" }));
			Assert.That(exc.Message, Does.StartWith("Object validator has a property or field with asynchronous validation rules."));
		}

		[Test]
		public async Task Should_AsyncInvoke_SuccessValidationHandler_When_IsValid()
		{
			int percentSum = 0;
			var result = await new ExpressValidatorBuilder<ObjWithTwoPublicProps>()
						   .AddFunc(o => o.PercentValue1 + o.PercentValue2, "percentSum", (p) => percentSum = p)
						   .WithValidation(o => o.MustAsync(async (_, __) => { await Task.Delay(1); return true; }).InclusiveBetween(0, 100))
						   .Build()
						   .ValidateAsync(new ObjWithTwoPublicProps() { PercentValue1 = 20, PercentValue2 = 80 });
			Assert.That(percentSum, Is.EqualTo(100));
			Assert.That(result.IsValid, Is.True);
		}

		[Test]
		public async Task Should_Not_AsyncInvoke_SuccessValidationHandler_When_IsNotValid()
		{
			int percentSum = 0;
			var result = await new ExpressValidatorBuilder<ObjWithTwoPublicProps>()
						   .AddFunc(o => o.PercentValue1 + o.PercentValue2, "percentSum", (p) => percentSum = p)
						   .WithValidation(o => o.MustAsync(async (_, __) => { await Task.Delay(1); return true; }).InclusiveBetween(0, 100))
						   .Build()
						   .ValidateAsync(new ObjWithTwoPublicProps() { PercentValue1 = 20, PercentValue2 = 82 });
			Assert.That(percentSum, Is.EqualTo(0));
			Assert.That(result.IsValid, Is.False);
		}

		[Test]
		public void Should_Validate_Throw_For_AsyncRule_ForField()
		{
			var builder = new ExpressValidatorBuilder<ObjWithTwoPublicProps>()
						   .AddField(o => o._sField)
						   .WithAsyncValidation(o => o.MustAsync(async (_, __) => { await Task.Delay(1); return true; }))
						   .Build();
			var exc = Assert.Throws<InvalidOperationException>(() => builder.Validate(new ObjWithTwoPublicProps() { I = 1, _sField = "b" }));
			Assert.That(exc.Message, Does.StartWith("Object validator has a property or field with asynchronous validation rules."));
		}

		[Test]
		public void Should_Validate_Throw_For_AsyncRule_ForFunc()
		{
			var builder = new ExpressValidatorBuilder<ObjWithTwoPublicProps>()
						   .AddFunc(o => o.PercentValue1 + o.PercentValue2, "percentSum")
						   .WithAsyncValidation(o => o.MustAsync(async (_, __) => { await Task.Delay(1); return true; }))
						   .Build();
			var exc = Assert.Throws<InvalidOperationException>(() => builder.Validate(new ObjWithTwoPublicProps() { I = 1, _sField = "b" }));
			Assert.That(exc.Message, Does.StartWith("Object validator has a property or field with asynchronous validation rules."));
		}

		[Test]
		public async Task Should_ValidateAsync_Work_For_AsyncRules()
		{
			var result = await new ExpressValidatorBuilder<ObjWithTwoPublicProps>()
			   .AddProperty(o => o.I)
			   .WithAsyncValidation(o => o.MustAsync(async (_, __) => { await Task.Delay(1); return true; }))
			   .AddField(o => o._sField)
			   .WithAsyncValidation(o => o.MustAsync(async (_, __) => { await Task.Delay(1); return true; }))
			   .AddFunc(o => o.PercentValue1 + o.PercentValue2, "percentSum")
			   .WithAsyncValidation(o => o.MustAsync(async (_, __) => { await Task.Delay(1); return true; }))
			   .Build()
			   .ValidateAsync(new ObjWithTwoPublicProps() { I = 1, _sField = "" });

			ClassicAssert.AreEqual(true, result.IsValid);
		}

		[Test]
		public async Task Should_ValidateAsync_When_Using_Combined_Validation_Strategy()
		{
			var result = await new ExpressValidatorBuilder<ObjWithTwoPublicProps>()
			   .AddProperty(o => o.I)
			   .WithAsyncValidation(o => o.MustAsync(async (_, __) => { await Task.Delay(1); return false; }))
			   .AddFunc(o => o.PercentValue1 + o.PercentValue2, "percentSum")
			   .WithValidation(o => o.InclusiveBetween(0, 100))
			   .Build()
			   .ValidateAsync(new ObjWithTwoPublicProps() { I = 1, PercentValue1 = 200 });

			Assert.That(result.IsValid, Is.False);
			Assert.That(result.Errors.Count, Is.EqualTo(2));
		}
	}
}
