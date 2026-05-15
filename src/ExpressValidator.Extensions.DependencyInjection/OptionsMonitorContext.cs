using Microsoft.Extensions.Options;
using System;
using System.Threading;

namespace ExpressValidator.Extensions.DependencyInjection
{
	internal sealed class OptionsMonitorContext<TOptions> : IOptionsMonitorContext<TOptions>, IDisposable
	{
		// Immutable snapshot for lock-free reads
		private sealed class OptionsSnapshot
		{
			public OptionsSnapshot(TOptions options, DateTimeOffset lastUpdated)
			{
				Options = options;
				LastUpdated = lastUpdated;
			}

			public TOptions Options { get; }
			public DateTimeOffset LastUpdated { get; }
		}

		private OptionsSnapshot _currentSnapshot;
		private readonly IDisposable _changeToken;
		private readonly string _sectionName;

		public OptionsMonitorContext(IOptionsMonitor<TOptions> optionsMonitor, IOptions<SectionPathHolder<TOptions>> options)
		{
			if (optionsMonitor == null)
				throw new ArgumentNullException(nameof(optionsMonitor));
			if (options?.Value == null)
				throw new ArgumentNullException(nameof(options));

			_sectionName = options.Value.SectionPath;

			_currentSnapshot = new OptionsSnapshot(optionsMonitor.CurrentValue, DateTimeOffset.UtcNow);

			// Store change token for proper disposal
			_changeToken = optionsMonitor.OnChange(OnOptionsChanged);
		}

		// Lock-free property access using Interlocked (hot path optimization)
		public TOptions Options
		{
			get
			{
				var snapshot = Interlocked.CompareExchange(ref _currentSnapshot, null, null);
				return snapshot.Options;
			}
		}

		// Lock-free property access using Interlocked (hot path optimization)
		public DateTimeOffset LastUpdated
		{
			get
			{
				var snapshot = Interlocked.CompareExchange(ref _currentSnapshot, null, null);
				return snapshot.LastUpdated;
			}
		}

		private void OnOptionsChanged(TOptions newValue, string section)
		{
			// Only update if section matches - use Interlocked for atomic writes
			if (string.Equals(_sectionName, section, StringComparison.Ordinal))
			{
				var newSnapshot = new OptionsSnapshot(newValue, DateTimeOffset.UtcNow);
				Interlocked.Exchange(ref _currentSnapshot, newSnapshot);
			}
		}

		public void Dispose()
		{
			_changeToken?.Dispose();
		}
	}
}
