﻿using FluentValidation;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using System;
using System.Collections.Generic;
using System.Linq;

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
						   .Build()
						   .Validate(new ObjWithTwoPublicProps() { I = 1, S = "b" });
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
						   .Build()
						   .Validate(new ObjWithTwoPublicProps() { I = -1, S = "ab" });
			ClassicAssert.AreEqual(false, result.IsValid);
			if (validationMode == OnFirstPropertyValidatorFailed.Break)
			{
				ClassicAssert.AreEqual(1, result.Errors.Count);
			}
			else
			{
				ClassicAssert.AreEqual(2, result.Errors.Count);
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
		public void Should_IsValid_Equals_True_WhenNoValidators()
		{
			var result = new ExpressValidatorBuilder<ObjWithTwoPublicProps>()
							.Build()
							.Validate(new ObjWithTwoPublicProps());
			Assert.That(result.IsValid, Is.True);
		}

		[Test]
		[TestCase(SetPropertyNameType.WithName)]
		[TestCase(SetPropertyNameType.NotSetExplicitly)]
		[TestCase(SetPropertyNameType.Override)]
		public void Should_Preserve_Property_Name(SetPropertyNameType setPropertyNameType)
		{
			var builder = new ExpressValidatorBuilder<ObjWithTwoPublicProps>();
			var builderWithProperty = builder.AddProperty(o => o.I);

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
