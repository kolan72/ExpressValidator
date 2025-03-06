﻿using NUnit.Framework;
using System;
using FluentValidation;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressValidator.Tests
{
	internal class FluentPropertyValidatorTests
	{
		[Test]
		public void Should_Be_Initialized_Correctly_By_Ctor()
		{
			static decimal funcForField(Customer c) => c.CustomerDiscount;
			const string propName = "CustomerDiscount";
			var validator = new TypeValidator<decimal>();
			validator.SetValidation((opt) => opt.GreaterThan(0), propName);

			var fluentValidator = new FluentPropertyValidator<Customer, decimal>(funcForField, propName, validator);
			Assert.That(fluentValidator.Count(), Is.EqualTo(1));
			Assert.That(fluentValidator.FirstOrDefault().PropertyName, Is.EqualTo(propName));

			var notValidCustomer = new Customer();
			var failedValidationResult = fluentValidator.Validate(notValidCustomer);
			Assert.That(failedValidationResult.IsValid, Is.False);
			Assert.That(failedValidationResult.Errors.Count, Is.EqualTo(1));

			Assert.That(fluentValidator.Validate(new Customer() { CustomerDiscount = 1 }).IsValid, Is.True);
		}
	}
}
