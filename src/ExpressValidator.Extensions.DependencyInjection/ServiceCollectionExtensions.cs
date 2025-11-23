using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Reflection;

namespace ExpressValidator.Extensions.DependencyInjection
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddExpressValidation(this IServiceCollection services, Assembly assemblyToScan, ServiceLifetime lifetime = ServiceLifetime.Transient)
		{
			assemblyToScan = assemblyToScan ?? Assembly.GetExecutingAssembly();
			services.AddAllConfigurators(assemblyToScan, lifetime);
			services.Add(new ServiceDescriptor(typeof(IExpressValidator<>), typeof(ProxyValidator<>), lifetime));
			return services;
		}

		public static IServiceCollection AddExpressValidator<T>(this IServiceCollection services, Action<ExpressValidatorBuilder<T>> configure, ServiceLifetime serviceLifetime = ServiceLifetime.Transient)
		{
			return AddExpressValidator(services, configure, new ExpressValidatorOptions() { OnFirstPropertyValidatorFailed = OnFirstPropertyValidatorFailed.Continue }, serviceLifetime);
		}

		public static IServiceCollection AddExpressValidator<T>(this IServiceCollection services, Action<ExpressValidatorBuilder<T>> configure, ExpressValidatorOptions expressValidatorOptions, ServiceLifetime serviceLifetime = ServiceLifetime.Transient)
		{
			IExpressValidator<T> func(IServiceProvider sp)
			{
				var s = sp.GetRequiredService<IOptions<ExpressValidatorOptions>>();
				var eb = new ExpressValidatorBuilder<T>(s.Value.OnFirstPropertyValidatorFailed);
				configure(eb);
				return eb.Build();
			}
			services.AddOptions<ExpressValidatorOptions>()
					.Configure(options =>
										options.OnFirstPropertyValidatorFailed = expressValidatorOptions.OnFirstPropertyValidatorFailed);
			services.Add(new ServiceDescriptor(typeof(IExpressValidator<T>), func, serviceLifetime));
			return services;
		}

		public static IServiceCollection AddExpressValidatorBuilder<T, TOptions>(this IServiceCollection services, Action<ExpressValidatorBuilder<T, TOptions>> configure, ServiceLifetime serviceLifetime = ServiceLifetime.Transient)
		{
			return AddExpressValidatorBuilder(services, configure, new ExpressValidatorOptions() { OnFirstPropertyValidatorFailed = OnFirstPropertyValidatorFailed.Continue }, serviceLifetime);
		}

		public static IServiceCollection AddExpressValidatorBuilder<T, TOptions>(this IServiceCollection services, Action<ExpressValidatorBuilder<T, TOptions>> configure, ExpressValidatorOptions expressValidatorOptions, ServiceLifetime serviceLifetime = ServiceLifetime.Transient)
		{
			ExpressValidatorBuilder<T, TOptions> func(IServiceProvider sp)
			{
				var s = sp.GetRequiredService<IOptions<ExpressValidatorOptions>>();
				var eb = new ExpressValidatorBuilder<T, TOptions>(s.Value.OnFirstPropertyValidatorFailed);
				configure(eb);
				return eb;
			}
			services.AddOptions<ExpressValidatorOptions>()
					.Configure(options =>
										options.OnFirstPropertyValidatorFailed = expressValidatorOptions.OnFirstPropertyValidatorFailed);
			services.Add(new ServiceDescriptor(typeof(IExpressValidatorBuilder<T, TOptions>), func, serviceLifetime));
			return services;
		}

		///<inheritdoc cref = "AddExpressValidatorWithReload{T, TOptions}(IServiceCollection, Action{ExpressValidatorBuilder{T, TOptions}}, ExpressValidatorOptions, string)"/>
		public static IServiceCollection AddExpressValidatorWithReload<T, TOptions>(this IServiceCollection services, Action<ExpressValidatorBuilder<T, TOptions>> configure, string configSectionPath) where TOptions : class
		{
			return AddExpressValidatorWithReload(services, configure, new ExpressValidatorOptions() { OnFirstPropertyValidatorFailed = OnFirstPropertyValidatorFailed.Continue }, configSectionPath);
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
		/// <returns></returns>
		public static IServiceCollection AddExpressValidatorWithReload<T, TOptions>(this IServiceCollection services, Action<ExpressValidatorBuilder<T, TOptions>> configure, ExpressValidatorOptions expressValidatorOptions, string configSectionPath) where TOptions : class
		{
			services.AddOptions<TOptions>(configSectionPath).BindConfiguration(configSectionPath);

			services.AddOptions<SectionPathHolder<TOptions>>().Configure(opt => opt.SectionPath = configSectionPath);

			ExpressValidatorWithReload<T, TOptions> func(IServiceProvider sp)
			{
				var s = sp.GetRequiredService<IOptions<ExpressValidatorOptions>>();
				var eb = new ExpressValidatorBuilder<T, TOptions>(s.Value.OnFirstPropertyValidatorFailed);
				configure(eb);
				var ctx = sp.GetRequiredService<IOptionsMonitorContext<TOptions>>();
				return new ExpressValidatorWithReload<T, TOptions>(eb, ctx);
			}
			services.AddOptions<ExpressValidatorOptions>()
					.Configure(options =>
										options.OnFirstPropertyValidatorFailed = expressValidatorOptions.OnFirstPropertyValidatorFailed);

			services.Add(new ServiceDescriptor(typeof(IExpressValidatorWithReload<T>), func, ServiceLifetime.Singleton));
			services.AddSingleton<IOptionsMonitorContext<TOptions>, OptionsMonitorContext <TOptions>>();
			return services;
		}

		internal static IServiceCollection AddAllConfigurators(
			this IServiceCollection services,
			Assembly assemblyToScan,
			ServiceLifetime lifetime = ServiceLifetime.Transient)
		{
			// 1. Define the open generic interface type to search for.
			var openGenericInterface = typeof(IExpressConfigurator<>);

			// 2. Scan the assembly for all types that are concrete classes and implement IExpressConfigurator.
			var builderTypes = assemblyToScan.GetTypes()
				.Where(t => t.IsClass && !t.IsAbstract && !t.IsGenericTypeDefinition)
				.Select(t => new
				{
					ImplementationType = t,
					InterfaceType = Array.Find(t.GetInterfaces(),
											i => i.IsGenericType && i.GetGenericTypeDefinition() == openGenericInterface)
				})
				.Where(x => x.InterfaceType != null);

			// 3. Register each implementation.
			foreach (var builderRegistration in builderTypes)
			{
				var serviceType = builderRegistration.InterfaceType;
				var implementationType = builderRegistration.ImplementationType;
				var descriptor = new ServiceDescriptor(serviceType, implementationType, lifetime);
				services.Add(descriptor);
			}

			return services;
		}
	}
}
