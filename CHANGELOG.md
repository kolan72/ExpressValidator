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