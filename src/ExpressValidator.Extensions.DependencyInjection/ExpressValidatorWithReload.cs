using FluentValidation.Results;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ExpressValidator.Extensions.DependencyInjection
{
	internal class ExpressValidatorWithReload<TObj, TOptions> : IExpressValidatorWithReload<TObj>
	{
		private readonly IExpressValidatorBuilder<TObj, TOptions> _expressValidatorBuilder;

		private readonly IOptionsMonitorContext<TOptions> _optionsMonitorContext;

		private IExpressValidator<TObj> _expressValidator;

		private DateTimeOffset _lastUpdated;

		public ExpressValidatorWithReload(IExpressValidatorBuilder<TObj, TOptions> expressValidatorBuilder, IOptionsMonitorContext<TOptions> optionsMonitorContext)
		{
			_lastUpdated = DateTimeOffset.MinValue;
			_expressValidatorBuilder = expressValidatorBuilder;

			_optionsMonitorContext = optionsMonitorContext;
		}

		///<inheritdoc cref = "IExpressValidator{TObj}.Validate(TObj)"/>
		public ValidationResult Validate(TObj obj)
		{
			RebuildValidatorIfNeed();
			return _expressValidator.Validate(obj);
		}

		///<inheritdoc cref = "IExpressValidator{TObj}.ValidateAsync(TObj, CancellationToken)"/>
		public Task<ValidationResult> ValidateAsync(TObj obj, CancellationToken token = default)
		{
			RebuildValidatorIfNeed();
			return _expressValidator.ValidateAsync(obj, token);
		}

		private void RebuildValidatorIfNeed()
		{
			if (_optionsMonitorContext.LastUpdated > _lastUpdated)
			{
				_lastUpdated = _optionsMonitorContext.LastUpdated;
				var options = _optionsMonitorContext.Options;
				_expressValidator = _expressValidatorBuilder.Build(options);
			}
		}
	}
}
