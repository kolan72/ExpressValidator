using ExpressValidator.Extensions.DependencyInjection;

namespace ConfiguratorWithOptionsDemo
{
	public static class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// Automatically scans and registers ValidatorConfigurator<T, TOptions>
			// Reads ConfigSectionPath from each configurator and binds configuration automatically
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

	public interface IGuessTheNumberService
	{
		(bool Result, string Message) Guess();
	}

	public class GuessTheNumberService : IGuessTheNumberService
	{
		private readonly IExpressValidatorWithReload<ObjToValidate> _validator;

		public GuessTheNumberService(IExpressValidatorWithReload<ObjToValidate> validator)
		{
			_validator = validator;
		}

		public (bool Result, string Message) Guess()
		{
			var i = Random.Shared.Next(1, 21);
			var obj = new ObjToValidate { I = i };
			var validationResult = _validator.Validate(obj);

			if (!validationResult.IsValid)
			{
				return (false, string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
			}

			return (true, "Valid guess!");
		}
	}

	public class ObjToValidate
	{
		public int I { get; set; }
	}
}
