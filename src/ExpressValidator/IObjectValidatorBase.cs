using FluentValidation.Results;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

namespace ExpressValidator
{
	internal interface IObjectValidatorBase<in TObj>
	{
		bool IsAsync { get; }

		(bool IsValid, List<ValidationFailure> Failures) Validate(TObj obj);
		Task<(bool IsValid, List<ValidationFailure> Failures)> ValidateAsync(TObj obj, CancellationToken token = default);
	}
}
