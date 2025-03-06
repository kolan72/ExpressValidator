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
			const string propName = "CustomerDiscount";
			var validator = new TypeValidator<decimal>();
			validator.SetValidation((opt) => opt.GreaterThan(0), propName);

			var propForDiscount = new FluentPropertyValidator<Customer, decimal>(funcForDiscount, propName, validator);

			static int funcForId(Customer c) => c.CustomerId;
			const string propNameId = "CustomerId";
			var validatorId = new TypeValidator<int>();
			validatorId.SetValidation((opt) => opt.GreaterThan(1), propNameId);

			var propForId = new FluentPropertyValidator<Customer, int>(funcForId, propName, validatorId);

			var fv = new FluentValidator<Customer>(new List<AbstractValidator<Customer>>() { propForDiscount, propForId });
			Assert.That(fv.Count(), Is.EqualTo(2));

			var vr = fv.Validate(new Customer());

			Assert.That(vr.IsValid, Is.False);
			Assert.That(vr.Errors.Count, Is.EqualTo(2));
		}
	}
}
