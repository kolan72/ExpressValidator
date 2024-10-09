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

class SomeServiceThatUseIExpressValidator : ISomeServiceThatUseIExpressValidator
{
	private readonly IExpressValidator<ObjToValidate> _expressValidator;
	public SomeServiceThatUseIExpressValidator(IExpressValidator<ObjToValidate> expressValidator)
	{
		_expressValidator = expressValidator;
	}

	public void DoSomething()
	{
		var vr = _expressValidator.Validate(objToValidate);
		if(vr.IsValid)
		{
		...
		}
	}
}
```

See samples folder for concrete example.
