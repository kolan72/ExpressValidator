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

builder.Services.AddTransient<ISomeServiceThatUseIExpressValidator, SomeServiceThatUseIExpressValidator>();

var app = builder.Build();

...

interface ISomeServiceThatUseIExpressValidator
{
	void ValidateByValidator(ObjToValidate objToValidate);
	void ValidateByBuilder(ObjToValidate objToValidate);
}


class SomeServiceThatUseIExpressValidator : ISomeServiceThatUseIExpressValidator
{
	private readonly IExpressValidator<ObjToValidate> _expressValidator;
	private readonly IExpressValidatorBuilder<ObjToValidate, ObjectToValidateOptions> _expressValidatorBuilder;

	private readonly ObjectToValidateOptions _validateOptions;

	public SomeServiceThatUseIExpressValidator(	IExpressValidator<ObjToValidate> expressValidator,
												IExpressValidatorBuilder<ObjToValidate, ObjectToValidateOptions> expressValidatorBuilder, 
												IOptions<ObjectToValidateOptions> validateOptions)
	{
		_expressValidator = expressValidator;
		_validateOptions = validateOptions.Value;
		_expressValidatorBuilder = expressValidatorBuilder;
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

	private void ChangeOptions()
	{
		_validateOptions.IGreaterThanValue = ...;
	}
}

class ObjToValidate
{
	public int I { get; set; }
}

class ObjectToValidateOptions
{
	public int IGreaterThanValue { get; set; }
}
```

See samples folder for concrete example.
