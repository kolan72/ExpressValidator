using ExpressValidator.Extensions.DependencyInjection;
using FluentValidation;
using Shared;
using ValidatorWithReload;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddExpressValidatorWithReload<ObjToValidate, ValidationParametersOptions>(b =>
								b.AddProperty(o => o.I)
								.WithValidation((to, rbo) => rbo.GreaterThan(to.IGreaterThanValue)
								.WithMessage($"Must be greater than {to.IGreaterThanValue}!")),
								"ValidationParameters");

builder.Services.AddTransient<IReloadableNumberGuessingService, ReloadableNumberGuessingService>();

builder.Services.Configure<ValidationParametersOptions>(builder.Configuration.GetSection("ValidationParameters"));

var app = builder.Build();

app.MapGet("/guesswithreload", (IReloadableNumberGuessingService service) =>
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

app.MapGet("/guesswithreloadasync", async (IReloadableNumberGuessingService service) =>
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
