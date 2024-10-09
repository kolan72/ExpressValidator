using ExpressValidator;
using ExpressValidator.Extensions.DependencyInjection;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddExpressValidator<ObjToValidate>(b => 
								b.AddProperty(o => o.I)
								.WithValidation(o => o.GreaterThan(5)
								.WithMessage("Must be greater than 5!")));

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
}
);
app.Run();


#pragma warning disable S3903 // Types should be defined in named namespaces
public interface IGuessTheNumberService
{
	(bool Result, string Message) Guess();
}

public class GuessTheNumberService : IGuessTheNumberService
{
	private readonly IExpressValidator<ObjToValidate> _expressValidator;
	public GuessTheNumberService(IExpressValidator<ObjToValidate> expressValidator)
	{
		_expressValidator = expressValidator;
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
			return (false, $"You chose {i} and it is wrong. " + vr.ToString());
		}
	}
}

public class ObjToValidate
{
	public int I { get; set; }
}
#pragma warning restore S3903 // Types should be defined in named namespaces

