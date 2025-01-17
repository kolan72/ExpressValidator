The `ExpressValidator.Extensions.DependencyInjection` package extends `ExpressValidator` to provide integration with Microsoft Dependency Injection.

## Usage

```csharp
using ExpressValidator;
using ExpressValidator.Extensions.DependencyInjection;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddExpressValidator<ObjToValidate>(b => 
        b.AddProperty(o => o.I)
	    .WithValidation(o => o.GreaterThan(5)
	    .WithMessage("Must be greater than 5!")));

builder.Services.AddExpressValidatorBuilder<ObjToValidate, ValidationParametersOptions>
		(b => b.AddProperty(o => o.I)
		.WithValidation((to, rbo) => rbo.GreaterThan(to.IGreaterThanValue)
		.WithMessage($"Must be greater than {to.IGreaterThanValue}!")));

builder.Services.AddExpressValidatorWithReload<ObjToValidate, ValidationParametersOptions>(b =>
		b.AddProperty(o => o.I)
		.WithValidation((to, rbo) => rbo.GreaterThan(to.IGreaterThanValue)
		.WithMessage($"Must be greater than {to.IGreaterThanValue}!")),
		//Configuration section path
		"ValidationParameters");

builder.Services.AddTransient<ISomeServiceThatUseIExpressValidator, SomeServiceThatUseIExpressValidator>();

var app = builder.Build();

...

interface ISomeServiceThatUseIExpressValidator
{
	void ValidateByValidator(ObjToValidate objToValidate);
	void ValidateByBuilder(ObjToValidate objToValidate);
	void ValidateByValidatorWithReload(ObjToValidate objToValidate);
}

class SomeServiceThatUseIExpressValidator : ISomeServiceThatUseIExpressValidator
{
	private readonly IExpressValidator<ObjToValidate> _expressValidator;
	private readonly IExpressValidatorBuilder<ObjToValidate, ValidationParametersOptions> _expressValidatorBuilder;
	private readonly IExpressValidatorWithReload<ObjToValidate> _expressValidatorWithReload;

	private readonly ValidationParametersOptions _validateOptions;

	public SomeServiceThatUseIExpressValidator(
				IExpressValidator<ObjToValidate> expressValidator,
				IExpressValidatorBuilder<ObjToValidate, ValidationParametersOptions> expressValidatorBuilder, 
				IExpressValidatorWithReload<ObjToValidate> expressValidatorWithReload
				IOptions<ValidationParametersOptions> validateOptions)
	{
		_expressValidator = expressValidator;
		_expressValidatorBuilder = expressValidatorBuilder;
		_expressValidatorWithReload = expressValidatorWithReload;
		_validateOptions = validateOptions.Value; 
	}

	public void ValidateByValidator(ObjToValidate objToValidate)
	{
		var vr = _expressValidator.Validate(objToValidate);
		if(vr.IsValid)
		{
		...
		}
	}

	public void ValidateByBuilder(ObjToValidate objToValidate)
	{
		ChangeOptions();
		var vr = _expressValidatorBuilder
				.Build(_validateOptions)
				.Validate(objToValidate);
		if(vr.IsValid)
		{
		...
		}						
	}

	//Change the options in the configuration section path named "ValidationParameters" 
	//and use this method to revalidate the object without restarting the application.
	public void ValidateByValidatorWithReload(ObjToValidate objToValidate)
	{
		var vr = _expressValidatorWithReload.Validate(objToValidate);
		if(vr.IsValid)
		{
		...
		}
	}

	private void ChangeOptions()
	{
		_validateOptions.IGreaterThanValue = ...;
	}
}

class ObjToValidate
{
	public int I { get; set; }
}

class ValidationParametersOptions
{
	public int IGreaterThanValue { get; set; }
}
```

In the *appsettings.json*

```csharp
{
...
"ValidationParameters": {
  "IGreaterThanValue": 5
 }
...
}
```


See samples folder for concrete example.
