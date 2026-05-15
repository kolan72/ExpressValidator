using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Reflection;

namespace ExpressValidator.Extensions.DependencyInjection
{
	public static class ServiceCollectionExtensions
	{
		/// <summary>
		/// Registers all concrete, non-abstract, non-generic classes that inherit from <see cref="ValidatorConfigurator{T}"/>
		/// into the Microsoft Dependency Injection container.
		/// <br/>
		/// Behind the scenes, for every configurator type <c>ExpressConfigurator&lt;T&gt;</c> found,
		/// the DI container will also expose a proxy implementation of <see cref="IExpressValidator{T}"/>,
		/// enabling validation logic to be resolved transparently via the service provider.
		/// </summary>
		/// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
		/// <param name="assemblyToScan">The assembly to scan for configurator types.</param>
		/// <param name="lifetime">The service lifetime for the registered configurators. Defaults to <see cref="ServiceLifetime.Transient"/>.</param>
		/// <returns>The <see cref="IServiceCollection"/> for chaining.</returns>
		public static IServiceCollection AddExpressValidation(this IServiceCollection services, Assembly assemblyToScan, ServiceLifetime lifetime = ServiceLifetime.Transient)
		{
			if (services == null)
				throw new ArgumentNullException(nameof(services));

			assemblyToScan = assemblyToScan ?? Assembly.GetCallingAssembly();
			services.AddAllConfigurators(assemblyToScan, lifetime);
			services.TryAdd(new ServiceDescriptor(typeof(IExpressValidator<>), typeof(ProxyValidator<>), lifetime));
			return services;
		}

		/// <summary>
		/// Registers all concrete, non-abstract, non-generic classes that inherit from <see cref="ValidatorConfigurator{T}"/>
		/// from the assembly that contains <typeparamref name="T"/> into the Microsoft Dependency Injection container.
		/// </summary>
		/// <typeparam name="T">A type located in the assembly to scan.</typeparam>
		/// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
		/// <param name="lifetime">The service lifetime for the registered configurators. Defaults to <see cref="ServiceLifetime.Transient"/>.</param>
		/// <returns>The <see cref="IServiceCollection"/> for chaining.</returns>
		public static IServiceCollection AddExpressValidationFromAssemblyContaining<T>(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Transient)
		{
			if (services == null)
				throw new ArgumentNullException(nameof(services));

			return services.AddExpressValidation(typeof(T).Assembly, lifetime);
		}

		/// <summary>
		/// Registers all concrete, non-abstract, non-generic classes that inherit from <see cref="ValidatorConfigurator{T}"/>
		/// from the assembly that called this method into the Microsoft Dependency Injection container.
		/// </summary>
		/// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
		/// <param name="lifetime">The service lifetime for the registered configurators. Defaults to <see cref="ServiceLifetime.Transient"/>.</param>
		/// <returns>The <see cref="IServiceCollection"/> for chaining.</returns>
		public static IServiceCollection AddExpressValidationFromCurrentAssembly(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Transient)
		{
			if (services == null)
				throw new ArgumentNullException(nameof(services));

			return services.AddExpressValidation(Assembly.GetCallingAssembly(), lifetime);
		}

		public static IServiceCollection AddExpressValidator<T>(this IServiceCollection services, Action<ExpressValidatorBuilder<T>> configure, ServiceLifetime serviceLifetime = ServiceLifetime.Transient)
		{
			return AddExpressValidator(
				services, 
				configure, 
				new ExpressValidatorOptions { OnFirstPropertyValidatorFailed = OnFirstPropertyValidatorFailed.Continue }, 
				serviceLifetime);
		}

		public static IServiceCollection AddExpressValidator<T>(this IServiceCollection services, Action<ExpressValidatorBuilder<T>> configure, ExpressValidatorOptions expressValidatorOptions, ServiceLifetime serviceLifetime = ServiceLifetime.Transient)
		{
			if (services == null)
				throw new ArgumentNullException(nameof(services));
			if (configure == null)
				throw new ArgumentNullException(nameof(configure));
			if (expressValidatorOptions == null)
				throw new ArgumentNullException(nameof(expressValidatorOptions));

			ConfigureExpressValidatorOptions(services, expressValidatorOptions);

			services.Add(new ServiceDescriptor(
				typeof(IExpressValidator<T>), 
				sp => CreateExpressValidator(sp, configure), 
				serviceLifetime));

			return services;
		}

		public static IServiceCollection AddExpressValidatorBuilder<T, TOptions>(this IServiceCollection services, Action<ExpressValidatorBuilder<T, TOptions>> configure, ServiceLifetime serviceLifetime = ServiceLifetime.Transient)
		{
			return AddExpressValidatorBuilder(
				services, 
				configure, 
				new ExpressValidatorOptions { OnFirstPropertyValidatorFailed = OnFirstPropertyValidatorFailed.Continue }, 
				serviceLifetime);
		}

		public static IServiceCollection AddExpressValidatorBuilder<T, TOptions>(this IServiceCollection services, Action<ExpressValidatorBuilder<T, TOptions>> configure, ExpressValidatorOptions expressValidatorOptions, ServiceLifetime serviceLifetime = ServiceLifetime.Transient)
		{
			if (services == null)
				throw new ArgumentNullException(nameof(services));
			if (configure == null)
				throw new ArgumentNullException(nameof(configure));
			if (expressValidatorOptions == null)
				throw new ArgumentNullException(nameof(expressValidatorOptions));

			ConfigureExpressValidatorOptions(services, expressValidatorOptions);

			services.Add(new ServiceDescriptor(
				typeof(IExpressValidatorBuilder<T, TOptions>), 
				sp => CreateExpressValidatorBuilder(sp, configure), 
				serviceLifetime));

			return services;
		}

		///<inheritdoc cref = "AddExpressValidatorWithReload{T, TOptions}(IServiceCollection, Action{ExpressValidatorBuilder{T, TOptions}}, ExpressValidatorOptions, string)"/>
		public static IServiceCollection AddExpressValidatorWithReload<T, TOptions>(this IServiceCollection services, Action<ExpressValidatorBuilder<T, TOptions>> configure, string configSectionPath) where TOptions : class
		{
			return AddExpressValidatorWithReload(
				services, 
				configure, 
				new ExpressValidatorOptions { OnFirstPropertyValidatorFailed = OnFirstPropertyValidatorFailed.Continue }, 
				configSectionPath);
		}

		/// <summary>
		/// Registers a singleton <see cref="IExpressValidatorWithReload{T}"/> in the <see cref="IServiceCollection"/>.
		/// Enables dynamic rebuilding of the validator whenever the configuration values of the <paramref name="configSectionPath"/> section bound to the <typeparamref name="TOptions"/> options are changed.
		/// This is achieved by the proxy validator that implements the <see cref="IExpressValidatorWithReload{T}"/> interface using <see cref="IOptionsMonitor{TOptions}"/> and <see cref="ExpressValidatorBuilder{T, TOptions}"/> behind the scenes.
		/// </summary>
		/// <typeparam name="T">Object to validate.</typeparam>
		/// <typeparam name="TOptions">Options for use by <see cref="ExpressValidatorBuilder{T, TOptions}"/> to build a validator.</typeparam>
		/// <param name="services"><see cref="IServiceCollection"/></param>
		/// <param name="configure">Action to configure <see cref="ExpressValidatorBuilder{T, TOptions}"/>.</param>
		/// <param name="expressValidatorOptions"><see cref="ExpressValidatorOptions"/></param>
		/// <param name="configSectionPath">Configuration section path to bind TOptions type.</param>
		/// <returns>The <see cref="IServiceCollection"/> for chaining.</returns>
		public static IServiceCollection AddExpressValidatorWithReload<T, TOptions>(this IServiceCollection services, Action<ExpressValidatorBuilder<T, TOptions>> configure, ExpressValidatorOptions expressValidatorOptions, string configSectionPath) where TOptions : class
		{
			if (services == null)
				throw new ArgumentNullException(nameof(services));
			if (configure == null)
				throw new ArgumentNullException(nameof(configure));
			if (expressValidatorOptions == null)
				throw new ArgumentNullException(nameof(expressValidatorOptions));
			if (string.IsNullOrWhiteSpace(configSectionPath))
				throw new ArgumentException("Configuration section path cannot be null or whitespace.", nameof(configSectionPath));

			services.AddOptions<TOptions>()
				.BindConfiguration(configSectionPath);

			services.AddOptions<SectionPathHolder<TOptions>>()
				.Configure(opt => opt.SectionPath = configSectionPath);

			ConfigureExpressValidatorOptions(services, expressValidatorOptions);

			services.TryAddSingleton<IOptionsMonitorContext<TOptions>, OptionsMonitorContext<TOptions>>();

			services.AddSingleton<IExpressValidatorWithReload<T>>(sp =>
			{
				var options = sp.GetRequiredService<IOptions<ExpressValidatorOptions>>();
				var builder = new ExpressValidatorBuilder<T, TOptions>(options.Value.OnFirstPropertyValidatorFailed);
				configure(builder);

				var context = sp.GetRequiredService<IOptionsMonitorContext<TOptions>>();
				return new ExpressValidatorWithReload<T, TOptions>(builder, context);
			});

			return services;
		}

		// Extracted helper to eliminate repeated options configuration
		private static void ConfigureExpressValidatorOptions(
			IServiceCollection services, 
			ExpressValidatorOptions expressValidatorOptions)
		{
			services.AddOptions<ExpressValidatorOptions>()
				.Configure(options => 
					options.OnFirstPropertyValidatorFailed = expressValidatorOptions.OnFirstPropertyValidatorFailed);
		}

		// Extracted factory method for ExpressValidator creation
		private static IExpressValidator<T> CreateExpressValidator<T>(
			IServiceProvider sp, 
			Action<ExpressValidatorBuilder<T>> configure)
		{
			var options = sp.GetRequiredService<IOptions<ExpressValidatorOptions>>();
			var builder = new ExpressValidatorBuilder<T>(options.Value.OnFirstPropertyValidatorFailed);
			configure(builder);
			return builder.Build();
		}

		// Extracted factory method for ExpressValidatorBuilder creation
		private static ExpressValidatorBuilder<T, TOptions> CreateExpressValidatorBuilder<T, TOptions>(
			IServiceProvider sp, 
			Action<ExpressValidatorBuilder<T, TOptions>> configure)
		{
			var options = sp.GetRequiredService<IOptions<ExpressValidatorOptions>>();
			var builder = new ExpressValidatorBuilder<T, TOptions>(options.Value.OnFirstPropertyValidatorFailed);
			configure(builder);
			return builder;
		}

		internal static IServiceCollection AddAllConfigurators(
			this IServiceCollection services,
			Assembly assemblyToScan,
			ServiceLifetime lifetime = ServiceLifetime.Transient)
		{
			var openGenericInterface = typeof(IValidatorConfigurator<>);

			var configuratorTypes = assemblyToScan.GetTypes()
				.Where(t => t.IsClass && !t.IsAbstract && !t.IsGenericTypeDefinition)
				.Select(t => new
				{
					ImplementationType = t,
					InterfaceType = t.GetInterfaces()
						.FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == openGenericInterface)
				})
				.Where(x => x.InterfaceType != null);

			foreach (var registration in configuratorTypes)
			{
				services.Add(new ServiceDescriptor(
					registration.InterfaceType, 
					registration.ImplementationType, 
					lifetime));
			}

			return services;
		}
	}
}
