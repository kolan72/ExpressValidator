using System;

namespace ExpressValidator.Extensions.DependencyInjection
{
	internal interface IOptionsMonitorContext<out TOptions>
	{
		DateTimeOffset LastUpdated { get; }
		TOptions Options { get; }
	}
}