using FluentValidation;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ExpressValidator.Tests
{
	internal class ExpressAsyncValidatorTests
	{
		[Test]
		public async Task Should_Work_For_When_IsValid_Eq_True_And_AllValidatorsSync()
		{
			var result = await new ExpressValidatorBuilder<ObjWithTwoPublicProps>()
						   .AddProperty(o => o.I)
						   .WithValidation(o => o.GreaterThan(0))
						   .AddProperty(o => o.S)
						   .WithValidation(o => o.MaximumLength(1))
						   .Build()
						   .ValidateAsync(new ObjWithTwoPublicProps() { I = 2, S = "b" });
			ClassicAssert.AreEqual(true, result.IsValid);
		}

		[Test]
		public void Should_ValidateAsyncThrow_If_Cancellation_Occurs()
		{
			using (var ctSource = new CancellationTokenSource())
			{
				ctSource.Cancel();
				var builder = new ExpressValidatorBuilder<ObjWithTwoPublicProps>()
					   .AddProperty(o => o.I)
					   .WithValidation(o => o.GreaterThan(0))
					   .Build();

				Assert.ThrowsAsync<OperationCanceledException>(async () => await builder.ValidateAsync(new ObjWithTwoPublicProps() { I = 2, S = "b" }, ctSource.Token));
			}
		}

		[Test]
		public async Task  Should_IsValid_Equals_True_WhenNoValidators()
		{
			var result = await new ExpressValidatorBuilder<ObjWithTwoPublicProps>()
							.Build()
							.ValidateAsync(new ObjWithTwoPublicProps());
			Assert.That(result.IsValid, Is.True);
		}

		[Test]
		[TestCase(SetPropertyNameType.WithName)]
		[TestCase(SetPropertyNameType.NotSetExplicitly)]
		[TestCase(SetPropertyNameType.Override)]
		public async Task Should_Preserve_Property_Name(SetPropertyNameType setPropertyNameType)
		{
			var builder = new ExpressValidatorBuilder<ObjWithTwoPublicProps>();
			var builderWithProperty = builder.AddProperty(o => o.I);

			switch (setPropertyNameType)
			{
				case SetPropertyNameType.NotSetExplicitly:
					builder = builderWithProperty.WithAsyncValidation(o => o.GreaterThan(0).MustAsync(async(_, __) => { await Task.Delay(1); return true; }));
					break;
				case SetPropertyNameType.Override:
					builder = builderWithProperty.WithAsyncValidation(o => o.GreaterThan(0).MustAsync(async (_, __) => { await Task.Delay(1); return true; })
													.OverridePropertyName("TestPropName"));
					break;
				case SetPropertyNameType.WithName:
					builder = builderWithProperty.WithAsyncValidation(o => o.GreaterThan(0).MustAsync(async (_, __) => { await Task.Delay(1); return true; })
													.WithName("TestName"));
					break;
			}
			var result = await builder.Build().ValidateAsync(new ObjWithTwoPublicProps() { I = -1 });

			Assert.That(result.IsValid, Is.False);

			switch (setPropertyNameType)
			{
				case SetPropertyNameType.NotSetExplicitly:
				case SetPropertyNameType.WithName:
					Assert.That(result.Errors.FirstOrDefault().PropertyName, Is.EqualTo("I"));
					break;
				case SetPropertyNameType.Override:
					Assert.That(result.Errors.FirstOrDefault().PropertyName, Is.EqualTo("TestPropName"));
					break;
			}

			if (setPropertyNameType == SetPropertyNameType.WithName)
			{
				Assert.That(result.Errors.FirstOrDefault().ErrorMessage, Does.Contain("TestName"));
			}
		}
	}
}
