using System;

namespace ExpressValidator
{
	class AppendedArg<TOptions>
	{
		public string ArgName { get; set; }
		public Func<TOptions, object> Selector { get; set; }
	}
}
