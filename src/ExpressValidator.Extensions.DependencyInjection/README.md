The `ExpressValidator.Extensions.DependencyInjection` package extends `ExpressValidator` to provide integration with Microsoft Dependency Injection.

## 🔑 Key Features

- Configures and adds the `IExpressValidator<ObjToValidate>` interface in Microsoft's Dependency Injection (DI) container.
- Additionally, the `IExpressValidatorBuilder<ObjToValidate, ValidationParametersOptions>` interface can be configured and registered to update the validator parameters when the `ValidationParametersOptions` change.
- Ability to dynamically update the validator parameters from options bound to the configuration section without restarting the application by configuring the `IExpressValidatorWithReload<ObjToValidate>` interface.

## 📜 Documentation

Explore the API documentation and in-depth details on [DeepWiki](https://deepwiki.com/kolan72/ExpressValidator/3-dependency-injection-extension).

## 🚀 Quick Start

Register an `IExpressValidator<T>` implementation in the dependency injection (DI) container using the `AddExpressValidator` method, then inject and use it in a consuming service:

```csharp
using ExpressValidator;
using ExpressValidator.Extensions.DependencyInjection;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);

// Registers the validator for ObjToValidate with specified validation rules.
builder.Services.AddExpressValidator<ObjToValidate>(b => 
        b.AddProperty(o => o.I)
	    .WithValidation(o => o.GreaterThan(5)
	    .WithMessage("Must be greater than 5!")));

// Registers the service that will use the validator.
builder.Services.AddTransient<IGuessTheNumberService, GuessTheNumberService>();

var app = builder.Build();

app.MapGet("/guess", (IGuessTheNumberService service) =>
{
	var (Result, Message) = service.Guess();
	if (!Result)
	{
		return Results.BadRequest(Message);
	}
	// Additional logic here...
});

await app.RunAsync();

// ... (Other code omitted for brevity)

// Service interface definition.
public interface IGuessTheNumberService
{
	(bool Result, string Message) Guess();
}

// Service implementation that uses the validator.
public class GuessTheNumberService : IGuessTheNumberService
{
	private readonly IExpressValidator<ObjToValidate> _expressValidator;

	public GuessTheNumberService(IExpressValidator<ObjToValidate> expressValidator)
	{
		_expressValidator = expressValidator;
	}

	public (bool Result, string Message) Guess()
	{
		...
		var vr = _expressValidator.Validate(objToValidate);
		if (!vr.IsValid)
		{
			...
		}
		// ... (Additional logic)
	}
}
// ... (Other code omitted for brevity)
```

## 🛠️ Quick Start: Using a `ValidatorConfigurator<T>` (Alternative Approach)

As an alternative to inline configuration, you can define validation rules by creating a dedicated configurator class that inherits from `ValidatorConfigurator<T>`, where `T` is the type being validated:

```csharp
/// <summary>
/// Configures validation rules for ObjToValidate.
/// </summary>
public class GuessValidatorConfigurator : ValidatorConfigurator<ObjToValidate>
{
	/// <summary>
    /// Configures the validator builder with rules.
    /// </summary>
	public override void Configure(ExpressValidatorBuilder<ObjToValidate> expressValidatorBuilder)
		=> expressValidatorBuilder
			.AddProperty(o => o.I)
			.WithValidation((o) => o.GreaterThan(5));
}
```

Then use `AddExpressValidation` method to register the configurator in DI:

```csharp
var builder = WebApplication.CreateBuilder(args);

// Scans the assembly and registers validators from configurators.
builder.Services.AddExpressValidation(Assembly.GetExecutingAssembly());

// Registers the service that will use the validator.
builder.Services.AddTransient<IGuessTheNumberService, GuessTheNumberService>();

// ... (Application build and run code omitted; same as in Quick Start)

// The GuessTheNumberService implementation remains the same as in the Quick Start example.
// ... (Other code omitted for brevity)
```

## ⚙️ Validation with Options

In this approach, register an `IExpressValidatorBuilder<T, TOptions>` implementation (instead of `IExpressValidator<T>`) in the DI container by calling the `AddExpressValidatorBuilder` method.

```csharp
var builder = WebApplication.CreateBuilder(args);

// Registers the validator builder with options-dependent rules.
builder.Services.AddExpressValidatorBuilder<ObjToValidate, ValidationParametersOptions>(b =>
	b.AddProperty(o => o.I)
	.WithValidation((to, rbo) => rbo.GreaterThan(to.IGreaterThanValue)
	.WithMessage($"Must be greater than {to.IGreaterThanValue}!")));

builder.Services.AddTransient<IAdvancedNumberGuessingService, AdvancedNumberGuessingService>();

// Configures options from the application settings.
builder.Services.Configure<ValidationParametersOptions>(builder.Configuration.GetSection("ValidationParameters"));

var app = builder.Build();

app.MapGet("/complexguess", (IAdvancedNumberGuessingService service) =>
{
	var (Result, Message) = service.ComplexGuess();
	if (!Result)
	{
		return Results.BadRequest(Message);
	}
	// Additional logic here...
});

app.Run();

// Service interface definition:
public interface IAdvancedNumberGuessingService
{
	(bool Result, string Message) ComplexGuess();
}

// Service implementation that builds and uses the validator with options.
public class AdvancedNumberGuessingService : IAdvancedNumberGuessingService
{
	private readonly ValidationParametersOptions _validateOptions;
	private readonly IExpressValidatorBuilder<ObjToValidate, ValidationParametersOptions> _expressValidatorBuilder;

	public AdvancedNumberGuessingService(IExpressValidatorBuilder<ObjToValidate, ValidationParametersOptions> expressValidatorBuilder,
		IOptions<ValidationParametersOptions> validateOptions)
	{
		_validateOptions = validateOptions.Value;
		_expressValidatorBuilder = expressValidatorBuilder;
	}

	//Updates options, rebuilds the validator, and validates.
	public (bool Result, string Message) ComplexGuess()
	{
		...
		ChangeValidateOptions();

		var vr = _expressValidatorBuilder.Build(_validateOptions).Validate(objToValidate);
		if (!vr.IsValid)
		{
			// ... (Handle invalid case)
		}
		// ... (Additional logic)
	}

	private void ChangeValidateOptions()
	{
		// ... (Option update logic omitted)
	}
}
```
In the *appsettings.json*

```csharp
{
// ... (Other settings omitted)
"ValidationParameters": {
  "IGreaterThanValue": 5
 }
}
```

## 🔥 Validation with Automatic Reload on Configuration Changes

To validate options when configuration changes - without restarting the application - use the `AddExpressValidatorWithReload` method:

```csharp
var builder = WebApplication.CreateBuilder(args);

// Registers a reloadable validator that updates on configuration changes.
builder.Services.AddExpressValidatorWithReload<ObjToValidate, ValidationParametersOptions>(b =>
	b.AddProperty(o => o.I)
	.WithValidation((to, rbo) => rbo.GreaterThan(to.IGreaterThanValue)
	.WithMessage($"Must be greater than {to.IGreaterThanValue}!")),
	"ValidationParameters");

// Registers the reloadable service.
builder.Services.AddTransient<IReloadableNumberGuessingService, ReloadableNumberGuessingService>();

// Configures options from the application settings.
builder.Services.Configure<ValidationParametersOptions>(builder.Configuration.GetSection("ValidationParameters"));

var app = builder.Build();

app.MapGet("/guesswithreload", (IReloadableNumberGuessingService service) =>
{
	var (Result, Message) = service.GuessWithReload();
	if (!Result)
	{
		return Results.BadRequest(Message);
	}
	// Additional logic here...
});

// Service interface definition.
public interface IReloadableNumberGuessingService
{
	(bool Result, string Message) GuessWithReload();
}

// Service implementation that uses the reloadable validator.
public class ReloadableNumberGuessingService : IReloadableNumberGuessingService
{
	private readonly IExpressValidatorWithReload<ObjToValidate> _expressValidatorWithReload;

	public ReloadableNumberGuessingService(IExpressValidatorWithReload<ObjToValidate> expressValidatorWithReload)
	{
		_expressValidatorWithReload = expressValidatorWithReload;
	}

	public (bool Result, string Message) GuessWithReload()
	{
		...
		var vr = _expressValidatorWithReload.Validate(objToValidate);
		if (!vr.IsValid)
		{
			...
		}
	}
}
```

## 🏆 Sample

See samples folder for concrete example. [![CSharp](https://img.shields.io/badge/C%23-code-blue.svg)](../../samples/ExpressValidator.Extensions.DependencyInjection.Sample)
