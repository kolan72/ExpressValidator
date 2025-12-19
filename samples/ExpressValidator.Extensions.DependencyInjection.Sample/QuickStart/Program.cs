using ExpressValidator;
using ExpressValidator.Extensions.DependencyInjection;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddExpressValidator<ObjToValidate>(b =>
								b.AddProperty(o => o.I)
								.WithValidation(o => o.GreaterThan(5)
								.WithMessage("Must be greater than 5!")));

builder.Services.AddExpressValidatorWithReload<ObjToValidate, ValidationParametersOptions>(b =>
								b.AddProperty(o => o.I)
								.WithValidation((to, rbo) => rbo.GreaterThan(to.IGreaterThanValue)
								.WithMessage($"Must be greater than {to.IGreaterThanValue}!")),
								"ValidationParameters");

builder.Services.AddTransient<IGuessTheNumberService, GuessTheNumberService>();

builder.Services.Configure<ValidationParametersOptions>(builder.Configuration.GetSection("ValidationParameters"));

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

app.MapGet("/guesswithreload", (IGuessTheNumberService service) =>
{
	var (Result, Message) = service.GuessWithReload();
	if (!Result)
	{
		return Results.BadRequest(Message);
	}
	else
	{
		return Results.Ok(Message);
	}
});

app.MapGet("/guesswithreloadasync", async (IGuessTheNumberService service) =>
{
	var (Result, Message) = await service.GuessWithReloadAsync();
	if (!Result)
	{
		return Results.BadRequest(Message);
	}
	else
	{
		return Results.Ok(Message);
	}
});

await app.RunAsync();


#pragma warning disable S3903 // Types should be defined in named namespaces
public interface IGuessTheNumberService
{
	(bool Result, string Message) Guess();
	(bool Result, string Message) GuessWithReload();
	Task<(bool Result, string Message)> GuessWithReloadAsync();
}

public class GuessTheNumberService : IGuessTheNumberService
{
	private readonly IExpressValidator<ObjToValidate> _expressValidator;
	private readonly IExpressValidatorWithReload<ObjToValidate> _expressValidatorWithReload;

	public GuessTheNumberService(IExpressValidator<ObjToValidate> expressValidator,
								 IExpressValidatorWithReload<ObjToValidate> expressValidatorWithReload)
	{
		_expressValidator = expressValidator;
		_expressValidatorWithReload = expressValidatorWithReload;
	}

	public (bool Result, string Message) Guess()
	{
		var i = Random.Shared.Next(1, 11);
		var objToValidate = new ObjToValidate() { I = i };
		var vr = _expressValidator.Validate(objToValidate);
		if (vr.IsValid)
		{
			return (true, $"You guessed {i} and it is correct!");
		}
		else
		{
			return (false, $"You have chosen {i} and it is wrong. " + vr.ToString());
		}
	}

	public (bool Result, string Message) GuessWithReload()
	{
		var i = Random.Shared.Next(1, 11);
		var objToValidate = new ObjToValidate() { I = i };
		var vr = _expressValidatorWithReload.Validate(objToValidate);
		if (vr.IsValid)
		{
			return (true, $"You guessed {i} and it is correct!");
		}
		else
		{
			return (false, $"You have chosen {i} and it is wrong. " + vr.ToString());
		}
	}

	public async Task<(bool Result, string Message)> GuessWithReloadAsync()
	{
		var i = Random.Shared.Next(1, 11);
		var objToValidate = new ObjToValidate() { I = i };
		var vr = await _expressValidatorWithReload.ValidateAsync(objToValidate).ConfigureAwait(false);
		if (vr.IsValid)
		{
			return (true, $"You guessed {i} and it is correct!");
		}
		else
		{
			return (false, $"You have chosen {i} and it is wrong. " + vr.ToString());
		}
	}
}

	public class ObjToValidate
	{
		public int I { get; set; }
	}

public class ValidationParametersOptions
{
	public int IGreaterThanValue { get; set; }
}
#pragma warning restore S3903 // Types should be defined in named namespaces

