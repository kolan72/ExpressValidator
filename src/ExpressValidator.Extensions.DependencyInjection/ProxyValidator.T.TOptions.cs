using FluentValidation.Results;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ExpressValidator.Extensions.DependencyInjection
{
	internal class ProxyValidator<T, TOptions> : IExpressValidatorWithReload<T> where TOptions : class
	{
		private readonly IServiceScopeFactory _scopeFactory;
		private readonly IOptionsMonitor<TOptions> _optionsMonitor;
		private readonly object _lock = new object();

		// Immutable snapshot class for thread-safe reference swapping
		private sealed class ValidatorSnapshot
		{
			public ValidatorSnapshot(IExpressValidator<T> validator, TOptions options)
			{
				Validator = validator;
				Options = options;
			}

			public IExpressValidator<T> Validator { get; }
			public TOptions Options { get; }
		}

		// Use Interlocked for atomic reference updates
		private ValidatorSnapshot _currentSnapshot;

		public ProxyValidator(IServiceProvider serviceProvider)
		{
			if (serviceProvider == null)
				throw new ArgumentNullException(nameof(serviceProvider));

			// Resolve the transient configurator lazily per build via a dedicated scope
			// to avoid a captive dependency (singleton holding a transient).
			_scopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();

			// Validate the configuration section path via a short-lived scope
			using (var scope = _scopeFactory.CreateScope())
			{
				var configurator = scope.ServiceProvider.GetRequiredService<IValidatorConfigurator<T, TOptions>>();
				var configSectionPath = configurator.ConfigSectionPath;
				if (string.IsNullOrWhiteSpace(configSectionPath))
					throw new InvalidOperationException(
						$"ConfigSectionPath for {configurator.GetType().Name} cannot be null or whitespace.");
			}

			// Get or create the options monitor - this requires configuration to be bound
			_optionsMonitor = serviceProvider.GetRequiredService<IOptionsMonitor<TOptions>>();

			// Initialize with current options
			var currentOptions = _optionsMonitor.CurrentValue;
			var initialValidator = BuildValidator(currentOptions);
			_currentSnapshot = new ValidatorSnapshot(initialValidator, currentOptions);
		}

		private IExpressValidator<T> BuildValidator(TOptions options)
		{
			// Transient configurator is resolved and disposed within a dedicated scope per call
			using (var scope = _scopeFactory.CreateScope())
			{
				var configurator = scope.ServiceProvider.GetRequiredService<IValidatorConfigurator<T, TOptions>>();
				return configurator.Build(options);
			}
		}

		public ValidationResult Validate(T obj)
		{
			var validator = GetOrRebuildValidator();
			return validator.Validate(obj);
		}

		public Task<ValidationResult> ValidateAsync(T obj, CancellationToken token = default)
		{
			var validator = GetOrRebuildValidator();
			return validator.ValidateAsync(obj, token);
		}

		private IExpressValidator<T> GetOrRebuildValidator()
		{
			// Atomic read of current snapshot reference
			var snapshot = Interlocked.CompareExchange(ref _currentSnapshot, null, null);
			var currentOptions = _optionsMonitor.CurrentValue;

			// Fast path: validator is up-to-date (lock-free read)
			// Compare by reference since IOptionsMonitor returns same instance until change
			if (ReferenceEquals(snapshot.Options, currentOptions))
			{
				return snapshot.Validator;
			}

			// Slow path: rebuild needed - use lock to prevent duplicate rebuilds
			lock (_lock)
			{
				// Double-check: another thread may have already rebuilt while we waited for lock
				snapshot = _currentSnapshot;
				currentOptions = _optionsMonitor.CurrentValue;

				if (ReferenceEquals(snapshot.Options, currentOptions))
				{
					return snapshot.Validator;
				}

				// Rebuild validator with current options
				var newValidator = BuildValidator(currentOptions);

				// Atomically update snapshot reference using Interlocked
				var newSnapshot = new ValidatorSnapshot(newValidator, currentOptions);
				Interlocked.Exchange(ref _currentSnapshot, newSnapshot);

				return newValidator;
			}
		}
	}
}
