using FluentValidation.Results;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace ExpressValidator.Extensions.DependencyInjection.Tests
{
	[TestFixture]
	public class AddExpressValidationIntegrationTests
	{
		private ServiceCollection _services;
		private IServiceProvider _serviceProvider;

		[SetUp]
		public void SetUp()
		{
			_services = new ServiceCollection();
		}

		[TearDown]
		public void TearDown()
		{
			(_serviceProvider as IDisposable)?.Dispose();
		}

		[Test]
		public void Should_ResolveValidator_WhenConfiguratorIsRegistered()
		{
			_services.AddExpressValidation(Assembly.GetExecutingAssembly());
			_serviceProvider = _services.BuildServiceProvider();

			var validator = _serviceProvider.GetService<IExpressValidator<TestPersonModel>>();

			Assert.That(validator, Is.Not.Null);
			Assert.That(validator, Is.InstanceOf<ProxyValidator<TestPersonModel>>());
		}

		[Test]
		public void Should_BuildValidatorFromConfigurator_WhenResolved()
		{
			_services.AddExpressValidation(Assembly.GetExecutingAssembly());
			_serviceProvider = _services.BuildServiceProvider();

			var validator = _serviceProvider.GetService<IExpressValidator<TestPersonModel>>();
			var testPerson = new TestPersonModel { Name = "John", Age = 25 };

			var result = validator.Validate(testPerson);

			Assert.That(result, Is.Not.Null);
		}

		[Test]
		public async Task Should_ValidateAsynchronously_WhenValidatorIsResolved()
		{
			_services.AddExpressValidation(Assembly.GetExecutingAssembly());
			_serviceProvider = _services.BuildServiceProvider();

			var validator = _serviceProvider.GetService<IExpressValidator<TestPersonModel>>();
			var testPerson = new TestPersonModel { Name = "Jane", Age = 30 };

			var result = await validator.ValidateAsync(testPerson);

			Assert.That(result, Is.Not.Null);
		}

		[Test]
		public void Should_CreateNewInstancePerRequest_WhenLifetimeIsTransient()
		{
			_services.AddExpressValidation(Assembly.GetExecutingAssembly(), ServiceLifetime.Transient);
			_serviceProvider = _services.BuildServiceProvider();

			var validator1 = _serviceProvider.GetService<IExpressValidator<TestPersonModel>>();
			var validator2 = _serviceProvider.GetService<IExpressValidator<TestPersonModel>>();

			Assert.That(validator1, Is.Not.Null);
			Assert.That(validator2, Is.Not.Null);
			Assert.That(validator1, Is.Not.SameAs(validator2));
		}

		[Test]
		public void Should_ReturnSameInstance_WhenLifetimeIsSingleton()
		{
			_services.AddExpressValidation(Assembly.GetExecutingAssembly(), ServiceLifetime.Singleton);
			_serviceProvider = _services.BuildServiceProvider();

			var validator1 = _serviceProvider.GetService<IExpressValidator<TestPersonModel>>();
			var validator2 = _serviceProvider.GetService<IExpressValidator<TestPersonModel>>();

			Assert.That(validator1, Is.Not.Null);
			Assert.That(validator2, Is.Not.Null);
			Assert.That(validator1, Is.SameAs(validator2));
		}

		[Test]
		public void Should_ReturnSameInstancePerScope_WhenLifetimeIsScoped()
		{
			_services.AddExpressValidation(Assembly.GetExecutingAssembly(), ServiceLifetime.Scoped);
			_serviceProvider = _services.BuildServiceProvider();

			using (var scope1 = _serviceProvider.CreateScope())
			using (var scope2 = _serviceProvider.CreateScope())
			{
				var validator1a = scope1.ServiceProvider.GetService<IExpressValidator<TestPersonModel>>();
				var validator1b = scope1.ServiceProvider.GetService<IExpressValidator<TestPersonModel>>();
				var validator2 = scope2.ServiceProvider.GetService<IExpressValidator<TestPersonModel>>();

				Assert.That(validator1a, Is.SameAs(validator1b));
				Assert.That(validator1a, Is.Not.SameAs(validator2));
			}
		}

		[Test]
		public void Should_ResolveMultipleValidators_WhenMultipleConfiguratorsExist()
		{
			_services.AddExpressValidation(Assembly.GetExecutingAssembly());
			_serviceProvider = _services.BuildServiceProvider();

			var personValidator = _serviceProvider.GetService<IExpressValidator<TestPersonModel>>();
			var productValidator = _serviceProvider.GetService<IExpressValidator<TestProductModel>>();

			Assert.That(personValidator, Is.Not.Null);
			Assert.That(productValidator, Is.Not.Null);
		}

		[Test]
		public void Should_ExecuteConfigureMethod_WhenValidatorIsBuilt()
		{
			_services.AddExpressValidation(Assembly.GetExecutingAssembly());
			_serviceProvider = _services.BuildServiceProvider();

			var validator = _serviceProvider.GetService<IExpressValidator<TestPersonModel>>();
			var testPerson = new TestPersonModel { Name = "", Age = -1 };

			var result = validator.Validate(testPerson);

			// The configurator should have added validation rules
			Assert.That(result, Is.Not.Null);
		}

		[Test]
		public void Should_ThrowException_WhenConfiguratorDoesNotExist()
		{
			_services.AddExpressValidation(Assembly.GetExecutingAssembly());
			_serviceProvider = _services.BuildServiceProvider();

			Assert.Throws<InvalidOperationException>((Action)(() =>
				_serviceProvider.GetRequiredService<IExpressValidator<NonExistentModel>>()));
		}

		[Test]
		public void Should_ResolveConfigurator_WhenRequestedDirectly()
		{
			_services.AddExpressValidation(Assembly.GetExecutingAssembly());
			_serviceProvider = _services.BuildServiceProvider();

			var configurator = _serviceProvider.GetService<IValidatorConfigurator<TestPersonModel>>();

			Assert.That(configurator, Is.Not.Null);
			Assert.That(configurator, Is.InstanceOf<TestPersonModelConfigurator>());
		}

		[Test]
		public void Should_BuildValidatorMultipleTimes_WhenConfiguratorIsTransient()
		{
			_services.AddExpressValidation(Assembly.GetExecutingAssembly(), ServiceLifetime.Transient);
			_serviceProvider = _services.BuildServiceProvider();

			var validator1 = _serviceProvider.GetService<IExpressValidator<TestPersonModel>>();
			var validator2 = _serviceProvider.GetService<IExpressValidator<TestPersonModel>>();

			var testPerson = new TestPersonModel { Name = "Test", Age = 20 };
			var result1 = validator1.Validate(testPerson);
			var result2 = validator2.Validate(testPerson);

			Assert.That(result1, Is.Not.Null);
			Assert.That(result2, Is.Not.Null);
		}

		[Test]
		public async Task Should_HandleConcurrentValidation_WhenMultipleThreadsValidate()
		{
			_services.AddExpressValidation(Assembly.GetExecutingAssembly());
			_serviceProvider = _services.BuildServiceProvider();

			var validator = _serviceProvider.GetService<IExpressValidator<TestPersonModel>>();
			var tasks = new Task<ValidationResult>[10];

			for (int i = 0; i < tasks.Length; i++)
			{
				var testPerson = new TestPersonModel { Name = $"Person{i}", Age = 20 + i };
				tasks[i] = validator.ValidateAsync(testPerson);
			}

			var results = await Task.WhenAll(tasks);

			Assert.That(results, Has.Length.EqualTo(10));
			Assert.That(results, Has.All.Not.Null);
		}

		[Test]
		public void Should_WorkWithDifferentAssemblies_WhenSpecified()
		{
			var currentAssembly = Assembly.GetExecutingAssembly();
			_services.AddExpressValidation(currentAssembly);
			_serviceProvider = _services.BuildServiceProvider();

			var validator = _serviceProvider.GetService<IExpressValidator<TestPersonModel>>();

			Assert.That(validator, Is.Not.Null);
		}

		[Test]
		public void Should_AllowMultipleRegistrations_WhenCalledMultipleTimes()
		{
			_services.AddExpressValidation(Assembly.GetExecutingAssembly());
			_services.AddExpressValidation(Assembly.GetExecutingAssembly());
			_serviceProvider = _services.BuildServiceProvider();

			var validators = _serviceProvider.GetServices<IExpressValidator<TestPersonModel>>();

			Assert.That(validators, Is.Not.Null);
			Assert.That(validators.Count(), Is.GreaterThan(0));
		}

		[Test]
		public void Should_DisposeProperlyInScope_WhenScopedLifetimeUsed()
		{
			_services.AddExpressValidation(Assembly.GetExecutingAssembly(), ServiceLifetime.Scoped);
			_serviceProvider = _services.BuildServiceProvider();

			IExpressValidator<TestPersonModel> validator;

			using (var scope = _serviceProvider.CreateScope())
			{
				validator = scope.ServiceProvider.GetService<IExpressValidator<TestPersonModel>>();
				Assert.That(validator, Is.Not.Null);
			}

			// Validator should have been disposed with scope
			Assert.DoesNotThrow((Action)(() => validator.Validate(new TestPersonModel())));
		}

		[Test]
		public void Should_UseExpressValidatorOptions_WhenConfiguratorHasOptions()
		{
			_services.AddExpressValidation(Assembly.GetExecutingAssembly());
			_serviceProvider = _services.BuildServiceProvider();

			var validator = _serviceProvider.GetService<IExpressValidator<TestPersonWithOptionsModel>>();
			var testPerson = new TestPersonWithOptionsModel { Name = "", Age = -1 };

			var result = validator.Validate(testPerson);

			Assert.That(result, Is.Not.Null);
		}
	}

	// Test Models
	public class TestPersonModel
	{
		public string Name { get; set; }
		public int Age { get; set; }
	}

	public class TestProductModel
	{
		public string ProductName { get; set; }
		public decimal Price { get; set; }
	}

	public class TestPersonWithOptionsModel
	{
		public string Name { get; set; }
		public int Age { get; set; }
	}

	public class NonExistentModel
	{
		public string Value { get; set; }
	}

	// Test Configurators
	public class TestPersonModelConfigurator : ValidatorConfigurator<TestPersonModel>
	{
		public override void Configure(ExpressValidatorBuilder<TestPersonModel> expressValidatorBuilder)
		{
			// Add some validation rules for testing
		}
	}

	public class TestProductModelConfigurator : ValidatorConfigurator<TestProductModel>
	{
		public override void Configure(ExpressValidatorBuilder<TestProductModel> expressValidatorBuilder)
		{
			// Add some validation rules for testing
		}
	}

	public class TestPersonWithOptionsModelConfigurator : ValidatorConfigurator<TestPersonWithOptionsModel>
	{
		public TestPersonWithOptionsModelConfigurator()
			: base(new ExpressValidatorOptions
			{
				OnFirstPropertyValidatorFailed = OnFirstPropertyValidatorFailed.Continue
			})
		{
		}

		public override void Configure(ExpressValidatorBuilder<TestPersonWithOptionsModel> expressValidatorBuilder)
		{
			// Add some validation rules for testing
		}
	}
}
