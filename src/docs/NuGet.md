ExpressValidator is a library that provides the ability to validate objects using the `FluentValidation` library, but without object inheritance from `AbstractValidator`.


## Key Features

- Easy on-the-fly creation of object validator class called `ExpressValidator` by using `ExpressValidatorBuilder`.
- Supports asynchronous validation.
- Verifies that a property expression is a property and a field expression is a field, and throws `ArgumentException` if it is not.
- Targets .NET Standard 2.0+

## Usage

```csharp
//Class we want to validate
public class ObjWithTwoProps
{
    public int I { get; set; }
    public string S { get; set; }
	public string _sField;
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
				//Choose field to validate
				.AddField(o => o._sField)
				//And set rules for the field
				.WithValidation(o => o.MinimumLength(1))
				//We get IExpressValidator<ObjWithTwoProps> after calling the Build method
				.Build()
	 			//And finally validate the object
				.Validate(new ObjWithTwoProps() { I = i, S = s, _sField = sf });
if(!result.IsValid)
{
    //As usual with validation result...
}
```