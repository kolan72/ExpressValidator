using ExpressValidator;
using ExpressValidator.Extensions.DependencyInjection;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddExpressValidator<ObjToValidate>(b => 
								b.AddProperty(o => o.I)
								.WithValidation(o => o.GreaterThan(5)
								.WithMessage("Must be greater than 5!")));

builder.Services.AddExpressValidatorBuilder<ObjToValidate, ObjectToValidateOptions>(b =>
								b.AddProperty(o => o.I)
								.WithValidation((to, rbo) => rbo.GreaterThan(to.IGreaterThanValue)
								.WithMessage($"Must be greater than {to.IGreaterThanValue}!")));

builder.Services.AddTransient<IGuessTheNumberService, GuessTheNumberService>();

builder.Services.Configure<ObjectToValidateOptions>(builder.Configuration.GetSection("ObjectToValidateOptions"));

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

app.MapGet("/complexguess", (IGuessTheNumberService service) =>
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

await app.RunAsync();


#pragma warning disable S3903 // Types should be defined in named namespaces
public interface IGuessTheNumberService
{
	(bool Result, string Message) Guess();
	(bool Result, string Message) ComplexGuess();
}

public class GuessTheNumberService : IGuessTheNumberService
{
	private readonly IExpressValidator<ObjToValidate> _expressValidator;
	private readonly IExpressValidatorBuilder<ObjToValidate, ObjectToValidateOptions> _expressValidatorBuilder;

	private readonly ObjectToValidateOptions _validateOptions;

	private const string WIN_PHRASE = "The rules have changed in the middle of the game, but you still win!";
	private const string LOSE_PHRASE = "Sorry, the rules changed in the middle of the game.";

	public GuessTheNumberService(IExpressValidator<ObjToValidate> expressValidator, 
								IExpressValidatorBuilder<ObjToValidate, ObjectToValidateOptions> expressValidatorBuilder, 
								IOptions<ObjectToValidateOptions> validateOptions)
	{
		_expressValidator = expressValidator;
		_validateOptions = validateOptions.Value;
		_expressValidatorBuilder = expressValidatorBuilder;
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

	public (bool Result, string Message) ComplexGuess()
	{
		var i = Random.Shared.Next(1, 11);
		var objToValidate = new ObjToValidate() { I = i };

		ChangeValidateOptions();

		var vr = _expressValidatorBuilder.Build(_validateOptions).Validate(objToValidate);
		if (vr.IsValid)
		{
			return (true, WIN_PHRASE + " " +
							$"You guessed {i} and it is correct because it's greater than {_validateOptions.IGreaterThanValue}.");
		}
		else
		{
			return (false, LOSE_PHRASE + " " +
				$"You have chosen {i} and it is wrong. " + vr.ToString());
		}
	}

	private void ChangeValidateOptions()
	{
		_validateOptions.IGreaterThanValue = Random.Shared.Next(2, 10);
	}
}

public class ObjToValidate
{
	public int I { get; set; }
}

public class ObjectToValidateOptions
{
	public int IGreaterThanValue { get; set; }
}
#pragma warning restore S3903 // Types should be defined in named namespaces

