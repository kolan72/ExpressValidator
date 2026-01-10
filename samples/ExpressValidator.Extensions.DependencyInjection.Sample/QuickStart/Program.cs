using ExpressValidator.Extensions.DependencyInjection;
using FluentValidation;
using Shared;

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
});

await app.RunAsync();
