using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;

namespace ExpressValidator.Extensions.DependencyInjection
{
	public static class ServiceCollectionExtensions
	{
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
	}
}
