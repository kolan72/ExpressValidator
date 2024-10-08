using FluentValidation.Results;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ExpressValidator
{
	internal interface IObjectValidator<TObj, TOptions> : IObjectValidator<TObj>
	{
		void ApplyOptions(TOptions options);
	}

	internal interface IObjectValidator<TObj>
	{
		(bool IsValid, List<ValidationFailure> Failures) Validate(TObj obj);
		Task<(bool IsValid, List<ValidationFailure> Failures)> ValidateAsync(TObj obj, CancellationToken token = default);
		bool IsAsync { get; }
	}
}
