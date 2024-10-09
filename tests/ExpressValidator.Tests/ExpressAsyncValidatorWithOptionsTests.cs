using FluentValidation;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ExpressValidator.Tests
{
	internal class ExpressAsyncValidatorWithOptionsTests
	{
		private readonly ObjWithTwoPublicPropsOptions _objWithTwoPublicPropsOptions = new ObjWithTwoPublicPropsOptions()
		{
			IGreaterThanValue = 0,
			SMaximumLengthValue = 1,
			SFieldMaximumLengthValue = 1
		};

		[Test]
		public async Task Should_Work_When_IsValid()
		{
			var result = await new ExpressValidatorBuilder<ObjWithTwoPublicProps, ObjWithTwoPublicPropsOptions>()
						   .AddProperty(o => o.I)
						   .WithAsyncValidation((to, p) => p.GreaterThan(to.IGreaterThanValue).MustAsync(async (_, __) => { await Task.Delay(1); return true;}))
						   .AddProperty(o => o.S)
						   .WithAsyncValidation((to, p) => p.MaximumLength(to.SMaximumLengthValue).MustAsync(async (_, __) => { await Task.Delay(1); return true;}))
						   .AddField(o => o._sField)
						   .WithAsyncValidation((to, f) => f.MaximumLength(to.SFieldMaximumLengthValue).MustAsync(async (_, __) => { await Task.Delay(1); return true; }))
						   .Build(_objWithTwoPublicPropsOptions)
						   .ValidateAsync(new ObjWithTwoPublicProps() { I = 1, S = "b", _sField = "1" });
			ClassicAssert.AreEqual(true, result.IsValid);
		}

		[Test]
		public async Task Should_Validation_Result_Change_When_Options_Change()
		{
			var builder = new ExpressValidatorBuilder<ObjWithTwoPublicProps, ObjWithTwoPublicPropsOptions>()
						   .AddProperty(o => o.I)
						   .WithAsyncValidation((to, p) => p.GreaterThan(to.IGreaterThanValue).MustAsync(async (_, __) => { await Task.Delay(1); return true; }))
						   .AddProperty(o => o.S)
						   .WithAsyncValidation((to, p) => p.MaximumLength(to.SMaximumLengthValue).MustAsync(async (_, __) => { await Task.Delay(1); return true; }))
						   .AddField(o => o._sField)
						   .WithAsyncValidation((to, f) => f.MaximumLength(to.SFieldMaximumLengthValue).MustAsync(async (_, __) => { await Task.Delay(1); return true; }));

			var result1 = await builder.Build(_objWithTwoPublicPropsOptions)
								.ValidateAsync(new ObjWithTwoPublicProps() { I = 1, S = "b", _sField = "1" });
			ClassicAssert.AreEqual(true, result1.IsValid);

			var options2 = new ObjWithTwoPublicPropsOptions() { IGreaterThanValue = 2, SMaximumLengthValue = 2, SFieldMaximumLengthValue = 1 };
			var result2 = await builder.Build(options2)
								.ValidateAsync(new ObjWithTwoPublicProps() { I = 1, S = "abc", _sField = "12" });
			ClassicAssert.AreEqual(false, result2.IsValid);
			ClassicAssert.AreEqual(3, result2.Errors.Count);

			var options3 = new ObjWithTwoPublicPropsOptions() { IGreaterThanValue = 3, SMaximumLengthValue = 3, SFieldMaximumLengthValue = 2 };
			var result3 = await builder.Build(options3)
								.ValidateAsync(new ObjWithTwoPublicProps() { I = 2, S = "abcd", _sField = "123" });
			ClassicAssert.AreEqual(false, result3.IsValid);
			ClassicAssert.AreEqual(3, result3.Errors.Count);
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

		[Test]
		[TestCase(SetPropertyNameType.WithName, MemberTypes.Property)]
		[TestCase(SetPropertyNameType.NotSetExplicitly, MemberTypes.Property)]
		[TestCase(SetPropertyNameType.Override, MemberTypes.Property)]
		[TestCase(SetPropertyNameType.WithName, MemberTypes.Field)]
		[TestCase(SetPropertyNameType.NotSetExplicitly, MemberTypes.Field)]
		[TestCase(SetPropertyNameType.Override, MemberTypes.Field)]
		public async Task Should_Preserve_Property_Name(SetPropertyNameType setPropertyNameType, MemberTypes memberTypes)
		{
			var builder = new ExpressValidatorBuilder<ObjWithTwoPublicProps, ObjWithTwoPublicPropsOptions>();
			IBuilderWithPropValidator<ObjWithTwoPublicProps, ObjWithTwoPublicPropsOptions, int> builderWithProperty = null;

			if (memberTypes == MemberTypes.Property)
			{
				builderWithProperty = builder.AddProperty(o => o.I);
			}
			else
			{
				builderWithProperty = builder.AddField(o => o._iField);
			}

			switch (setPropertyNameType)
			{
				case SetPropertyNameType.NotSetExplicitly:
					builder = builderWithProperty.WithAsyncValidation((topt, p) => p.GreaterThan(topt.IGreaterThanValue).MustAsync(async (_, __) => { await Task.Delay(1); return true; }));
					break;
				case SetPropertyNameType.Override:
					builder = builderWithProperty.WithAsyncValidation((topt, p) => p.GreaterThan(topt.IGreaterThanValue).MustAsync(async (_, __) => { await Task.Delay(1); return true; })
													.OverridePropertyName("TestPropName"));
					break;
				case SetPropertyNameType.WithName:
					builder = builderWithProperty.WithAsyncValidation((topt, p) => p.GreaterThan(topt.IGreaterThanValue).MustAsync(async (_, __) => { await Task.Delay(1); return true; })
													.WithName("TestName"));
					break;
			}
			var result = await builder.Build(_objWithTwoPublicPropsOptions).ValidateAsync(new ObjWithTwoPublicProps());

			Assert.That(result.IsValid, Is.False);

			switch (setPropertyNameType)
			{
				case SetPropertyNameType.NotSetExplicitly:
				case SetPropertyNameType.WithName:
					Assert.That(result.Errors.FirstOrDefault().PropertyName, memberTypes == MemberTypes.Property ? Is.EqualTo("I") : Is.EqualTo("_iField"));
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

		[Test]
		public async Task Should_IsValid_Equals_True_WhenNoValidators()
		{
			var result = await new ExpressValidatorBuilder<ObjWithTwoPublicProps, ObjWithTwoPublicPropsOptions>()
							.Build(_objWithTwoPublicPropsOptions)
							.ValidateAsync(new ObjWithTwoPublicProps());
			Assert.That(result.IsValid, Is.True);
		}
	}
}
