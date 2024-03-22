using FluentValidation;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressValidator.Tests
{
    internal partial class ExpressValidatorTests
    {
		[Test]
		public void Should_Validate_Throw_For_AsyncRules()
		{
			var builder = new ExpressValidatorBuilder<ObjWithTwoPublicProps>()
						   .AddProperty(o => o.I)
						   .WithAsyncValidation(o => o.MustAsync(async (_, __) => { await Task.Delay(1); return true; }))
						   .Build();
			var exc = Assert.Throws<InvalidOperationException>(() => builder.Validate(new ObjWithTwoPublicProps() { I = 1, S = "b" }));
			Assert.That(exc.Message, Does.StartWith("Object validator has a property with asynchronous validation rules."));
		}
	}
}
