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