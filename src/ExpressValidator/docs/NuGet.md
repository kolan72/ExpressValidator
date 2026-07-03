ExpressValidator is a library that provides the ability to validate objects using the `FluentValidation` library, but without object inheritance from `AbstractValidator`.


## Key Features

- Easy on-the-fly creation of object validator class called `ExpressValidator` by using `ExpressValidatorBuilder`.
- Possibility to dynamically change the parameters of the `FluentValidation` validators.
- Supports adding a property or field for validation via `AddProperty`, `AddField`, or the unified `AddMember` method.
- Verifies that a property expression is a property and a field expression is a field, and throws `ArgumentException` if it is not.
- Supports adding a `Func` that provides a value for validation.
- Built-in `null` tolerance - `null` root instances fail validation instead of throwing exceptions.
- Quick and easy validation with `QuickValidator`, with robust support for `null` values.
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

### Using `AddMember`

`AddMember` accepts either a property or a field expression, so you don't need to choose between `AddProperty` and `AddField` at the call site:

```csharp
// Works for both properties and fields interchangeably.
var result = new ExpressValidatorBuilder<ObjToValidate>()
				.AddMember(o => o.I)
				.WithValidation(rbo => rbo.GreaterThan(0))
				.AddMember(o => o._sField)
				.WithValidation(rbo => rbo.MinimumLength(1))
				.Build()
				.Validate(new ObjToValidate() { I = i, _sField = sf });
if(!result.IsValid)
{
    //As usual with validation result...
}
```

Passing an expression that is not a property or field access (e.g. a method call) throws `ArgumentException`.

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
It is also tolerant of `null` values, i.e., it avoids exceptions when the input is null.

## Composition-Based Property Validation with SetExpressValidator

**New Feature:** ExpressValidator now provides the `SetExpressValidator` extension method for `IRuleBuilder<T, TProperty>`, enabling composition-based property validation **without inheriting from `PropertyValidator`**.

### Why Use SetExpressValidator?

In FluentValidation, creating custom property validators requires inheriting from `PropertyValidator<T, TProperty>` and implementing validation logic in a separate class. With `SetExpressValidator`, you can define complex property validation rules inline using the familiar `ExpressValidatorBuilder` API, avoiding the need for inheritance.

### Key Benefits

- **No inheritance required** - Define validators inline without creating separate `PropertyValidator` classes
- **Composition over inheritance** - Build complex validators by composing existing rules
- **Configurable validation** - Use options to parameterize validation rules
- **Custom error messages** - Define message templates with dynamic arguments
- **Seamless integration** - Works naturally with FluentValidation's `RuleFor` syntax

### Example: Validating Complex Properties

```csharp
public class CatPerson
{
    public IList<Cat> Cats { get; set; } = new List<Cat>();
    public int Id { get; set; } = 20;
}

public class Cat {...}

public class CatsOptions
{
    public int CatsCount { get; set; }
    public int MinimumCats { get; set; }
}

public class CatPersonValidator : AbstractValidator<CatPerson>
{
    public CatPersonValidator()
    {
        // Validate the Cats collection's Count property using SetExpressValidator
        RuleFor(person => person.Cats)
            .SetExpressValidator(
                builder => builder
                    .Configure(b =>
                        b.AddProperty(p => p.Count)
                            .WithValidation((options, prop) =>
                                prop.LessThan(options.CatsCount)
                                    .GreaterThanOrEqualTo(options.MinimumCats)))
                    .WithMessageTemplate((ctx, value, result) =>
                        "{PropertyName} must contain fewer than {MaxElements} items " +
                        "and greater than or equal {MinElements} items.")
                    .WithTemplateArgument("MaxElements", o => o.CatsCount)
                    .WithTemplateArgument("MinElements", o => o.MinimumCats),
                new CatsOptions { CatsCount = 14, MinimumCats = 1 });

        // Validate Id using a simple inline validator
        RuleFor(person => person.Id)
            .SetExpressValidator(
                config => config.Configure(b =>
                    b.AddFunc(id => id, "Id")
                        .WithValidation((maxValue, prop) =>
                            prop.LessThan(maxValue))),
                1);
    }
}

// Usage
var validator = new CatPersonValidator();
var person = new CatPerson 
{ 
    Cats = new List<Cat> { new Cat(), new Cat() }, 
    Id = 0 
};

var result = validator.Validate(person);
// result.IsValid == true
```

### Comparison with FluentValidation's Approach

**Traditional FluentValidation (requires inheritance):**
```csharp
// Separate class required
public class CatsCountValidator : PropertyValidator<CatPerson, IList<Cat>>
{
    private readonly CatsOptions _options;
    
    public CatsCountValidator(CatsOptions options)
    {
        _options = options;
    }
    
    public override bool IsValid(ValidationContext<CatPerson> context, IList<Cat> value)
    {
        // Manual validation logic
        return value.Count < _options.CatsCount && 
               value.Count >= _options.MinimumCats;
    }
    
    // Manual error message handling
}

// Usage in validator
RuleFor(p => p.Cats).SetValidator(new CatsCountValidator(options));
```

**ExpressValidator's SetExpressValidator (no inheritance):**
```csharp
// Inline definition - no separate class needed
RuleFor(person => person.Cats)
    .SetExpressValidator(
        builder => builder.Configure(b =>
            b.AddProperty(p => p.Count)
                .WithValidation((o, p) => 
                    p.LessThan(o.CatsCount)
                     .GreaterThanOrEqualTo(o.MinimumCats))),
        new CatsOptions { CatsCount = 14, MinimumCats = 1 });
```

### Advanced Features

**Custom Message Templates:**
```csharp
.WithMessageTemplate((context, value, validationResult) =>
    $"Custom error for {context.DisplayName}: {validationResult.Errors.Count} errors")
```

**Template Arguments:**
```csharp
.WithTemplateArgument("MaxValue", options => options.MaxValue)
.WithTemplateArgument("MinValue", options => options.MinValue)
```

**Multiple Property Validation:**
```csharp
builder.Configure(b => b
    .AddProperty(obj => obj.Property1)
        .WithValidation((opts, prop) => prop.NotEmpty())
    .AddProperty(obj => obj.Property2)
        .WithValidation((opts, prop) => prop.MaximumLength(opts.MaxLength)))
```

## Nuances Of Using The Library

For `ExpressValidatorBuilder` methods (`AddFunc`, `AddProperty`, `AddField`, and `AddMember`), the overridden property name (set via `FluentValidation`'s `OverridePropertyName` method in `With(Async)Validation`) takes precedence over the property name passed as a string or via `Expression` in `AddFunc`/`AddProperty`/`AddField`/`AddMember`.  
For example, for the `ObjToValidate` object from the 'Quick Start' chapter, `result.Errors[0].PropertyName` will equal "percentSum" (the property name overridden in the validation rule):
```csharp
// result.Errors[0].PropertyName == "percentSum"
var result = new ExpressValidatorBuilder<ObjToValidate>()
		.AddFunc(o => o.PercentValue1 + o.PercentValue2, "sum")
		.WithValidation((o) => o.InclusiveBetween(0, 100)
			.OverridePropertyName("percentSum"))
		.BuildAndValidate(new ObjToValidate() { PercentValue1 = 200});
```