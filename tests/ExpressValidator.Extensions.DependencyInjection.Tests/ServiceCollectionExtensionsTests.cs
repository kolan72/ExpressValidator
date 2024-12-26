using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using FluentValidation;
using System.IO;
using Microsoft.Extensions.Configuration;

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

        [Test]
        public void Should_AddExpressValidatorWithReload_Register()
        {
			var root = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
				.AddEnvironmentVariables()
                .Build();

			var services = new ServiceCollection();
            services.AddSingleton<IConfiguration>(root);        
            //HACK
            services.Configure<ObjectToValidateOptions>((opt) => root.GetSection("ObjectToValidateOptions").Bind(opt));

            services.AddExpressValidatorWithReload<ObjectToValidate, ObjectToValidateOptions>((builder) =>
                                                            builder
                                                            .AddProperty(o => o.I)
                                                            .WithValidation((to, v) => v.GreaterThan(to.IGreaterThanValue)), "ObjectToValidateOptions");
            var serviceProvider = services.BuildServiceProvider();
            var validator = serviceProvider.GetService<IExpressValidatorWithReload<ObjectToValidate>>();

            var resValid = validator.Validate(new ObjectToValidate() { I = 1 });

            Assert.That(resValid.IsValid, Is.False);
        }

        public class ObjectToValidate
        {
            public int I { get; set; }
        }

        public class ObjectToValidateOptions
        {
            public int IGreaterThanValue { get; set; }
        }
    }
}
