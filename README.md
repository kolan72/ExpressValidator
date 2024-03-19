# ExpressValidator

ExpressValidator is a library that provides the ability to validate objects using the `FluentValidation` library, but without object inheritance from `AbstractValidator`.


## Key Features

- Easy on-the-fly creation of object validator class called `ExpressValidator` by using `ExpressValidatorBuilder`.
- Supports asynchronous validation.
- Controls for a property expression to be a property.
- Targets .NET Standard 2.0+

## Usage

```csharp
//Class we want to validate
public class ObjWithTwoPProps
{
    public int I { get; set; }
    public string S { get; set; }
}

var result = new ExpressValidatorBuilder<ObjWithTwoProps>()
				//Choose property to validate
				.AddProperty(o => o.I)
				//Usual FluentValidation rules here
				.WithValidation(o => o.GreaterThan(0))
				//Choose other property
				.AddProperty(o => o.S)
				//And set rules again
				.WithValidation(o => o.MaximumLength(1))
				//We get IExpressValidator<ObjWithTwoProps> after calling the Build method
				.Build()
    			//And finally validate the object
				.Validate(new ObjWithTwoPublicProps() { I = i, S = s });
if(!results.IsValid)
{
    //As usual with validation result...
}
```

## Drawbacks

- Non-canonical way of using of FluentValidation.
- Behind the scenes, there is a subclass of `AbstractValidator` for each validated property, rather than one for the whole object.
- Workaround for validating a property with a null value.