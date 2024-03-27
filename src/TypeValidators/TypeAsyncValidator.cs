using FluentValidation;
using System.Threading.Tasks;

namespace ExpressValidator
{
	internal class TypeAsyncValidator<T> : TypeValidatorBase<T>
	{
		public TypeAsyncValidator()
		{
			_ruleBuilderInitial = RuleFor(i => i).MustAsync((_, __) => Task.FromResult(true));
		}
	}
}
