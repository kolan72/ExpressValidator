using Microsoft.Extensions.Options;
using NUnit.Framework;
using System;

namespace ExpressValidator.Extensions.DependencyInjection.Tests
{
	internal class OptionsMonitorContextTests
	{
        [Test]
		public void Should_OptionsMonitorContext_Be_Initialized_Correctly()
		{
            var context = new OptionsMonitorContext<ObjectToValidateOptions>(new TestOptionsMonitor(new ObjectToValidateOptions()));
            Assert.That(context.Options, Is.Not.Null);
            Assert.That(context.LastUpdated, Is.GreaterThan(DateTimeOffset.MinValue));
        }

        public class TestOptionsMonitor : IOptionsMonitor<ObjectToValidateOptions>
        {
            public TestOptionsMonitor(ObjectToValidateOptions currentValue)
            {
                CurrentValue = currentValue;
            }

            public ObjectToValidateOptions Get(string name)
            {
                return CurrentValue;
            }

            public IDisposable OnChange(Action<ObjectToValidateOptions, string> listener)
            {
                return new EmptyDisposable();
            }

            public ObjectToValidateOptions CurrentValue { get; }

			[System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S3881:\"IDisposable\" should be implemented correctly", Justification = "<Pending>")]
			public class EmptyDisposable : IDisposable
			{
                public void Dispose()
                {
                    // no op
                }
            }
		}
    }
}
