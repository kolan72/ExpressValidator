using FluentValidation;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ExpressValidator.Tests
{
	internal class FluentValidatorTests
	{
		[Test]
		public void Should_Be_Correctly_Initialized_By_FluentPropertyValidators()
		{
			static decimal funcForDiscount(Customer c) => c.CustomerDiscount;
			const string propNameDiscount = "CustomerDiscount";
			var validator = new TypeValidator<decimal>();
			validator.SetValidation((opt) => opt.GreaterThan(0), propNameDiscount);

			var propForDiscount = new FluentPropertyValidator<Customer, decimal>(funcForDiscount, propNameDiscount, validator);

			static int funcForId(Customer c) => c.CustomerId;
			const string propNameId = "CustomerId";
			var validatorId = new TypeValidator<int>();
			validatorId.SetValidation((opt) => opt.GreaterThan(1), propNameId);

			var propForId = new FluentPropertyValidator<Customer, int>(funcForId, propNameId, validatorId);

			static string funcForName(Customer c) => c.Name;
			const string propNameName = "Name";
			var nameValidator = new TypeValidator<string>();
			nameValidator.SetValidation((opt) => opt.Length(2), propNameName);

			var propForName = new FluentPropertyValidator<Customer, string>(funcForName, propNameName, nameValidator);

			var fv = new FluentValidator<Customer>(new List<AbstractValidator<Customer>>() { propForDiscount, propForId, propForName });
			Assert.That(fv.Count(), Is.EqualTo(3));

			var vr = fv.Validate(new Customer() );

			Assert.That(vr.IsValid, Is.False);
			Assert.That(vr.Errors.Count, Is.EqualTo(3));
		}

		[Test]
		public void Should_Apply_FluentValidator_To_Each_Item_In_Collection()
		{
			static string funcForName(Contact c) => c.Name;
			const string propName = "Name";
			var nameValidator = new TypeValidator<string>();
			nameValidator.SetValidation((opt) => opt.Length(2), propName);

			var propForName = new FluentPropertyValidator<Contact, string>(funcForName, propName, nameValidator);

			static string funcForEmail(Contact c) => c.Email;
			const string propEmail = "Email";
			var emailValidator = new TypeValidator<string>();
			emailValidator.SetValidation((opt) => opt.EmailAddress(), propEmail);

			var propForEmail = new FluentPropertyValidator<Contact, string>(funcForEmail, propEmail, emailValidator);

			var fv = new FluentValidator<Contact>(new List<AbstractValidator<Contact>>() { propForName, propForEmail });
			Assert.That(fv.Count(), Is.EqualTo(2));

			var result = new ExpressValidatorBuilder<SubObjWithComplexCollectionProperty>()
					   .AddProperty(o => o.Contacts)
					   .WithValidation(o => o.ForEach(o1 => o1.SetValidator(fv)))
					   .Build()
					   .Validate(new SubObjWithComplexCollectionProperty()
					   {
						   I = 1,
						   S = "b",
						   Contacts = new List<Contact>() { new Contact() { Name = "A", Email = "a"}, new Contact() { Name = "K", Email = "b"} }
					   });
			Assert.That(result.IsValid, Is.False);
			Assert.That(result.Errors.Count, Is.EqualTo(4));
		}
	}
}
