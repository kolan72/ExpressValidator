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
	}
}
