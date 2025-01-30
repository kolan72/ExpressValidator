# Introduction

ExpressValidator is a library that provides the ability to validate objects using the `FluentValidation` library, but without object inheritance from `AbstractValidator`.

## Key Features

- Easy on-the-fly creation of object validator class called `ExpressValidator` by using `ExpressValidatorBuilder`.
- Possibility to dynamically change the parameters of the `FluentValidation` validators (since _version_ 0.0.21).
- Supports adding a property or field for validation.
- Verifies that a property expression is a property and a field expression is a field, and throws `ArgumentException` if it is not.
- Supports adding a `Func` that provides a value for validation.
- Supports asynchronous validation.
- Targets .NET Standard 2.0+