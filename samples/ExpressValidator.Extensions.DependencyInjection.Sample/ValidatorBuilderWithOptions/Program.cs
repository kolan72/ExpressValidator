using ExpressValidator.Extensions.DependencyInjection;
using FluentValidation;
using Shared;

namespace ValidatorBuilderWithOptions
{
	public static class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			builder.Services.AddExpressValidatorBuilder<ObjToValidate, ValidationParametersOptions>(b =>
								b.AddProperty(o => o.I)
								.WithValidation((to, rbo) => rbo.GreaterThan(to.IGreaterThanValue)
								.WithMessage($"Must be greater than {to.IGreaterThanValue}!")));

			builder.Services.AddTransient<IAdvancedNumberGuessingService, AdvancedNumberGuessingService>();

			builder.Services.Configure<ValidationParametersOptions>(builder.Configuration.GetSection("ValidationParameters"));

			var app = builder.Build();

			app.MapGet("/complexguess", (IAdvancedNumberGuessingService service) =>
			{
				var (Result, Message) = service.ComplexGuess();
				if (!Result)
				{
					return Results.BadRequest(Message);
				}
				else
				{
					return Results.Ok(Message);
				}
			});

			app.Run();
		}
	}
}
