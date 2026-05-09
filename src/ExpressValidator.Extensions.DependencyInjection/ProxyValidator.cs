using FluentValidation.Results;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ExpressValidator.Extensions.DependencyInjection
{
	internal class ProxyValidator<T> : IExpressValidator<T>
    {
		private readonly IExpressValidator<T> _innerValidator;
		public ProxyValidator(IServiceProvider serviceProvider)
		{
			var innerConfigurator = serviceProvider.GetRequiredService<IValidatorConfigurator<T>>();
			_innerValidator = innerConfigurator.Build();
		}

		public ValidationResult Validate(T obj) => _innerValidator.Validate(obj);

		public Task<ValidationResult> ValidateAsync(T obj, CancellationToken token = default) => _innerValidator.ValidateAsync(obj, token);
	}
}
