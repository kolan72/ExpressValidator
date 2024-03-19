using FluentValidation.Results;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ExpressValidator
{
	public interface IObjectValidator
	{
		(bool IsValid, List<ValidationFailure> Failures) Validate<TObj>(TObj obj);
		Task<(bool IsValid, List<ValidationFailure> Failures)> ValidateAsync<TObj>(TObj obj, CancellationToken token = default);
		bool IsAsync { get; }
	}
}
