# ExpressValidator

ExpressValidator is a library that provides the ability to validate objects using the `FluentValidation` library, but without object inheritance from `AbstractValidator`.  

![ExpressValidator](ExpressValidator.png)

[![nuget](https://img.shields.io/nuget/v/ExpressValidator?style=plastic)](https://www.nuget.org/packages/ExpressValidator/) [![nuget](https://img.shields.io/nuget/v/ExpressValidator.Extensions.DependencyInjection?style=plastic&labelColor=lightblue&color=blue)](https://www.nuget.org/packages/ExpressValidator.Extensions.DependencyInjection)    



## Key Features

- Easy on-the-fly creation of object validator class called `ExpressValidator` by using `ExpressValidatorBuilder`.
- Possibility to dynamically change the parameters of the `FluentValidation` validators (since _version_ 0.0.21).
- Supports adding a property or field for validation.
- Verifies that a property expression is a property and a field expression is a field, and throws `ArgumentException` if it is not.
- Supports adding a `Func` that provides a value for validation.
- Supports asynchronous validation.
- Targets .NET Standard 2.0+

## Documentation

For details, please check the [API documentation](https://kolan72.github.io/ExpressValidator/api/ExpressValidator.html).


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

To dynamically change the parameters of the `FluentValidation` validators:  

1. Create an options object that contains the parameters of validators.  
2. Configure the `ExpressValidatorBuilder<TObj, TOptions>` builder using the options object.  
3. Pass the options to the builder's `Build` method.
4. Created `IExpressValidator<TObj>` validator will validate an a `TObj` object using parameters from the options object.

To validate an object with different parameters, simply rebuild the validator using the same builder with the different options.  

See example below.  
```csharp
//Object with options
var objToValidateOptions = new ObjToValidateOptions()
			{
				IGreaterThanValue = 0,
				SMaximumLengthValue = 1,
				SFieldMaximumLengthValue = 1,
				PercentSumMinValue = 0,
				PercentSumMaxValue = 100,
			};


var builder = new ExpressValidatorBuilder<ObjToValidate, ObjToValidateOptions>()
			.AddProperty(o => o.I)
			//Get Greater Than validator parameter from options
			.WithValidation((to, p) => p.GreaterThan(to.IGreaterThanValue))
			.AddProperty(o => o.S)
			//Get MaxLength validator parameter from options
			.WithValidation((to, p)=> p.MaximumLength(to.SMaximumLengthValue))
			.AddField(o => o._sField)
			//Get MaxLength validator parameter from options for field
			.WithValidation((to, f) => f.MaximumLength(to.SFieldMaximumLengthValue))
			.AddFunc(o => o.PercentValue1 + o.PercentValue2, "percentSum")
			//Get InclusiveBetween validator parameters from options
			.WithValidation((to, f) => f.InclusiveBetween(to.PercentSumMinValue, to.PercentSumMaxValue));

//ValidationResult with parameters from objToValidateOptions
var result = builder	
			//Pass options in the Build method
			.Build(objToValidateOptions)
			.Validate(new ObjToValidate() { I = i, S = s, _sField = sf, PercentValue1 = pv1, PercentValue2 = pv2 });
				
if(!result.IsValid)
{
...
}		

var objToValidateOptions2 = new ObjToValidateOptions() {...};

var result2 = builder	
			//Pass other options in the Build method
			.Build(objToValidateOptions2)
			.Validate(new ObjToValidate() { I = i, S = s, _sField = sf, PercentValue1 = pv1, PercentValue2 = pv2 });

//Check IsValid after rebuild
if(!result2.IsValid)
{
...
}
```

## Drawbacks

- Non-canonical way of using of FluentValidation.
- Behind the scenes, there is a subclass of `AbstractValidator` for each validated property, rather than one for the whole object.
- Workaround for validating a property with a null value.