using ExpressValidator.CollectionValidation;
using FluentValidation;
using NUnit.Framework;
using System.Collections.Generic;

namespace ExpressValidator.Tests
{
	internal class CollectionValidatorTests
	{
		[Test]
		public void Should_Fail_With_Expected_Errors()
		{
			var Contacts = new List<Contact>() { new Contact(), new Contact() };
			var result = CollectionValidator.Validate(Contacts,
				(eb) => eb.AddProperty(c => c.Name)
						.WithValidation(o => o.NotNull())
						.AddProperty(c => c.Email)
						.WithValidation(o => o.NotNull()),
				"contacts");
			Assert.That(result.IsValid, Is.False);
			Assert.That(result.Errors.Count, Is.EqualTo(4));
		}
	}
}
