using FluentValidation;
using NUnit.Framework;
using System;

namespace ExpressValidator.Extensions.DependencyInjection.Tests
{
	internal class ExpressValidatorWithReloadTests
	{
		[Test]
		public void Should_ExpressValidatorWithReload_ValidatorParameters_Change_When_OptionsMonitorContext_Change()
		{
			var builder = new ExpressValidatorBuilder<ObjToValidate, ObjectToValidateOptions>()
							.AddProperty(o => o.I)
							.WithValidation((opts, rule) => rule.GreaterThan(opts.IGreaterThanValue));

			var optsBeforeChange = new ObjectToValidateOptions() { IGreaterThanValue = 5 };

			var context = new TestOptionsMonitorContext() {LastUpdated = DateTimeOffset.UtcNow, Options = optsBeforeChange };

			var reloadableValidator = new ExpressValidatorWithReload<ObjToValidate, ObjectToValidateOptions>(builder, context);

			var objToTest = new ObjToValidate() { I = 4 };

			var result = reloadableValidator.Validate(objToTest);

			Assert.That(result.IsValid, Is.False);

			//Change options in context and validate again
			context.LastUpdated += TimeSpan.FromMilliseconds(1);
			context.Options = new ObjectToValidateOptions { IGreaterThanValue = 3 };

			result = reloadableValidator.Validate(objToTest);
			Assert.That(result.IsValid, Is.True);
		}

		private class TestOptionsMonitorContext : IOptionsMonitorContext<ObjectToValidateOptions>
		{
			public DateTimeOffset LastUpdated { get; set; }
			public ObjectToValidateOptions Options { get; set; }
		}
	}
}
