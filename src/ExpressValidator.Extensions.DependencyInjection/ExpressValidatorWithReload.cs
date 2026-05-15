using FluentValidation.Results;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ExpressValidator.Extensions.DependencyInjection
{
	internal sealed class ExpressValidatorWithReload<TObj, TOptions> : IExpressValidatorWithReload<TObj>
	{
		private readonly IExpressValidatorBuilder<TObj, TOptions> _expressValidatorBuilder;
		private readonly IOptionsMonitorContext<TOptions> _optionsMonitorContext;
		private readonly object _lock = new object();
		
		// Immutable snapshot class for thread-safe reference swapping
		private sealed class ValidatorSnapshot
		{
			public ValidatorSnapshot(IExpressValidator<TObj> validator, DateTimeOffset timestamp)
			{
				Validator = validator;
				Timestamp = timestamp;
			}
			
			public IExpressValidator<TObj> Validator { get; }
			public DateTimeOffset Timestamp { get; }
		}
		
		// Use Interlocked for atomic reference updates instead of volatile
		private ValidatorSnapshot _currentSnapshot;

		public ExpressValidatorWithReload(IExpressValidatorBuilder<TObj, TOptions> expressValidatorBuilder, IOptionsMonitorContext<TOptions> optionsMonitorContext)
		{
			_expressValidatorBuilder = expressValidatorBuilder ?? throw new ArgumentNullException(nameof(expressValidatorBuilder));
			_optionsMonitorContext = optionsMonitorContext ?? throw new ArgumentNullException(nameof(optionsMonitorContext));
			
			// Initialize with empty snapshot - will rebuild on first use
			_currentSnapshot = new ValidatorSnapshot(null, DateTimeOffset.MinValue);
		}

		///<inheritdoc cref = "IExpressValidator{TObj}.Validate(TObj)"/>
		public ValidationResult Validate(TObj obj)
		{
			var validator = GetOrRebuildValidator();
			return validator.Validate(obj);
		}

		///<inheritdoc cref = "IExpressValidator{TObj}.ValidateAsync(TObj, CancellationToken)"/>
		public Task<ValidationResult> ValidateAsync(TObj obj, CancellationToken token = default)
		{
			var validator = GetOrRebuildValidator();
			return validator.ValidateAsync(obj, token);
		}

		private IExpressValidator<TObj> GetOrRebuildValidator()
		{
			// Atomic read of current snapshot reference
			var snapshot = Interlocked.CompareExchange(ref _currentSnapshot, null, null);
			var optionsLastUpdated = _optionsMonitorContext.LastUpdated;
			
			// Fast path: validator is up-to-date (lock-free read)
			if (snapshot.Validator != null && snapshot.Timestamp >= optionsLastUpdated)
			{
				return snapshot.Validator;
			}
			
			// Slow path: rebuild needed - use lock to prevent duplicate rebuilds
			lock (_lock)
			{
				// Double-check: another thread may have already rebuilt while we waited for lock
				snapshot = _currentSnapshot;
				if (snapshot.Validator != null && snapshot.Timestamp >= optionsLastUpdated)
				{
					return snapshot.Validator;
				}
				
				// Rebuild validator with current options
				var options = _optionsMonitorContext.Options;
				var newValidator = _expressValidatorBuilder.Build(options);
				
				// Atomically update snapshot reference using Interlocked
				var newSnapshot = new ValidatorSnapshot(newValidator, optionsLastUpdated);
				Interlocked.Exchange(ref _currentSnapshot, newSnapshot);
				
				return newValidator;
			}
		}
	}
}
