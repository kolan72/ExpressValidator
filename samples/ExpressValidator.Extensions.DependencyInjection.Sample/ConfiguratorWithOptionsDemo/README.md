# ConfiguratorWithOptionsDemo

This sample demonstrates how to use `ValidatorConfigurator<T, TOptions>` with automatic configuration binding and hot reload support.

## Features Demonstrated

1. **Automatic Registration**: The `GuessValidatorConfigurator` is automatically discovered and registered during `AddExpressValidationFromCurrentAssembly()`.

2. **Configuration Binding**: The `ConfigSectionPath` property tells the system to bind `GuessValidationOptions` from the "GuessValidation" configuration section.

3. **Hot Reload**: When `appsettings.json` changes (e.g., MinValue or MaxValue), the validator automatically rebuilds with the new values.

4. **No Manual Registration**: Unlike explicit registration methods, you only need to:
   - Inherit from `ValidatorConfigurator<T, TOptions>`
   - Implement `ConfigSectionPath` property
   - Have a public parameterless constructor

## Key Components

### GuessValidatorConfigurator
```csharp
public class GuessValidatorConfigurator : ValidatorConfigurator<ObjToValidate, GuessValidationOptions>
{
    public override string ConfigSectionPath => "GuessValidation";
    
    public override void Configure(ExpressValidatorBuilder<ObjToValidate, GuessValidationOptions> builder)
    {
        builder.AddProperty(o => o.I)
            .WithValidation((options, ruleBuilder) => ruleBuilder
                .GreaterThanOrEqualTo(options.MinValue)
                .LessThanOrEqualTo(options.MaxValue));
    }
}
```

### GuessValidationOptions
```csharp
public class GuessValidationOptions
{
    public int MinValue { get; set; }
    public int MaxValue { get; set; }
}
```

### Configuration (appsettings.json)
```json
{
  "GuessValidation": {
    "MinValue": 5,
    "MaxValue": 10
  }
}
```

## Running the Sample

1. Build and run the project:
   ```bash
   dotnet run
   ```

2. Test the endpoint:
   ```bash
   curl http://localhost:5000/guess
   ```

3. Try changing `MinValue` or `MaxValue` in `appsettings.json` while the app is running - the validator will automatically rebuild with the new values!

## Requirements

For automatic registration to work, your configurator must:

1. ✅ Inherit from `ValidatorConfigurator<T, TOptions>`
2. ✅ Have a public parameterless constructor
3. ✅ Implement `ConfigSectionPath` with a non-null/whitespace value

If any of these requirements are not met, the system will throw a clear exception at startup explaining what's wrong.

## Comparison with ConfiguratorDemo

- **ConfiguratorDemo**: Uses `ValidatorConfigurator<T>` with static rules
- **ConfiguratorWithOptionsDemo**: Uses `ValidatorConfigurator<T, TOptions>` with dynamic, configuration-driven rules that support hot reload
