# Getting Started

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
				.WithValidation(rbo => rbo.GreaterThan(0))
				//Choose other property
				.AddProperty(o => o.S)
				//And set rules again
				.WithValidation(rbo => rbo.MaximumLength(1))
				//Choose field to validate
				.AddField(o => o._sField)
				//And set rules for the field
				.WithValidation(rbo => rbo.MinimumLength(1))
				//Add the Func that returns sum of percentage properties for validation
				.AddFunc(o => o.PercentValue1 + o.PercentValue2, "percentSum")
				//And set rules for the sum of percentages
				.WithValidation(rbo => rbo.InclusiveBetween(0, 100))
				//We get IExpressValidator<ObjToValidate> after calling the Build method
				.Build()
	 			//And finally validate the object
				.Validate(new ObjToValidate() { I = i, S = s, PercentValue1 = pv1, PercentValue2 = pv2 });
if(!result.IsValid)
{
    //As usual with validation result...
}
```