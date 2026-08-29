using ExpressValidator.Extensions.DependencyInjection;
using Shared;

namespace ConfiguratorDemo
{
	public static class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			builder.Services.AddExpressValidationFromCurrentAssembly();

			builder.Services.AddTransient<IGuessTheNumberService, GuessTheNumberService>();

			var app = builder.Build();

			app.MapGet("/guess", (IGuessTheNumberService service) =>
			{
				var (Result, Message) = service.Guess();
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