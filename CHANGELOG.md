## 0.12.0

- Support .NET 8.0 and FluentValidation 12.0.0.
- Fix `ExpressPropertyValidator<TObj, T>` to prevent calling `Func<TObj, T>` propertyFunc twice when a success handler is present.
- Fix NU1504: Duplicate 'PackageReference' found
- Update NUnit NuGet package to v4.4.0.
- Add test for `ValidateAsync` with both `WithValidation` and `WithAsyncValidation` in `ExpressValidatorBuilder`.
- Add test to ensure synchronous Validate throws `AsyncValidatorInvokedSynchronouslyException` if the builder has async rules.
- Add a test for the `ValidateAsync` method with simulated external services.
- Add 'Asynchronous Validation' README Chapter.


## 0.10.0

- Introduced the `QuickValidator.ValidateAsync<T>(T, Action<IRuleBuilderOptions<T, T>>, string, Action<T>, CancellationToken)` extension method.
- Introduced the `QuickValidator.ValidateAsync<T>(T, Action<IRuleBuilderOptions<T, T>>, PropertyNameMode, Action<T>, CancellationToken)` extension method.
- Edit 'Quick Validation' README Chapter.
- Edit 'Quick Validation' NuGet README Chapter.


## 0.9.0

- Added quick validation support via `QuickValidator` and its `Validate<T>` overloads.
- Improve performance by applying options in `ExpressValidator<TObj, TOptions>` during the `ExpressValidatorBuilder<TObj, TOptions>.Build` call instead of at validation time.
- Introduce the `Unit` readonly struct.
- Add 'Nuances Of Using The Library' README Chapter.
- Add 'Nuances Of Using The Library' NuGet README Chapter.
- Add 'Quick Validation' README Chapter.
- Add 'Quick Validation' NuGet README Chapter.


## 0.5.0

- Introduced the `IExpressValidatorBuilder<TObj, TOptions>.BuildAndValidate<TObj, TOptions>(TObj, TOptions)` extension method.
- Introduced the `IExpressValidatorBuilder<TObj, TOptions>.BuildAndValidateAsync<TObj, TOptions>(TObj, TOptions, CancellationToken)` extension method.
- Introduced an optional `Action<T>` parameter in `ExpressValidatorBuilder.AddFunc` for handling successful `Func` result validation.
- Remove `ConfigureAwait(false)` in the loop where items are asynchronously added to `List<ValidationFailure>` as a precaution.
- Add 'Modifying FluentValidation Validator Parameters Using Options' chapter in README.


## 0.2.0

- Introduce `IExpressValidatorBuilder<TObj>.BuildAndValidate(TObj obj)` extension method.
- Introduce `IExpressValidatorBuilder<TObj>.BuildAndValidateAsync(TObj, CancellationToken)` extension method.
- Remove unused internal `ExpressValidatorBuilder<TObj, TOptions>.Options` property.
- Made the `TObj` parameter of the `IExpressPropertyValidator<TObj, T>`, `IExpressPropertyValidator<in TObj, TOptions, T>` interfaces contravariant.
- Add doc comments to the `IExpressValidatorBuilder<TObj>` interface.
- Add doc comments to the `IExpressValidatorBuilder<TObj, TOptions>` and `ExpressValidatorBuilder<TObj, TOptions>`.
- Add doc comments to the `IBuilderWithPropValidator<TObj, TOptions, T>` and `BuilderWithPropValidator<TObj, TOptions, T>`.
- Add documentation for the full API.
- Add documentation README Chapter.
- Change logo.


## 0.1.0

- Update to FluentValidation 11.11.0.  
- Made the TOptions parameter of the `IObjectValidator<TObj, TOptions>` contravariant.  
- Made the TObj parameter of the IObjectValidator<TObj>, `IObjectValidator<TObj, TOptions>` interfaces contravariant. 
- ExpressValidator codebase size reduced by moving `PropertyInfoToFuncConverter` class to bench project.  
- Unreference the Benchmark.csproj project from reference to the 'ExpressValidator.csproj' project.  
- Add the Rider-related .gitignore.  
- Remove unused using directives.  
- Update Benchmark.csproj nuget packages.


## 0.0.24

- Update to FluentValidation 11.10.0.
- Fix warnings in tests.
- Update to NUnit 4.2.2.


## 0.0.23

- Extract `IExpressValidatorBuilder<TObj>` interface.  
- Extract `IExpressValidatorBuilder<TObj, TOptions>` interface.


## 0.0.21

- Introduce the possibility to dynamically change the parameters of the 'FluentValidation' validators. (via `ExpressValidatorBuilder<TObj, TOptions>`, `ExpressValidator<TObj, TOptions>` classes).  
- Do not compare a non-nullable value type to null during pre-validation.  
- Refactor adding object member to validate by introducing `MemberInfoParser.ParseProperty`, `MemberInfoParser.ParseField` methods.  
- Slightly improve performance by using `PropertyInfo.GetGetMethod` to get a property from an object.  
- Add benchmark to get the fastest way to get `Func` from `PropertyInfo`.
- Remove redundant overload of the `PropertyInfo.GetTypedValue` extension method.  
- Remove redundant `MemberInfoParser.TryParse` method overload.  
- Update to 'FluentValidation' 11.9.2.  
- 'NUnit3TestAdapter' from 4.5.0 to 4.6.0.  
- Move builder-related classes to the new 'ValidatorBuilders' folder.  
- Update README.md and NuGet README.


## 0.0.16

- Support validation for nullable value types.
- Slightly improved performance by removing the restriction on the type of instance to validate in the `TypeValidatorBase<T>.Prevalidate` method.
- Make `_nullMessageProvider`, `_propName` and `_rule` `TypeValidatorBase<T>` class fields private instead of protected.
- Remove `IExpressPropertyValidatorBase` interface.
- Remove redundant `ExpressPropertyValidator` constructor.  


## 0.0.14

- Introduce `ExpressValidatorBuilder.AddFunc` method.
- Update to FluentValidation 11.9.1.
- Made the `BuilderWithPropValidator` constructor internal.
- Add `TObj` generic param to `IExpressPropertyValidator`, `ExpressPropertyValidator`.


## 0.0.10

- Support for adding a field for validation - introduced `ExpressValidatorBuilder.AddField` method.
- Made `IExpressPropertyValidatorBase` internal.
- Get rid of the `ExpressAsyncPropertyValidator`, `ExpressPropertyValidator` classes, rename `ExpressPropertyValidatorBase` to `ExpressPropertyValidator` and use it in sync and async scenarios.
- Remove the `BuilderWithPropValidator.PropertyValidator` property, make the `ExpressValidatorBuilder` property private.
- Move `IsAsync` property from `ExpressValidators` to `TypeValidators`.