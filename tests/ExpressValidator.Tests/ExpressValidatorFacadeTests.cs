using FluentValidation;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using System;
using System.Threading.Tasks;

namespace ExpressValidator.Tests
{
	/// <summary>
	/// Tests for the <see cref="IExpressValidatorFacade{TObj}"/> surface returned by <see cref="ExpressValidator.For{TObj}()"/>.
	/// </summary>
	[TestFixture]
	internal class ExpressValidatorFacadeTests
	{
		[Test]
		public void Should_For_Return_IExpressValidatorFacade()
		{
			IExpressValidatorFacade<ObjWithTwoPublicProps> facade = ExpressValidator.For<ObjWithTwoPublicProps>();
			Assert.That(facade, Is.Not.Null);
		}

		[Test]
		[TestCase(true)]
		[TestCase(false)]
		public void Should_Facade_Validate_Produce_Same_Result_As_Build_Validate(bool isValid)
		{
			int i = isValid ? 1 : -1;
			var obj = new ObjWithTwoPublicProps() { I = i };

			var facadeResult = ExpressValidator.For<ObjWithTwoPublicProps>()
							.AddProperty(o => o.I)
							.WithValidation(o => o.GreaterThan(0))
							.Validate(obj);

			var buildResult = new ExpressValidatorBuilder<ObjWithTwoPublicProps>()
							.AddProperty(o => o.I)
							.WithValidation(o => o.GreaterThan(0))
							.Build()
							.Validate(obj);

			ClassicAssert.AreEqual(buildResult.IsValid, facadeResult.IsValid);
			ClassicAssert.AreEqual(buildResult.Errors.Count, facadeResult.Errors.Count);
		}

		[Test]
		[TestCase(true)]
		[TestCase(false)]
		public async Task Should_Facade_ValidateAsync_Produce_Same_Result_As_Build_ValidateAsync(bool isValid)
		{
			int i = isValid ? 1 : -1;
			var obj = new ObjWithTwoPublicProps() { I = i };

			var facadeResult = await ExpressValidator.For<ObjWithTwoPublicProps>()
							.AddProperty(o => o.I)
							.WithAsyncValidation(o => o.GreaterThan(0))
							.ValidateAsync(obj);

			var buildResult = await new ExpressValidatorBuilder<ObjWithTwoPublicProps>()
							.AddProperty(o => o.I)
							.WithAsyncValidation(o => o.GreaterThan(0))
							.Build()
							.ValidateAsync(obj);

			ClassicAssert.AreEqual(buildResult.IsValid, facadeResult.IsValid);
			ClassicAssert.AreEqual(buildResult.Errors.Count, facadeResult.Errors.Count);
		}

		[Test]
		[TestCase(true)]
		[TestCase(false)]
		public void Should_Facade_Validate_With_Options_Produce_Same_Result_As_Build_Validate(bool isValid)
		{
			var options = new ObjWithTwoPublicPropsOptions()
			{
				IGreaterThanValue = isValid ? -1 : 1,
			};

			var obj = new ObjWithTwoPublicProps() { I = 0 };

			var facadeResult = ExpressValidator.For<ObjWithTwoPublicProps, ObjWithTwoPublicPropsOptions>()
							.AddProperty(o => o.I)
							.WithValidation((to, opt) => opt.GreaterThan(to.IGreaterThanValue))
							.Validate(obj, options);

			var buildResult = new ExpressValidatorBuilder<ObjWithTwoPublicProps, ObjWithTwoPublicPropsOptions>()
							.AddProperty(o => o.I)
							.WithValidation((to, opt) => opt.GreaterThan(to.IGreaterThanValue))
							.Build(options)
							.Validate(obj);

			ClassicAssert.AreEqual(buildResult.IsValid, facadeResult.IsValid);
			ClassicAssert.AreEqual(buildResult.Errors.Count, facadeResult.Errors.Count);
		}

		[Test]
		public void Should_Facade_Validate_Throw_When_AsyncRules_Used()
		{
			var facade = ExpressValidator.For<ObjWithTwoPublicProps>()
						   .AddProperty(o => o.I)
						   .WithAsyncValidation(o => o.MustAsync(async (_, __) => { await Task.Delay(1); return true; }));

			var exc = Assert.Throws<InvalidOperationException>(() => facade.Validate(new ObjWithTwoPublicProps() { I = 1 }));
			Assert.That(exc.Message, Does.StartWith("Object validator has a property or field with asynchronous validation rules."));
		}

		[Test]
		public void Should_Facade_Validate_Return_Failure_When_Object_Is_Null()
		{
			var result = ExpressValidator.For<ObjWithTwoPublicProps>()
						   .AddProperty(o => o.I)
						   .WithValidation(o => o.GreaterThan(0))
						   .Validate(null);

			Assert.That(result.IsValid, Is.False);
			Assert.That(result.Errors.Count, Is.EqualTo(1));
		}

		[Test]
		public async Task Should_Facade_ValidateAsync_Return_Failure_When_Object_Is_Null()
		{
			var result = await ExpressValidator.For<ObjWithTwoPublicProps>()
						   .AddProperty(o => o.I)
						   .WithValidation(o => o.GreaterThan(0))
						   .ValidateAsync(null);

			Assert.That(result.IsValid, Is.False);
			Assert.That(result.Errors.Count, Is.EqualTo(1));
		}
	}
}
