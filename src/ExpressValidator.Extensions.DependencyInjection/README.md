The `ExpressValidator.Extensions.DependencyInjection` package extends `ExpressValidator` to provide integration with Microsoft Dependency Injection.

## 🔑 Key Features

- **Automatic DI Registration**: Configures and registers `IExpressValidator<T>` with Microsoft's Dependency Injection container.
- **Class-Based Configuration**: Define validation rules via dedicated configurator classes inheriting from `ValidatorConfigurator<T>` (static rules) or `ValidatorConfigurator<T, TOptions>` (configuration-driven rules with automatic options binding and hot reload), providing an alternative to inline configuration.
- **Dynamic Parameter Updates**: Registers `IExpressValidatorBuilder<T, TOptions>` to automatically update validation parameters when configuration options change.
- **Automatic Reload Capability**: Automatically reload validation rules when configuration changes using `IExpressValidatorWithReload<T>`.

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

If you prefer not to pass an explicit `Assembly`, two convenience helpers are also available:

- `AddExpressValidationFromAssemblyContaining<T>()` - scans the assembly that contains `T`
- `AddExpressValidationFromCurrentAssembly()` - scans the calling assembly

## 🛠️ Quick Start: Using a `ValidatorConfigurator<T, TOptions>` (Class-Based Configuration with Options)

For configuration-driven rules, inherit from `ValidatorConfigurator<T, TOptions>`, where `T` is the type being validated and `TOptions` is the options type. Rules are defined dynamically from options that are automatically bound from a configuration section, and the validator is automatically rebuilt whenever that section changes - without restarting the application:

```csharp
/// <summary>
/// Configures validation rules for ObjToValidate using configuration-driven options.
/// </summary>
public class GuessValidatorConfigurator : ValidatorConfigurator<ObjToValidate, GuessValidationOptions>
{
	// Binds GuessValidationOptions from the "GuessValidation" configuration section.
	public override string ConfigSectionPath => "GuessValidation";

	public override void Configure(ExpressValidatorBuilder<ObjToValidate, GuessValidationOptions> expressValidatorBuilder)
		=> expressValidatorBuilder
			.AddProperty(o => o.I)
			.WithValidation((val, options) => options
				.GreaterThanOrEqualTo(val.MinValue)
				.WithMessage($"Value must be at least {val.MinValue}")
				.LessThanOrEqualTo(val.MaxValue)
				.WithMessage($"Value must be at most {val.MaxValue}"));
}

// Options class bound from the "GuessValidation" configuration section.
public class GuessValidationOptions
{
	public int MinValue { get; set; }
	public int MaxValue { get; set; }
}
```

In the *appsettings.json*

```json
{
  "GuessValidation": {
    "MinValue": 5,
    "MaxValue": 10
  }
}
```

The same `AddExpressValidation` methods used for `ValidatorConfigurator<T>` discover these configurators during assembly scanning. For each discovered `ValidatorConfigurator<T, TOptions>` the package:

- registers `IValidatorConfigurator<T, TOptions>`,
- binds `TOptions` from the section returned by `ConfigSectionPath` (with hot reload support),
- registers a singleton `IExpressValidatorWithReload<T>` proxy that automatically rebuilds the validator whenever the bound options change.

```csharp
var builder = WebApplication.CreateBuilder(args);

// Scans the assembly, registers configurators, binds options from each ConfigSectionPath,
// and registers IExpressValidatorWithReload<T> proxies.
builder.Services.AddExpressValidationFromCurrentAssembly();

// Registers the service that will use the reloadable validator.
builder.Services.AddTransient<IGuessTheNumberService, GuessTheNumberService>();

// ... (Application build and run code omitted; same as in Quick Start)

// Service implementation that uses the reloadable validator.
public class GuessTheNumberService : IGuessTheNumberService
{
	private readonly IExpressValidatorWithReload<ObjToValidate> _expressValidatorWithReload;

	public GuessTheNumberService(IExpressValidatorWithReload<ObjToValidate> expressValidatorWithReload)
	{
		_expressValidatorWithReload = expressValidatorWithReload;
	}

	public (bool Result, string Message) Guess()
	{
		...
		var vr = _expressValidatorWithReload.Validate(objToValidate);
		if (!vr.IsValid)
		{
			...
		}
		// ... (Additional logic)
	}
}
// ... (Other code omitted for brevity)
```

To consume a validator defined via `ValidatorConfigurator<T, TOptions>`, inject `IExpressValidatorWithReload<T>` - it is the service registered for this configurator kind, and it transparently rebuilds the validator when the bound configuration changes.

### ✅ Requirements for automatic registration

A `ValidatorConfigurator<T, TOptions>` class must:

1. Inherit from `ValidatorConfigurator<T, TOptions>`
2. Have a public parameterless constructor
3. Override `ConfigSectionPath` with a non-null, non-whitespace configuration section path

If any of these requirements are not met, a descriptive exception is thrown at startup.

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

See samples folder for concrete examples, including `ConfiguratorWithOptionsDemo`, which demonstrates `ValidatorConfigurator<T, TOptions>` with automatic configuration binding and hot reload. [![CSharp](https://img.shields.io/badge/C%23-code-blue.svg)](../../samples/ExpressValidator.Extensions.DependencyInjection.Sample)
