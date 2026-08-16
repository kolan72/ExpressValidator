using FluentValidation.Results;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ExpressValidator.Extensions.DependencyInjection.Tests
{
	/// <summary>
	/// Tests for ValidatorConfigurator{T,TOptions} registration through Microsoft DI.
	/// </summary>
	[TestFixture]
	public class ValidatorConfiguratorTwoTypeParamsTests
	{
		private IServiceCollection _services;

		[SetUp]
		public void SetUp() => _services = new ServiceCollection();

		#region Auto-Discovery Registration Scenarios

		[Test]
		public void Should_DiscoverClassDerivingFromValidatorConfiguratorTTWhenCallingAddExpressValidation()
		{
			var assembly = Assembly.GetExecutingAssembly();
			_services.AddExpressValidation(assembly);

			Assert.That(
				_services.Any(sd => sd.ServiceType == typeof(IValidatorConfigurator<OrderDto, OrderOptions>)),
				Is.True);
		}

		[Test]
		public void Should_NotDiscoverAbstractBaseType_WhenUsingAssemblyScaner()
		{
			var emptyAssembly = typeof(object).Assembly;

			Assert.DoesNotThrow((Action)(() => _services.AddExpressValidation(emptyAssembly)));

			var nonExistent = _services.FirstOrDefault(sd =>
				sd.ServiceType == typeof(IValidatorConfigurator<SthDummy, SthDummyOptions>));

			Assert.That(nonExistent, Is.Null);
		}

		#endregion

		#region Manual Registration Scenarios

		[Test]
		public void Should_CreateNewInstancePerRequest_WhenLifetimeIsTransient()
		{
			_services.AddTransient<IValidatorConfigurator<Item, ItemValidation>, ItemManualValidatorConfigurator>();
			var sp = _services.BuildServiceProvider();

			var c1 = sp.GetService<IValidatorConfigurator<Item, ItemValidation>>();
			var c2 = sp.GetService<IValidatorConfigurator<Item, ItemValidation>>();

			Assert.That(c1, Is.Not.SameAs(c2));
		}

		[Test]
		public void Should_ReturnSameInstance_WhenLifetimeIsSingleton()
		{
			_services.AddSingleton<IValidatorConfigurator<Item, ItemValidation>, ItemManualValidatorConfigurator>();
			var sp = _services.BuildServiceProvider();

			var c1 = sp.GetService<IValidatorConfigurator<Item, ItemValidation>>();
			var c2 = sp.GetRequiredService<IValidatorConfigurator<Item, ItemValidation>>();

			Assert.That(c1, Is.SameAs(c2));
		}

		#endregion

		#region Config Section Path Behavior

		[Test]
		public void Should_ConfigSectionPathReturned_WhenDerivedFromValidatorConfiguratorTT()
		{
			var assembly = Assembly.GetExecutingAssembly();
			_services.AddExpressValidation(assembly);
			var sp = _services.BuildServiceProvider();

			var cfg = sp.GetRequiredService<IValidatorConfigurator<OrderDto, OrderOptions>>() as ValidatorConfigurator<OrderDto, OrderOptions>;
			Assert.That(cfg, Is.Not.Null);
			Assert.That(cfg.ConfigSectionPath, Is.EqualTo("SomeSettings"));
		}

		#endregion

		#region Support Models

		public class OrderDto
		{
			public int ItemCount { get; set; }
			public string Description { get; set; }
		}

		public class OrderOptions
		{
			public int? MinItems { get; set; }
			public int? MaxItems { get; set; }
		}

		public class Item
		{
			public string Name { get; set; }
			public decimal Price { get; set; }
		}

		public class ItemValidation
		{
			public decimal MinPrice { get; set; }
		}

		public class SthDummy
		{
			public string Value { get; set; }
		}

		public class SthDummyOptions
		{
			public bool Enabled { get; set; }
		}

		#endregion



		internal class OrderManualValidatorConfigurator : ValidatorConfigurator<OrderDto, OrderOptions>
		{
			public OrderManualValidatorConfigurator() { }

			public override void Configure(ExpressValidatorBuilder<OrderDto, OrderOptions> b) { }

			public override string ConfigSectionPath => "SomeSettings";
		}

		internal class ItemManualValidatorConfigurator : ValidatorConfigurator<Item, ItemValidation>
		{
			public ItemManualValidatorConfigurator() { }

			public override void Configure(ExpressValidatorBuilder<Item, ItemValidation> b) { }
			public override string ConfigSectionPath => "ItemSettings";
		}
	}
}