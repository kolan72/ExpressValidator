using Microsoft.Extensions.Options;
using System;

namespace ExpressValidator.Extensions.DependencyInjection
{
	internal class OptionsMonitorContext<TOptions> : IOptionsMonitorContext<TOptions>
	{
		private TOptions _options;

		private DateTimeOffset _lastUpdated;

		private readonly object _sync = new object();

		public OptionsMonitorContext(IOptionsMonitor<TOptions> optionsMonitor)
		{
			_options = optionsMonitor.CurrentValue;
			_lastUpdated = DateTimeOffset.UtcNow;

			optionsMonitor.OnChange((newValue, _) =>
			{
				lock (_sync)
				{
					_options = newValue;
					_lastUpdated = DateTimeOffset.UtcNow;
				}
			});
		}

		public TOptions Options
		{
			get
			{
				lock (_sync)
				{
					return _options;
				}
			}
		}

		public DateTimeOffset LastUpdated
		{
			get
			{
				lock (_sync)
				{
					return _lastUpdated;
				}
			}
		}
	}
}
