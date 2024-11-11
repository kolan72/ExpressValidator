using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using FluentValidation;

namespace ExpressValidator.Extensions.DependencyInjection.Tests
{
	internal class ServiceCollectionExtensionsTests
    {
        [Test]
        public void Should_AddExpressValidator_Register()
        {
            var services = new ServiceCollection();
            services.AddExpressValidator<ObjectToValidate>((builder) =>
                                                            builder.AddProperty(o => o.I)
                                                            .WithValidation((v) => v.GreaterThan(1)));
            var serviceProvider = services.BuildServiceProvider();

            var service = serviceProvider.GetService<IExpressValidator<ObjectToValidate>>();
            Assert.That(service, Is.Not.Null);
        }

        [Test]
        public void Should_AddExpressValidatorBuilder_Register()
        {
            var services = new ServiceCollection();
            services.AddExpressValidatorBuilder<ObjectToValidate, ObjectToValidateOptions>((builder) =>
                                                            builder
                                                            .AddProperty(o => o.I)
                                                            .WithValidation((to, v) => v.GreaterThan(to.IGreaterThanValue)));
            var serviceProvider = services.BuildServiceProvider();

            var service = serviceProvider.GetService<IExpressValidatorBuilder<ObjectToValidate, ObjectToValidateOptions>>();
            Assert.That(service, Is.Not.Null);
        }

        public class ObjectToValidate
        {
            public int I { get; }
        }

        public class ObjectToValidateOptions
        {
            public int IGreaterThanValue { get; set; }
        }
    }
}
