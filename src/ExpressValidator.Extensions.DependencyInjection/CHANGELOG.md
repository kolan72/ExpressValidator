## 0.8.0

- Introduced `IValidatorConfigurator<T, TOptions>` and `ValidatorConfigurator<T, TOptions>`.
- Intoduced the `ProxyValidator<T, TOptions>` as the open-generic implementation of `IExpressValidatorWithReload<T>`.
- Implement `IDisposable` on `IOptionsMonitorContext` to prevent memory leaks.
- Add null-safety guards, use `TryAdd` for idempotent registration, and extract `ConfigureExpressValidatorOptions` and other methods to eliminate duplicated options configuration.
- Uses Interlocked for atomic snapshot updates and immutable `ValidatorSnapshot` class to prevent concurrent rebuild issues under high load.
- Use an immutable `OptionsSnapshot` class for atomic updates.
- Add a sample project demonstrating the `ValidatorConfigurator<T, TOptions>` functionality.
- Add NUnit tests for `ValidatorConfigurator<T,TOptions>` DI registration.
- Document `ValidatorConfigurator<T, TOptions>` in README and NuGet docs.
- Update Microsoft nuget packages.
- Update to ExpressValidator 0.20.0.
- Update tests to use ExpressValidator 0.20.0.
- Update Shared.csproj to ExpressValidator 0.20.0.


## 0.5.0

- Add `IServiceCollection` extensions `AddExpressValidationFromAssemblyContaining<T>` and `AddExpressValidationFromCurrentAssembly`.
- Change `ValidatorConfigurator<T>` to inherit from `ValidationProfile<T>`.
- Update to ExpressValidator 0.15.0 and FluentValidation 12.1.1.
- Update ExpressValidator package for ExpressValidator.Extensions.DependencyInjection.Tests.
- Update ConfiguratorDemo sample to use `AddExpressValidationFromCurrentAssembly`.
- Unify applicationUrl in launchSettings.json for all samples.
- Update Shared.csproj to ExpressValidator 0.15.0.
- Update NUnit NuGet package to v4.6.0.
- Update Microsoft nuget packages.


## 0.4.0

- Introduced class-based validation configuration with dedicated configurator classes inheriting from `ValidatorConfigurator<T>` and registered through `AddExpressValidation`.
- Update to ExpressValidator 0.12.2 and FluentValidation 12.1.0.
- Update Microsoft nuget packages.
- Edit NuGet README.
- Edit README.md.
- Add Shared.csproj to the ExpressValidator.Extensions.DependencyInjection.Sample.sln solution.
- Split sample project into multiple projects illustrating README-described features.


## 0.3.12

- Support .NET 8.0 and FluentValidation 12.0.0.
- Update Microsoft nuget packages.
- Update ExpressValidator NuGet package to v0.12.0.
- Update NUnit NuGet package to v4.4.0.
- Retarget ExpressValidator.Extensions.DependencyInjection.Sample to .NET 8.0.


## 0.3.9

- Update ExpressValidator nuget package.
- Update Microsoft nuget packages.
- Update Microsoft NuGet packages for ExpressValidator.Extensions.DependencyInjection.Tests.


## 0.3.7

- Update ExpressValidator nuget package.
- Split the DI extensions into a dedicated solution.
- Update Microsoft nuget packages.
- Update Microsoft NuGet packages for ExpressValidator.Extensions.DependencyInjection.Tests.


## 0.3.5

- Reduced unnecessary updates to validator parameters by listening to `IOptionsMonitor.Change` with named validation options.
- Update 'ExpressValidator.Extensions.DependencyInjection' to ExpressValidator 0.5.0.
- Update Microsoft nuget packages.
- Update README with a libraries/dependencies table.
- Add a configuration section in 'ExpressValidator.Extensions.DependencyInjection.Sample' for testing purposes.


## 0.3.2

- Update to ExpressValidator 0.2.0.
- Update the 'Microsoft.Extensions...' NuGet package.
- Update the 'Microsoft.Extensions...' and 'System...' NuGet packages in the 'ExpressValidator.Extensions.DependencyInjection.Tests' project.
- Update 'ExpressValidator.Extensions.DependencyInjection.Tests' to NUnit 4.3.2.
- Add a badge to the 'ExpressValidator.Extensions.DependencyInjection'.
- Added logo.
- Changed link to doc.
- Emojis in README.


## 0.3.0

- Add ability to dynamically update the validator parameters from options bound to the configuration section without restarting the application.
- Introduce the `IServiceCollection. AddExpressValidatorWithReload<T, TOptions>` extension method.
- Introduce `IExpressValidatorWithReload<TObj>` interface to dynamically rebuild `IExpressValidatorBuilder{TObj, TOptions}` to get new validator when options are changed.
- Update the README to illustrate the use of the new AddExpressValidatorWithReload method.
- Add 'Key Features' README Chapter.
- Update nuget README.
- Add the "/guesswithreload" and "/guesswithreloadasync" endpoints to the sample.
- Update sample README.


## 0.2.1

- Update ExpressValidator.Extensions.DependencyInjection to ExpressValidator 0.1.0.  
- Update ExpressValidator.Extensions.DependencyInjection.Tests to FluentValidation 11.11.0.


## 0.2.0

- Introduce the `IServiceCollection.AddExpressValidatorBuilder<T, TOptions>` extension methods.
- Extract solution for the `ExpressValidator.Extensions.DependencyInjection` sample project.
- Update microsoft nuget packages.
- Correct the `<PackageProjectUrl>` of ExpressValidator.Extensions.DependencyInjection.csproj.
- Update sample.
- Update README.
- Update NuGet README.