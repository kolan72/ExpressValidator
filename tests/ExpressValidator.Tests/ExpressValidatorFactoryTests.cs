using ExpressValidator.Extensions;
using FluentValidation;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using System.Threading.Tasks;

namespace ExpressValidator.Tests
{
	internal class ExpressValidatorFactoryTests
	{
		[Test]
		public void Should_For_Return_NonNull_Builder()
		{
			var builder = ExpressValidator.For<ObjWithTwoPublicProps>();
			Assert.That(builder, Is.Not.Null);
		}

		[Test]
		public void Should_For_With_Mode_Return_NonNull_Builder()
		{
			var builder = ExpressValidator.For<ObjWithTwoPublicProps>(OnFirstPropertyValidatorFailed.Break);
			Assert.That(builder, Is.Not.Null);
		}

		[Test]
		[TestCase(true)]
		[TestCase(false)]
		public void Should_FullChain_Work_When_Validate(bool isValid)
		{
			int i = isValid ? 1 : -1;
			var result = ExpressValidator.For<ObjWithTwoPublicProps>()
						   .AddProperty(o => o.I)
						   .WithValidation(o => o.GreaterThan(0))
						   .Validate(new ObjWithTwoPublicProps() { I = i });
			ClassicAssert.AreEqual(isValid, result.IsValid);
		}

		[Test]
		[TestCase(true)]
		[TestCase(false)]
		public void Should_FullChain_Work_With_MultipleProperties(bool isValid)
		{
			var result = ExpressValidator.For<ObjWithTwoPublicProps>()
						   .AddProperty(o => o.I)
						   .WithValidation(o => o.GreaterThan(0))
						   .AddProperty(o => o.S)
						   .WithValidation(o => o.MaximumLength(1))
						   .Validate(new ObjWithTwoPublicProps() { I = isValid ? 1 : -1, S = isValid ? "b" : "ab" });
			ClassicAssert.AreEqual(isValid, result.IsValid);
		}

		[Test]
		public void Should_Break_Mode_Stop_At_First_Failure()
		{
			var result = ExpressValidator.For<ObjWithTwoPublicProps>(OnFirstPropertyValidatorFailed.Break)
						   .AddProperty(o => o.I)
						   .WithValidation(o => o.GreaterThan(0))
						   .AddProperty(o => o.S)
						   .WithValidation(o => o.MaximumLength(1))
						   .Validate(new ObjWithTwoPublicProps() { I = -1, S = "ab" });
			ClassicAssert.AreEqual(false, result.IsValid);
			ClassicAssert.AreEqual(1, result.Errors.Count);
		}

		[Test]
		public void Should_Continue_Mode_Collect_All_Failures()
		{
			var result = ExpressValidator.For<ObjWithTwoPublicProps>()
						   .AddProperty(o => o.I)
						   .WithValidation(o => o.GreaterThan(0))
						   .AddProperty(o => o.S)
						   .WithValidation(o => o.MaximumLength(1))
						   .Validate(new ObjWithTwoPublicProps() { I = -1, S = "ab" });
			ClassicAssert.AreEqual(false, result.IsValid);
			ClassicAssert.AreEqual(2, result.Errors.Count);
		}

		[Test]
		[TestCase(true)]
		[TestCase(false)]
		public void Should_FullChain_Work_With_Func(bool isValid)
		{
			var result = ExpressValidator.For<ObjWithTwoPublicProps>()
						   .AddFunc(o => o.PercentValue1 + o.PercentValue2, "percentSum")
						   .WithValidation(o => o.InclusiveBetween(0, 100))
						   .Validate(new ObjWithTwoPublicProps() { PercentValue1 = 20, PercentValue2 = isValid ? 80 : 82 });
			ClassicAssert.AreEqual(isValid, result.IsValid);
		}

		[Test]
		public void Should_Null_Object_Return_Failure()
		{
			var result = ExpressValidator.For<ObjWithTwoPublicProps>()
						   .AddProperty(o => o.I)
						   .WithValidation(o => o.GreaterThan(0))
						   .Validate(null);
			ClassicAssert.AreEqual(false, result.IsValid);
		}

		[Test]
		[TestCase(true)]
		[TestCase(false)]
		public async Task Should_Async_FullChain_Work(bool isValid)
		{
			int i = isValid ? 1 : -1;
			var result = await ExpressValidator.For<ObjWithTwoPublicProps>()
						   .AddProperty(o => o.I)
						   .WithAsyncValidation(o => o.GreaterThan(0))
						   .ValidateAsync(new ObjWithTwoPublicProps() { I = i });
			ClassicAssert.AreEqual(isValid, result.IsValid);
		}

		[Test]
		[TestCase(true)]
		[TestCase(false)]
		public void Should_FullChain_Work_With_Options(bool isValid)
		{
			var options = new ObjWithTwoPublicPropsOptions()
			{
				IGreaterThanValue = isValid ? -1 : 1,
			};

			var result = ExpressValidator.For<ObjWithTwoPublicProps, ObjWithTwoPublicPropsOptions>()
						   .AddProperty(o => o.I)
						   .WithValidation((to, opt) => opt.GreaterThan(to.IGreaterThanValue))
						   .Validate(new ObjWithTwoPublicProps() { I = 0 }, options);
			ClassicAssert.AreEqual(isValid, result.IsValid);
		}

		[Test]
		[TestCase(true)]
		[TestCase(false)]
		public async Task Should_Async_FullChain_Work_With_Options(bool isValid)
		{
			var options = new ObjWithTwoPublicPropsOptions()
			{
				IGreaterThanValue = isValid ? -1 : 1,
			};

			var result = await ExpressValidator.For<ObjWithTwoPublicProps, ObjWithTwoPublicPropsOptions>()
						   .AddProperty(o => o.I)
						   .WithAsyncValidation((to, opt) => opt.GreaterThan(to.IGreaterThanValue))
						   .ValidateAsync(new ObjWithTwoPublicProps() { I = 0 }, options);
			ClassicAssert.AreEqual(isValid, result.IsValid);
		}

		[Test]
		[TestCase(true)]
		[TestCase(false)]
		public void Should_BuildAndValidate_And_Validate_Produce_Same_Result(bool isValid)
		{
			int i = isValid ? 1 : -1;
			var obj = new ObjWithTwoPublicProps() { I = i };

			var result1 = ExpressValidator.For<ObjWithTwoPublicProps>()
							.AddProperty(o => o.I)
							.WithValidation(o => o.GreaterThan(0))
							.BuildAndValidate(obj);

			var result2 = ExpressValidator.For<ObjWithTwoPublicProps>()
							.AddProperty(o => o.I)
							.WithValidation(o => o.GreaterThan(0))
							.Validate(obj);

			ClassicAssert.AreEqual(result1.IsValid, result2.IsValid);
			ClassicAssert.AreEqual(result1.Errors.Count, result2.Errors.Count);
		}

		[Test]
		[TestCase(true)]
		[TestCase(false)]
		public async Task Should_BuildAndValidateAsync_And_ValidateAsync_Produce_Same_Result(bool isValid)
		{
			int i = isValid ? 1 : -1;
			var obj = new ObjWithTwoPublicProps() { I = i };

			var result1 = await ExpressValidator.For<ObjWithTwoPublicProps>()
							.AddProperty(o => o.I)
							.WithAsyncValidation(o => o.GreaterThan(0))
							.BuildAndValidateAsync(obj);

			var result2 = await ExpressValidator.For<ObjWithTwoPublicProps>()
							.AddProperty(o => o.I)
							.WithAsyncValidation(o => o.GreaterThan(0))
							.ValidateAsync(obj);

			ClassicAssert.AreEqual(result1.IsValid, result2.IsValid);
			ClassicAssert.AreEqual(result1.Errors.Count, result2.Errors.Count);
		}
	}
}
