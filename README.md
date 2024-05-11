# ExpressValidator

[![nuget](https://img.shields.io/nuget/v/ExpressValidator)](https://www.nuget.org/packages/ExpressValidator/)  

ExpressValidator is a library that provides the ability to validate objects using the `FluentValidation` library, but without object inheritance from `AbstractValidator`.


## Key Features

- Easy on-the-fly creation of object validator class called `ExpressValidator` by using `ExpressValidatorBuilder`.
- Supports adding a property or field for validation.
- Verifies that a property expression is a property and a field expression is a field, and throws `ArgumentException` if it is not.
- Supports adding a `Func` that provides a value for validation.
- Supports asynchronous validation.
- Targets .NET Standard 2.0+

## Usage

```csharp
//Class we want to validate
public class ObjToValidate
{
	public int I { get; set; }
	public string S { get; set; }
	public string _sField;
	public int PercentValue1 { get; set; }
	public int PercentValue2 { get; set; }
}

var result = new ExpressValidatorBuilder<ObjToValidate>()
				//Choose property to validate
				.AddProperty(o => o.I)
				//Usual FluentValidation rules here
				.WithValidation(o => o.GreaterThan(0))
				//Choose other property
				.AddProperty(o => o.S)
				//And set rules again
				.WithValidation(o => o.MaximumLength(1))
				//Choose field to validate
				.AddField(o => o._sField)
				//And set rules for the field
				.WithValidation(o => o.MinimumLength(1))
				//Add the Func that returns sum of percentage properties for validation
				.AddFunc(o => o.PercentValue1 + o.PercentValue2, "percentSum")
				//And set rules for the sum of percentages
				.WithValidation(o => o.InclusiveBetween(0, 100))
				//We get IExpressValidator<ObjToValidate> after calling the Build method
				.Build()
	 			//And finally validate the object
				.Validate(new ObjToValidate() { I = i, S = s, PercentValue1 = pv1, PercentValue2 = pv2 });
if(!result.IsValid)
{
    //As usual with validation result...
}
```

## Drawbacks

- Non-canonical way of using of FluentValidation.
- Behind the scenes, there is a subclass of `AbstractValidator` for each validated property, rather than one for the whole object.
- Workaround for validating a property with a null value.