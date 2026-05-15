using System;

namespace ExpressValidator.Extensions.DependencyInjection
{
	internal interface IOptionsMonitorContext<out TOptions> : IDisposable
	{
		DateTimeOffset LastUpdated { get; }
		TOptions Options { get; }
	}
}