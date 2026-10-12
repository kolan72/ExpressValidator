using FluentValidation.Results;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ExpressValidator.Extensions.DependencyInjection.Tests
{
	[TestFixture]
	public class ValidatorConfiguratorTTOptionsDiRegistrationTests
	{
		private IServiceCollection _services;

		[SetUp]
		public void SetUp() => _services = new ServiceCollection();

		#region Assembly Scan Discovers Configurators

		[Test]
		public void Should_RegisterIValidatorConfiguratorTT_WhenAssemblyContainsDerivingClass()
		{
			var assembly = Assembly.GetExecutingAssembly();
			_services.AddExpressValidation(assembly);

			var descriptor = _services.FirstOrDefault(sd =>
				sd.ServiceType == typeof(IValidatorConfigurator<SimpleDto, SimpleOptions>));

			Assert.That(descriptor, Is.Not.Null);
		}

		[Test]
		public void Should_RegisterCorrectImplementationType_WhenAssemblyIsScanned()
		{
			var assembly = Assembly.GetExecutingAssembly();
			_services.AddExpressValidation(assembly);

			var descriptor = _services.FirstOrDefault(sd =>
				sd.ServiceType == typeof(IValidatorConfigurator<SimpleDto, SimpleOptions>));

			Assert.That(descriptor, Is.Not.Null);
			Assert.That(descriptor.ImplementationType, Is.EqualTo(typeof(SimpleDtoConfigurator)));
		}

		[Test]
		public void Should_RegisterIExpressValidatorWithReload_WhenTTOptionsConfiguratorIsDiscovered()
		{
			var assembly = Assembly.GetExecutingAssembly();
			_services.AddExpressValidation(assembly);

			var descriptor = _services.FirstOrDefault(sd =>
				sd.ServiceType == typeof(IExpressValidatorWithReload<SimpleDto>));

			Assert.That(descriptor, Is.Not.Null);
		}

		[Test]
		public void Should_RegisterProxyValidatorAsImplementationForWithReload_WhenTTOptionsConfiguratorExists()
		{
			var assembly = Assembly.GetExecutingAssembly();
			_services.AddExpressValidation(assembly);

			var descriptor = _services.FirstOrDefault(sd =>
				sd.ServiceType == typeof(IExpressValidatorWithReload<SimpleDto>));

			Assert.That(descriptor, Is.Not.Null);
			Assert.That(descriptor.ImplementationType, Is.EqualTo(typeof(ProxyValidator<SimpleDto, SimpleOptions>)));
		}

		[Test]
		public void Should_RegisterWithReloadAsSingleton_WhenTTOptionsConfiguratorIsDiscovered()
		{
			var assembly = Assembly.GetExecutingAssembly();
			_services.AddExpressValidation(assembly);

			var descriptor = _services.FirstOrDefault(sd =>
				sd.ServiceType == typeof(IExpressValidatorWithReload<SimpleDto>));

			Assert.That(descriptor, Is.Not.Null);
			Assert.That(descriptor.Lifetime, Is.EqualTo(ServiceLifetime.Singleton));
		}

		#endregion

		#region Lifetime Handling

		[Test]
		public void Should_RegisterConfiguratorWithTransientLifetime_WhenTransientIsSpecified()
		{
			var assembly = Assembly.GetExecutingAssembly();
			_services.AddExpressValidation(assembly, ServiceLifetime.Transient);

			var descriptor = _services.FirstOrDefault(sd =>
				sd.ServiceType == typeof(IValidatorConfigurator<SimpleDto, SimpleOptions>));

			Assert.That(descriptor, Is.Not.Null);
			Assert.That(descriptor.Lifetime, Is.EqualTo(ServiceLifetime.Transient));
		}

		[Test]
		public void Should_RegisterConfiguratorWithSingletonLifetime_WhenSingletonIsSpecified()
		{
			var assembly = Assembly.GetExecutingAssembly();
			_services.AddExpressValidation(assembly, ServiceLifetime.Singleton);

			var descriptor = _services.FirstOrDefault(sd =>
				sd.ServiceType == typeof(IValidatorConfigurator<SimpleDto, SimpleOptions>));

			Assert.That(descriptor, Is.Not.Null);
			Assert.That(descriptor.Lifetime, Is.EqualTo(ServiceLifetime.Singleton));
		}

		[Test]
		public void Should_RegisterConfiguratorWithScopedLifetime_WhenScopedIsSpecified()
		{
			var assembly = Assembly.GetExecutingAssembly();
			_services.AddExpressValidation(assembly, ServiceLifetime.Scoped);

			var descriptor = _services.FirstOrDefault(sd =>
				sd.ServiceType == typeof(IValidatorConfigurator<SimpleDto, SimpleOptions>));

			Assert.That(descriptor, Is.Not.Null);
			Assert.That(descriptor.Lifetime, Is.EqualTo(ServiceLifetime.Scoped));
		}

		#endregion

		#region Configuration Binding

		[Test]
		public void Should_BindOptionsToConfigSectionPath_WhenTTOptionsConfiguratorIsRegistered()
		{
			var assembly = Assembly.GetExecutingAssembly();
			var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>
			{
				["SimpleSettings:MinValue"] = "0",
				["SimpleSettings:MaxValue"] = "100"
			}).Build();
			_services.AddSingleton<IConfiguration>(config);
			_services.AddExpressValidation(assembly);
			var sp = _services.BuildServiceProvider();

			var options = sp.GetService<IOptions<SimpleOptions>>();

			Assert.That(options, Is.Not.Null);
		}

		[Test]
		public void Should_RegisterSectionPathHolder_WhenTTOptionsConfiguratorIsRegistered()
		{
			var assembly = Assembly.GetExecutingAssembly();
			var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>
			{
				["SimpleSettings:MinValue"] = "0",
				["SimpleSettings:MaxValue"] = "100"
			}).Build();
			_services.AddSingleton<IConfiguration>(config);
			_services.AddExpressValidation(assembly);
			var sp = _services.BuildServiceProvider();

			var holder = sp.GetService<IOptions<SectionPathHolder<SimpleOptions>>>();

			Assert.That(holder, Is.Not.Null);
		}

		[Test]
		public void Should_SetCorrectSectionPath_WhenTTOptionsConfiguratorIsRegistered()
		{
			var assembly = Assembly.GetExecutingAssembly();
			_services.AddExpressValidation(assembly);
			var sp = _services.BuildServiceProvider();

			var configurator = sp.GetRequiredService<IValidatorConfigurator<SimpleDto, SimpleOptions>>()
				as ValidatorConfigurator<SimpleDto, SimpleOptions>;

			Assert.That(configurator, Is.Not.Null);
			Assert.That(configurator.ConfigSectionPath, Is.EqualTo("SimpleSettings"));
		}

		#endregion

		#region Multiple Configurators

		[Test]
		public void Should_RegisterMultipleConfigurators_WhenAssemblyContainsMultipleTTOptionsDerivations()
		{
			var assembly = Assembly.GetExecutingAssembly();
			_services.AddExpressValidation(assembly);

			var descriptor1 = _services.FirstOrDefault(sd =>
				sd.ServiceType == typeof(IValidatorConfigurator<SimpleDto, SimpleOptions>));
			var descriptor2 = _services.FirstOrDefault(sd =>
				sd.ServiceType == typeof(IValidatorConfigurator<AnotherDto, AnotherOptions>));

			Assert.That(descriptor1, Is.Not.Null);
			Assert.That(descriptor2, Is.Not.Null);
		}

		[Test]
		public void Should_RegisterBothIExpressValidatorWithReload_WhenMultipleTTOptionsConfiguratorsExist()
		{
			var assembly = Assembly.GetExecutingAssembly();
			_services.AddExpressValidation(assembly);

			var reload1 = _services.FirstOrDefault(sd =>
				sd.ServiceType == typeof(IExpressValidatorWithReload<SimpleDto>));
			var reload2 = _services.FirstOrDefault(sd =>
				sd.ServiceType == typeof(IExpressValidatorWithReload<AnotherDto>));

			Assert.That(reload1, Is.Not.Null);
			Assert.That(reload2, Is.Not.Null);
		}

		#endregion

		#region Manual Registration

		[Test]
		public void Should_ManuallyRegisterIValidatorConfiguratorTT_WhenAddedViaAddTransient()
		{
			_services.AddTransient<IValidatorConfigurator<SimpleDto, SimpleOptions>, SimpleDtoConfigurator>();
			var sp = _services.BuildServiceProvider();

			var configurator = sp.GetService<IValidatorConfigurator<SimpleDto, SimpleOptions>>();

			Assert.That(configurator, Is.Not.Null);
			Assert.That(configurator, Is.InstanceOf<SimpleDtoConfigurator>());
		}

		[Test]
		public void Should_CreateNewInstancePerResolution_WhenManualTransientRegistration()
		{
			_services.AddTransient<IValidatorConfigurator<SimpleDto, SimpleOptions>, SimpleDtoConfigurator>();
			var sp = _services.BuildServiceProvider();

			var c1 = sp.GetService<IValidatorConfigurator<SimpleDto, SimpleOptions>>();
			var c2 = sp.GetService<IValidatorConfigurator<SimpleDto, SimpleOptions>>();

			Assert.That(c1, Is.Not.Null);
			Assert.That(c2, Is.Not.Null);
			Assert.That(c1, Is.Not.SameAs(c2));
		}

		[Test]
		public void Should_ReturnSameInstance_WhenManualSingletonRegistration()
		{
			_services.AddSingleton<IValidatorConfigurator<SimpleDto, SimpleOptions>, SimpleDtoConfigurator>();
			var sp = _services.BuildServiceProvider();

			var c1 = sp.GetService<IValidatorConfigurator<SimpleDto, SimpleOptions>>();
			var c2 = sp.GetService<IValidatorConfigurator<SimpleDto, SimpleOptions>>();

			Assert.That(c1, Is.Not.Null);
			Assert.That(c1, Is.SameAs(c2));
		}

		#endregion

		#region ConfigSectionPath Behavior

		[Test]
		public void Should_ReturnDifferentConfigSectionPath_WhenMultipleConfiguratorsExist()
		{
			var assembly = Assembly.GetExecutingAssembly();
			_services.AddExpressValidation(assembly);
			var sp = _services.BuildServiceProvider();

			var cfg1 = sp.GetRequiredService<IValidatorConfigurator<SimpleDto, SimpleOptions>>()
				as ValidatorConfigurator<SimpleDto, SimpleOptions>;
			var cfg2 = sp.GetRequiredService<IValidatorConfigurator<AnotherDto, AnotherOptions>>()
				as ValidatorConfigurator<AnotherDto, AnotherOptions>;

			Assert.That(cfg1.ConfigSectionPath, Is.Not.EqualTo(cfg2.ConfigSectionPath));
		}

		#endregion

		#region Error Scenarios

		[Test]
		public void Should_ThrowArgumentNullException_WhenServicesIsNull()
		{
			IServiceCollection nullServices = null;

			Assert.Throws<ArgumentNullException>((Action)(() =>
				nullServices.AddExpressValidation(Assembly.GetExecutingAssembly())));
		}

		#endregion

		#region AddExpressValidationFromAssemblyContaining

		[Test]
		public void Should_RegisterFromAssemblyContaining_WhenCalledWithTTOptionsType()
		{
			_services.AddExpressValidationFromAssemblyContaining<SimpleDtoConfigurator>();

			var descriptor = _services.FirstOrDefault(sd =>
				sd.ServiceType == typeof(IValidatorConfigurator<SimpleDto, SimpleOptions>));

			Assert.That(descriptor, Is.Not.Null);
			Assert.That(descriptor.ImplementationType, Is.EqualTo(typeof(SimpleDtoConfigurator)));
		}

		[Test]
		public void Should_RegisterWithReload_WhenAddExpressValidationFromAssemblyContainingIsCalled()
		{
			_services.AddExpressValidationFromAssemblyContaining<SimpleDtoConfigurator>();

			var descriptor = _services.FirstOrDefault(sd =>
				sd.ServiceType == typeof(IExpressValidatorWithReload<SimpleDto>));

			Assert.That(descriptor, Is.Not.Null);
		}

		#endregion

		#region Integration with ProxyValidator

		[Test]
		public void Should_ResolveProxyValidatorWithReload_WhenTTOptionsConfiguratorIsRegistered()
		{
			var assembly = Assembly.GetExecutingAssembly();
			_services.AddExpressValidation(assembly);

			var descriptor = _services.FirstOrDefault(sd =>
				sd.ServiceType == typeof(IExpressValidatorWithReload<SimpleDto>));

			Assert.That(descriptor, Is.Not.Null);
			Assert.That(typeof(ProxyValidator<SimpleDto, SimpleOptions>).IsAssignableFrom(descriptor.ImplementationType), Is.True);
		}

		#endregion

		#region Support Models

		public class SimpleDto
		{
			public string Name { get; set; }
			public int Value { get; set; }
		}

		public class SimpleOptions
		{
			public int MinValue { get; set; }
			public int MaxValue { get; set; }
		}

		public class AnotherDto
		{
			public string Code { get; set; }
			public decimal Amount { get; set; }
		}

		public class AnotherOptions
		{
			public decimal MinAmount { get; set; }
			public decimal MaxAmount { get; set; }
		}

		internal class SimpleDtoConfigurator : ValidatorConfigurator<SimpleDto, SimpleOptions>
		{
			public SimpleDtoConfigurator() { }

			public override string ConfigSectionPath => "SimpleSettings";

			public override void Configure(ExpressValidatorBuilder<SimpleDto, SimpleOptions> builder)
			{
			}
		}

		internal class AnotherDtoConfigurator : ValidatorConfigurator<AnotherDto, AnotherOptions>
		{
			public AnotherDtoConfigurator() { }

			public override string ConfigSectionPath => "AnotherSettings";

			public override void Configure(ExpressValidatorBuilder<AnotherDto, AnotherOptions> builder)
			{
			}
		}

		#endregion
	}
}
