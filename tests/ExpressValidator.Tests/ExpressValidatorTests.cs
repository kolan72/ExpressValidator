using FluentValidation;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ExpressValidator.Tests
{
	internal partial class ExpressValidatorTests
	{
		[Test]
		public void Should_Work_When_IsValid()
		{
			var result = new ExpressValidatorBuilder<ObjWithTwoPublicProps>()
						   .AddProperty(o => o.I)
						   .WithValidation(o => o.GreaterThan(0))
						   .AddProperty(o => o.S)
						   .WithValidation(o => o.MaximumLength(1))
						   .AddField(o=>o._sField)
						   .WithValidation(o => o.MinimumLength(1))
						   .AddFunc(o => o.PercentValue1 + o.PercentValue2, "percentSum")
						   .WithValidation(o => o.InclusiveBetween(0, 100))
						   .Build()
						   .Validate(new ObjWithTwoPublicProps() { I = 1, S = "b", _sField = "1", PercentValue1 = 20, PercentValue2 = 80 });
			ClassicAssert.AreEqual(true, result.IsValid);
		}

		[Test]
		public void Should_Work_When_IsValid_ForSubObjWithSimpleConditionForComplexProperty()
		{
			var result = new ExpressValidatorBuilder<SubObjWithComplexProperty>()
						   .AddProperty(o => o.I)
						   .WithValidation(o => o.GreaterThan(0))
						   .AddProperty(o => o.S)
						   .WithValidation(o => o.MaximumLength(1))
						   .AddProperty(o => o.Contact)
						   .WithValidation(o => o.Null())
						   .Build()
						   .Validate(new SubObjWithComplexProperty() { I = 1, S = "b"});
			ClassicAssert.AreEqual(true, result.IsValid);
		}

		[Test]
		public void Should_Work_When_NotValid_ForSubObjWithSimpleConditionForComplexProperty_WithFluentValidator_AndTwoPropsNotValid()
		{
			var result = new ExpressValidatorBuilder<SubObjWithComplexProperty>()
						   .AddProperty(o => o.I)
						   .WithValidation(o => o.GreaterThan(0))
						   .AddProperty(o => o.S)
						   .WithValidation(o => o.MaximumLength(1))
						   .AddProperty(o => o.Contact)
						   .WithValidation(o => o.SetValidator(new ContactValidator()))
						   .Build()
						   .Validate(new SubObjWithComplexProperty() { I = 2, S = "b", Contact = new Contact()});
			ClassicAssert.AreEqual(false, result.IsValid);
			ClassicAssert.AreEqual(2, result.Errors.Count);
		}

		[Test]
		public void Should_Work_When_NotValid_ForSubObjWithSimpleConditionForComplexProperty_WithFluentCollectionValidator_PropertyIsNotSet()
		{
			var result = new ExpressValidatorBuilder<SubObjWithComplexCollectionProperty>()
						   .AddProperty(o => o.I)
						   .WithValidation(o => o.GreaterThan(0))
						   .AddProperty(o => o.S)
						   .WithValidation(o => o.MaximumLength(1))
						   .AddProperty(o => o.Contacts)
						   .WithValidation(o => o.ForEach(o1 => o1.SetValidator(new ContactValidator())))
						   .Build()
						   .Validate(new SubObjWithComplexCollectionProperty() { I = 1, S = "b"});
			ClassicAssert.AreEqual(false, result.IsValid);
			ClassicAssert.AreEqual(1, result.Errors.Count);
		}

		[Test]
		public void Should_Work_When_NotValid_ForSubObjWithSimpleConditionForComplexProperty_WithFluentCollectionValidator_AndTwoObjectsWithTwoPropsNotValid()
		{
			var result = new ExpressValidatorBuilder<SubObjWithComplexCollectionProperty>()
					   .AddProperty(o => o.I)
					   .WithValidation(o => o.GreaterThan(0))
					   .AddProperty(o => o.S)
					   .WithValidation(o => o.MaximumLength(1))
					   .AddProperty(o => o.Contacts)
					   .WithValidation(o => o.NotEmpty().ForEach(o1 => o1.SetValidator(new ContactValidator())))
					   .Build()
					   .Validate(new SubObjWithComplexCollectionProperty() { I = 1, S = "b",
						   Contacts = new List<Contact>() { new Contact(), new Contact() } });
			ClassicAssert.AreEqual(false, result.IsValid);
			ClassicAssert.AreEqual(4, result.Errors.Count);
		}

		[Test]
		public void Should_Work_When_Valid_ForSubObjWithSimpleConditionForComplexProperty_WithFluentCollectionValidator()
		{
			var result = new ExpressValidatorBuilder<SubObjWithComplexCollectionProperty>()
					   .AddProperty(o => o.I)
					   .WithValidation(o => o.GreaterThan(0))
					   .AddProperty(o => o.S)
					   .WithValidation(o => o.MaximumLength(1))
					   .AddProperty(o => o.Contacts)
					   .WithValidation(o => o.ForEach(o1 => o1.SetValidator(new ContactValidator())))
					   .Build()
					   .Validate(new SubObjWithComplexCollectionProperty()
					   {
						   I = 1,
						   S = "b",
						   Contacts = new List<Contact>() { new Contact(){ Email = "", Name = "" }, new Contact(){ Email = "", Name = "" }}
					   });
			ClassicAssert.AreEqual(true, result.IsValid);
		}

		[Test]
		[TestCase(OnFirstPropertyValidatorFailed.Break)]
		[TestCase(OnFirstPropertyValidatorFailed.Continue)]
		public void Should_Work_When_NotValid(OnFirstPropertyValidatorFailed validationMode)
		{
			var result = new ExpressValidatorBuilder<ObjWithTwoPublicProps>(validationMode)
						   .AddProperty(o => o.I)
						   .WithValidation(o => o.GreaterThan(0))
						   .AddProperty(o => o.S)
						   .WithValidation(o => o.MaximumLength(1))
						   .AddField(o => o._sField)
						   .WithValidation(o => o.MinimumLength(1))
						   .AddFunc(o => o.PercentValue1 + o.PercentValue2, "percentSum")
						   .WithValidation(o => o.InclusiveBetween(0, 100))
						   .Build()
						   .Validate(new ObjWithTwoPublicProps() { I = -1, S = "ab", _sField = "", PercentValue1 = 2, PercentValue2 = 101});
			ClassicAssert.AreEqual(false, result.IsValid);
			if (validationMode == OnFirstPropertyValidatorFailed.Break)
			{
				ClassicAssert.AreEqual(1, result.Errors.Count);
			}
			else
			{
				ClassicAssert.AreEqual(4, result.Errors.Count);
			}
		}

		[Test]
		[TestCase(true)]
		[TestCase(false)]
		public void Should_Work_When_Nullable_And_Value_Prop_Values_Validated_WithNullValidator(bool propValueIsNull)
		{
			ObjWithTwoPublicProps objToTest = null;

			if (propValueIsNull)
			{
				objToTest = new ObjWithTwoPublicProps() { I = -1 };
			}
			else
			{
				objToTest = new ObjWithTwoPublicProps() { I = -1, S = "ab" };
			}
			var result = new ExpressValidatorBuilder<ObjWithTwoPublicProps>()
					   .AddProperty(o => o.I)
					   .WithValidation(o => o.GreaterThan(0))
					   .AddProperty(o => o.S)
					   .WithValidation(o => o.Null())
					   .Build()
					   .Validate(objToTest);

			if (propValueIsNull)
			{
				ClassicAssert.AreEqual(1, result.Errors.Count);
			}
			else
			{
				ClassicAssert.AreEqual(2, result.Errors.Count);
			}
		}

		[Test]
		public void Should_Throw_When_Non_Property()
		{
			Assert.Throws<ArgumentException>
						(() => new ExpressValidatorBuilder<ObjWithTwoPublicProps>()
						.AddProperty(o => o)
						.WithValidation(o => o.NotNull())
						.Build());
		}

		[Test]
		public void Should_Throw_When_Non_Field()
		{
			Assert.Throws<ArgumentException>
						(() => new ExpressValidatorBuilder<ObjWithTwoPublicProps>()
						.AddField(o => o)
						.WithValidation(o => o.NotNull())
						.Build());
		}

		[Test]
		public void Should_IsValid_Equals_True_WhenNoValidators()
		{
			var result = new ExpressValidatorBuilder<ObjWithTwoPublicProps>()
							.Build()
							.Validate(new ObjWithTwoPublicProps());
			Assert.That(result.IsValid, Is.True);
		}

		[Test]
		[TestCase(true)]
		[TestCase(false)]
		public void Should_Work_When_TheSamePropertyValidators_In_A_Row(bool isValid)
		{
			int i  = isValid ? 3 : -1;
			var validator = new ExpressValidatorBuilder<ObjWithTwoPublicProps>()
							.AddProperty(o => o.I)
							.WithValidation(o => o.GreaterThan(1))
							.AddProperty(o => o.I)
							.WithValidation(o => o.GreaterThan(2))
							.Build();

			var result = validator.Validate(new ObjWithTwoPublicProps() { I = i });
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
		public void Should_Preserve_Property_Name(SetPropertyNameType setPropertyNameType, MemberTypes memberTypes)
		{
			var builder = new ExpressValidatorBuilder<ObjWithTwoPublicProps>();
			IBuilderWithPropValidator<ObjWithTwoPublicProps, int> builderWithProperty = null;

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
					builder = builderWithProperty.WithValidation(o => o.GreaterThan(0));
					break;
				case SetPropertyNameType.Override:
					builder = builderWithProperty.WithValidation(o => o.GreaterThan(0).OverridePropertyName("TestPropName"));
					break;
				case SetPropertyNameType.WithName:
					builder = builderWithProperty.WithValidation(o => o.GreaterThan(0).WithName("TestName"));
					break;
			}
			var result = builder.Build().Validate(new ObjWithTwoPublicProps() { I = -1 });

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
		[TestCase(SetPropertyNameType.WithName)]
		[TestCase(SetPropertyNameType.NotSetExplicitly)]
		[TestCase(SetPropertyNameType.Override)]
		public void Should_AddFunc_Preserve_Property_Name(SetPropertyNameType setPropertyNameType)
		{
			var builder = new ExpressValidatorBuilder<ObjWithTwoPublicProps>();
			var builderWithProperty = builder.AddFunc(o => o.PercentValue1 + o.PercentValue2, "percentSum");

			switch (setPropertyNameType)
			{
				case SetPropertyNameType.NotSetExplicitly:
					builder = builderWithProperty.WithValidation(o => o.InclusiveBetween(0, 100));
					break;
				case SetPropertyNameType.Override:
					builder = builderWithProperty.WithValidation(o => o.InclusiveBetween(0, 100)
													.OverridePropertyName("TestPropName"));
					break;
				case SetPropertyNameType.WithName:
					builder = builderWithProperty.WithValidation(o => o.InclusiveBetween(0, 100)
													.WithName("TestName"));
					break;
			}

			var result = builder.Build().Validate(new ObjWithTwoPublicProps() { PercentValue1 = 1, PercentValue2 = 100 });

			Assert.That(result.IsValid, Is.False);

			switch (setPropertyNameType)
			{
				case SetPropertyNameType.NotSetExplicitly:
				case SetPropertyNameType.WithName:
					Assert.That(result.Errors.FirstOrDefault().PropertyName, Is.EqualTo("percentSum"));
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
