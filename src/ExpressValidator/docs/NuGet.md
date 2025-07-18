ExpressValidator is a library that provides the ability to validate objects using the `FluentValidation` library, but without object inheritance from `AbstractValidator`.


## Key Features

- Easy on-the-fly creation of object validator class called `ExpressValidator` by using `ExpressValidatorBuilder`.
- Possibility to dynamically change the parameters of the `FluentValidation` validators.
- Supports adding a property or field for validation.
- Verifies that a property expression is a property and a field expression is a field, and throws `ArgumentException` if it is not.
- Supports adding a `Func` that provides a value for validation.
- Provides quick and easy validation using the `QuickValidator`.
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

## Modifying FluentValidation Validator Parameters Using Options

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

## Quick Validation

Quick validation is convenient for primitive types or types without properties/fields (here, 'quick' refers to usability, not performance). Simply call `QuickValidator.Validate` on the object with a preconfigured rule:

```csharp
var value = 5;
// result.IsValid == false
// result.Errors[0].PropertyName == "value"
var result = QuickValidator.Validate(
	value,
	(opt) => opt.GreaterThan(10),
	nameof(value));
```

For complex types, use FluentValidation's `ChildRules` method:

```csharp
var obj = new ObjToValidate() { I = -1, PercentValue1 = 101 };
// result.IsValid == false
// result.Errors.Count == 2
// result.Errors[0].PropertyName == "obj.I"; result.Errors[1].PropertyName == "obj.PercentValue1"
var result = QuickValidator.Validate(
	obj,
	(opt) =>
		opt
		.ChildRules((v) => v.RuleFor(o => o.I).GreaterThan(0))
		.ChildRules((v) => v.RuleFor(o => o.PercentValue1).InclusiveBetween(0, 100)),
	nameof(obj));
```
The `QuickValidator` also provides a `ValidateAsync` method for asynchronous validation.

## Nuances Of Using The Library

For `ExpressValidatorBuilder` methods (`AddFunc`, `AddProperty`, and `AddField`), the overridden property name (set via  `FluentValidation`'s `OverridePropertyName` method in `With(Async)Validation`) takes precedence over the property name passed as a string or via `Expression` in  `AddFunc`/`AddProperty`/`AddField`.  
For example, for the `ObjToValidate` object from the 'Quick Start' chapter, `result.Errors[0].PropertyName` will equal "percentSum" (the property name overridden in the validation rule):
```csharp
// result.Errors[0].PropertyName == "percentSum"
var result = new ExpressValidatorBuilder<ObjToValidate>()
		.AddFunc(o => o.PercentValue1 + o.PercentValue2, "sum")
		.WithValidation((o) => o.InclusiveBetween(0, 100)
			.OverridePropertyName("percentSum"))
		.BuildAndValidate(new ObjToValidate() { PercentValue1 = 200});
```