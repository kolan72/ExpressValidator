using ExpressValidator.Extensions.DependencyInjection;
using System.Reflection;

namespace ConfiguratorDemo
{
	public static class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			builder.Services.AddExpressValidation(Assembly.GetExecutingAssembly());

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

	public class ObjToValidate
	{
		public int I { get; set; }
	}
}