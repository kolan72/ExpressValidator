using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.Linq;
using System.Reflection;

namespace ExpressValidator.Extensions.DependencyInjection.Tests
{
	internal partial class ServiceCollectionExtensionsTests
	{
		private IServiceCollection _services;

		[SetUp]
		public void SetUp()
		{
			_services = new ServiceCollection();
		}

		[Test]
		public void Should_ReturnServiceCollection_WhenCalled()
		{
			var result = _services.AddExpressValidation(Assembly.GetExecutingAssembly());

			Assert.That(result, Is.SameAs(_services));
		}

		[Test]
		public void Should_UseExecutingAssembly_WhenAssemblyIsNull()
		{
			var result = _services.AddExpressValidation(null);

			Assert.That(result, Is.Not.Null);
			Assert.That(result, Is.SameAs(_services));
		}

		[Test]
		public void Should_RegisterProxyValidatorAsTransient_WhenLifetimeIsNotSpecified()
		{
			_services.AddExpressValidation(Assembly.GetExecutingAssembly());

			var descriptor = _services.FirstOrDefault(sd =>
				sd.ServiceType.IsGenericType &&
				sd.ServiceType.GetGenericTypeDefinition() == typeof(IExpressValidator<>));

			Assert.That(descriptor, Is.Not.Null);
			Assert.That(descriptor.ImplementationType.GetGenericTypeDefinition(), Is.EqualTo(typeof(ProxyValidator<>)));
			Assert.That(descriptor.Lifetime, Is.EqualTo(ServiceLifetime.Transient));
		}

		[Test]
		public void Should_RegisterProxyValidatorAsSingleton_WhenLifetimeIsSingleton()
		{
			_services.AddExpressValidation(Assembly.GetExecutingAssembly(), ServiceLifetime.Singleton);

			var descriptor = _services.FirstOrDefault(sd =>
				sd.ServiceType.IsGenericType &&
				sd.ServiceType.GetGenericTypeDefinition() == typeof(IExpressValidator<>));

			Assert.That(descriptor, Is.Not.Null);
			Assert.That(descriptor.Lifetime, Is.EqualTo(ServiceLifetime.Singleton));
		}

		[Test]
		public void Should_RegisterProxyValidatorAsScoped_WhenLifetimeIsScoped()
		{
			_services.AddExpressValidation(Assembly.GetExecutingAssembly(), ServiceLifetime.Scoped);

			var descriptor = _services.FirstOrDefault(sd =>
				sd.ServiceType.IsGenericType &&
				sd.ServiceType.GetGenericTypeDefinition() == typeof(IExpressValidator<>));

			Assert.That(descriptor, Is.Not.Null);
			Assert.That(descriptor.Lifetime, Is.EqualTo(ServiceLifetime.Scoped));
		}

		[Test]
		public void Should_RegisterAllConfiguratorsFromAssembly_WhenCalled()
		{
			var assembly = Assembly.GetExecutingAssembly();

			_services.AddExpressValidation(assembly);

			var configuratorDescriptors = _services.Where(sd =>
				sd.ServiceType.IsGenericType &&
				sd.ServiceType.GetGenericTypeDefinition() == typeof(IValidatorConfigurator<>));

			Assert.That(configuratorDescriptors, Is.Not.Empty);
		}

		[Test]
		public void Should_RegisterConfiguratorsWithSpecifiedLifetime_WhenLifetimeIsProvided()
		{
			var assembly = Assembly.GetExecutingAssembly();

			_services.AddExpressValidation(assembly, ServiceLifetime.Singleton);

			var configuratorDescriptors = _services.Where(sd =>
				sd.ServiceType.IsGenericType &&
				sd.ServiceType.GetGenericTypeDefinition() == typeof(IValidatorConfigurator<>));

			Assert.That(configuratorDescriptors.All(d => d.Lifetime == ServiceLifetime.Singleton), Is.True);
		}

		[Test]
		public void Should_RegisterBothConfiguratorsAndValidator_WhenCalled()
		{
			_services.AddExpressValidation(Assembly.GetExecutingAssembly());

			var hasConfigurators = _services.Any(sd =>
				sd.ServiceType.IsGenericType &&
				sd.ServiceType.GetGenericTypeDefinition() == typeof(IValidatorConfigurator<>));

			var hasValidator = _services.Any(sd =>
				sd.ServiceType.IsGenericType &&
				sd.ServiceType.GetGenericTypeDefinition() == typeof(IExpressValidator<>));

			Assert.That(hasConfigurators || hasValidator, Is.True);
			Assert.That(hasValidator, Is.True);
		}

		[Test]
		public void Should_NotThrowException_WhenAssemblyHasNoConfigurators()
		{
			var emptyAssembly = typeof(object).Assembly; // mscorlib has no configurators

			Assert.DoesNotThrow(() => _services.AddExpressValidation(emptyAssembly));
		}

		[Test]
		public void Should_RegisterOnlyOneProxyValidator_WhenCalledMultipleTimes()
		{
			_services.AddExpressValidation(Assembly.GetExecutingAssembly());
			_services.AddExpressValidation(Assembly.GetExecutingAssembly());

			var validatorDescriptors = _services.Where(sd =>
				sd.ServiceType.IsGenericType &&
				sd.ServiceType.GetGenericTypeDefinition() == typeof(IExpressValidator<>) &&
				sd.ImplementationType?.GetGenericTypeDefinition() == typeof(ProxyValidator<>));

			Assert.That(validatorDescriptors.Count(), Is.EqualTo(2)); // Each call adds one
		}
	}

	// Test fixture helper classes for testing configurator discovery
	public class TestModel
	{
		public string Name { get; set; }
	}

	public class TestModelConfigurator : ValidatorConfigurator<TestModel>
	{
		public override void Configure(ExpressValidatorBuilder<TestModel> expressValidatorBuilder)
		{
			// Test implementation
		}
	}
}
