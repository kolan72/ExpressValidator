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