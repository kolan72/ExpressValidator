using FluentValidation;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using System;
using System.Linq;
using System.Reflection;

namespace ExpressValidator.Tests
{
	internal class ExpressValidatorWithOptonsTests
	{
		private readonly ObjWithTwoPublicPropsOptions _objWithTwoPublicPropsOptions = new ObjWithTwoPublicPropsOptions()
		{
			IGreaterThanValue = 0,
			SMaximumLengthValue = 1,
			SFieldMaximumLengthValue = 1
		};

		[Test]
		public void Should_Work_When_IsValid()
		{
			var result = new ExpressValidatorBuilder<ObjWithTwoPublicProps, ObjWithTwoPublicPropsOptions>()
						   .AddProperty(o => o.I)
						   .WithValidation((to, p) => p.GreaterThan(to.IGreaterThanValue))
						   .AddProperty(o => o.S)
						   .WithValidation((to, p)=> p.MaximumLength(to.SMaximumLengthValue))
						   .AddField(o => o._sField)
						   .WithValidation((to, f) => f.MaximumLength(to.SFieldMaximumLengthValue))
						   .Build(_objWithTwoPublicPropsOptions)
						   .Validate(new ObjWithTwoPublicProps() { I = 1, S = "b", _sField = "1"});
			ClassicAssert.AreEqual(true, result.IsValid);
		}

		[Test]
		public void Should_Validation_Result_Change_When_Options_Change()
		{
			var builder = new ExpressValidatorBuilder<ObjWithTwoPublicProps, ObjWithTwoPublicPropsOptions>()
						   .AddProperty(o => o.I)
						   .WithValidation((to, p) => p.GreaterThan(to.IGreaterThanValue))
						   .AddProperty(o => o.S)
						   .WithValidation((to, p) => p.MaximumLength(to.SMaximumLengthValue))
						   .AddField(o => o._sField)
						   .WithValidation((to, f) => f.MaximumLength(to.SFieldMaximumLengthValue));

			var result1 = builder.Build(_objWithTwoPublicPropsOptions)
								.Validate(new ObjWithTwoPublicProps() { I = 1, S = "b", _sField = "1"});
			ClassicAssert.AreEqual(true, result1.IsValid);

			var options2 = new ObjWithTwoPublicPropsOptions() { IGreaterThanValue = 2, SMaximumLengthValue = 2, SFieldMaximumLengthValue = 1 };
			var result2 = builder.Build(options2)
								.Validate(new ObjWithTwoPublicProps() { I = 1, S = "abc", _sField = "12"});
			ClassicAssert.AreEqual(false, result2.IsValid);
			ClassicAssert.AreEqual(3, result2.Errors.Count);

			var options3 = new ObjWithTwoPublicPropsOptions() { IGreaterThanValue = 3, SMaximumLengthValue = 3, SFieldMaximumLengthValue = 1 };
			var result3 = builder.Build(options3)
								.Validate(new ObjWithTwoPublicProps() { I = 2, S = "abcd", _sField = "123" });
			ClassicAssert.AreEqual(false, result3.IsValid);
			ClassicAssert.AreEqual(3, result3.Errors.Count);
		}

		[Test]
		[TestCase(SetPropertyNameType.WithName, MemberTypes.Property)]
		[TestCase(SetPropertyNameType.NotSetExplicitly, MemberTypes.Property)]
		[TestCase(SetPropertyNameType.Override, MemberTypes.Property)]
		[TestCase(SetPropertyNameType.WithName, MemberTypes.Field)]
		[TestCase(SetPropertyNameType.NotSetExplicitly, MemberTypes.Field)]
		[TestCase(SetPropertyNameType.Override, MemberTypes.Field)]
		public void Should_Preserve_Property_Name(SetPropertyNameType setPropertyNameType, MemberTypes memberTypes)
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
					builder = builderWithProperty.WithValidation((to, p) => p.GreaterThan(to.IGreaterThanValue));
					break;
				case SetPropertyNameType.Override:
					builder = builderWithProperty.WithValidation((to, p) => p.GreaterThan(to.IGreaterThanValue).OverridePropertyName("TestPropName"));
					break;
				case SetPropertyNameType.WithName:
					builder = builderWithProperty.WithValidation((to, p) => p.GreaterThan(to.IGreaterThanValue).WithName("TestName"));
					break;
			}
			var result = builder.Build(_objWithTwoPublicPropsOptions).Validate(new ObjWithTwoPublicProps() { I = -1 });

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
		public void Should_IsValid_Equals_True_WhenNoValidators()
		{
			var result = new ExpressValidatorBuilder<ObjWithTwoPublicProps, ObjWithTwoPublicPropsOptions>()
							.Build(_objWithTwoPublicPropsOptions)
							.Validate(new ObjWithTwoPublicProps());
			Assert.That(result.IsValid, Is.True);
		}

		[Test]
		public void Should_Throw_When_Non_Property()
		{
			Assert.Throws<ArgumentException>
						(() => new ExpressValidatorBuilder<ObjWithTwoPublicProps, ObjWithTwoPublicPropsOptions>()
						.AddProperty(o => o)
						.WithValidation((_, p) => p.NotNull())
						.Build(_objWithTwoPublicPropsOptions));
		}

		[Test]
		public void Should_Throw_When_Non_Field()
		{
			Assert.Throws<ArgumentException>
						(() => new ExpressValidatorBuilder<ObjWithTwoPublicProps, ObjWithTwoPublicPropsOptions>()
						.AddField(o => o)
						.WithValidation((_, p) => p.NotNull())
						.Build(_objWithTwoPublicPropsOptions));
		}
	}
}
